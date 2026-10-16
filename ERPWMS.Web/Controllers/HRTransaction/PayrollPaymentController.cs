using ERPWMS.Database;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPWMS.Web.Controllers.HRTransaction
{
    [Authorize]
    public class PayrollPaymentController : Controller
    {
        private readonly AppDbContext _context;
        public PayrollPaymentController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }

        public IActionResult Index() => View();

        // ========== GET ALL PAYMENTS ==========
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.PayrollPayments
                .Include(p => p.Employee)
                .Where(p => p.IsDeleted != true)
                .OrderByDescending(p => p.PaymentId)
                .Take(200)
                .Select(p => new {
                    paymentId = p.PaymentId,
                    paymentNo = p.PaymentNo,
                    employeeName = p.Employee.FirstName + " " + p.Employee.LastName,
                    employeeCode = p.Employee.EmployeeCode,
                    amount = p.Amount,
                    paymentDate = p.PaymentDate.ToString("yyyy-MM-dd"),
                    paymentMethod = p.PaymentMethod,
                    referenceNo = p.ReferenceNo,
                    bankName = p.BankName,
                    status = p.Status
                }).ToListAsync();
            return Json(l);
        }

        // ========== GET APPROVED RUNS ==========
        [HttpGet]
        public async Task<IActionResult> GetApprovedRuns()
        {
            var l = await _context.PayrollRuns
                .Where(r => r.IsDeleted != true && 
                    (r.Status == "Approved" || r.Status == "Posted" || r.Status == "Paid"))
                .OrderByDescending(r => r.RunId)
                .Select(r => new {
                    runId = r.RunId,
                    runNo = r.RunNo,
                    year = r.Year,
                    month = r.Month,
                    status = r.Status
                })
                .ToListAsync();
            return Json(l);
        }

        // ========== GET RUN EMPLOYEES WITH BANK DETAILS ==========
        [HttpGet]
        public async Task<IActionResult> GetRunEmployees(int runId)
        {
            var run = await _context.PayrollRuns.FirstOrDefaultAsync(r => r.RunId == runId);
            if (run == null)
                return Json(new { success = false, message = "Run not found" });

            var registers = await _context.PayrollRegisters
                .Include(r => r.Employee)
                .Where(r => r.RunId == runId && r.IsDeleted != true)
                .OrderBy(r => r.Employee.EmployeeCode)
                .ToListAsync();

            var empIds = registers.Select(r => r.EmployeeId).ToList();

            var bankAccounts = await _context.EmployeeBankAccounts
                .Where(b => empIds.Contains(b.EmployeeId) && b.IsDeleted != true && b.IsActive == true)
                .ToListAsync();

            var existingPayments = await _context.PayrollPayments
                .Where(p => p.RunId == runId && p.IsDeleted != true)
                .ToListAsync();

            var result = registers.Select(r => {
                var bank = bankAccounts
                    .Where(b => b.EmployeeId == r.EmployeeId)
                    .OrderByDescending(b => b.IsPrimary == true)
                    .ThenByDescending(b => b.BankAccountId)
                    .FirstOrDefault();

                var payment = existingPayments.FirstOrDefault(p => p.EmployeeId == r.EmployeeId);

                return new {
                    registerId = r.RegisterId,
                    employeeId = r.EmployeeId,
                    employeeCode = r.Employee.EmployeeCode,
                    employeeName = r.Employee.FirstName + " " + r.Employee.LastName,
                    netPay = r.NetPay,
                    hasBankAccount = bank != null,
                    bankName = bank != null ? bank.BankName : null,
                    accountNumber = bank != null ? bank.AccountNumber : null,
                    iban = bank != null ? bank.Iban : null,
                    accountType = bank != null ? bank.AccountType : null,
                    isPaid = payment != null,
                    paymentNo = payment != null ? payment.PaymentNo : null,
                    paymentMethod = payment != null ? payment.PaymentMethod : null,
                    paymentDate = payment != null ? payment.PaymentDate.ToString("yyyy-MM-dd") : null,
                    referenceNo = payment != null ? payment.ReferenceNo : null
                };
            }).ToList();

            return Json(new {
                success = true,
                run = new { runId = run.RunId, runNo = run.RunNo, year = run.Year, month = run.Month },
                employees = result
            });
        }

        // ========== PAY INDIVIDUAL ==========
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Approve")]
        public async Task<IActionResult> PayIndividual(
            int runId, int registerId, int employeeId,
            string paymentMethod, string? referenceNo, string? chequeNo)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var run = await _context.PayrollRuns.FindAsync(runId);
                if (run == null)
                    return Json(new { success = false, message = "Payroll run not found." });

                if (run.Status != "Approved" && run.Status != "Posted" && run.Status != "Paid")
                    return Json(new { success = false, message = "Payroll must be approved first." });

                var reg = await _context.PayrollRegisters.FindAsync(registerId);
                if (reg == null)
                    return Json(new { success = false, message = "Register not found." });

                if (reg.NetPay <= 0)
                    return Json(new { success = false, message = "Net pay is zero or negative." });

                var bank = await _context.EmployeeBankAccounts
                    .Where(b => b.EmployeeId == employeeId && b.IsDeleted != true && b.IsActive == true)
                    .OrderByDescending(b => b.IsPrimary == true)
                    .ThenByDescending(b => b.BankAccountId)
                    .FirstOrDefaultAsync();

                if (bank == null)
                    return Json(new { success = false, message = "Cannot pay — bank account not linked." });

                var exists = await _context.PayrollPayments.AnyAsync(p =>
                    p.RunId == runId && p.EmployeeId == employeeId && p.IsDeleted != true);
                if (exists)
                    return Json(new { success = false, message = "Payment already processed." });

                _context.PayrollPayments.Add(new PayrollPayment {
                    RunId = runId,
                    EmployeeId = employeeId,
                    PaymentNo = $"PAY-{run.Year}{run.Month:D2}-{employeeId:D5}",
                    Amount = reg.NetPay,
                    PaymentDate = DateOnly.FromDateTime(DateTime.Today),
                    PaymentMethod = paymentMethod ?? "BankTransfer",
                    ReferenceNo = referenceNo,
                    ChequeNo = chequeNo,
                    BankName = bank.BankName,
                    Status = "Paid",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });

                var totalRegs = await _context.PayrollRegisters.CountAsync(r => r.RunId == runId);
                var paidCount = await _context.PayrollPayments
                    .CountAsync(p => p.RunId == runId && p.IsDeleted != true);

                if (paidCount + 1 >= totalRegs) {
                    run.Status = "Paid";
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return Json(new { success = true, message = "Payment processed!", amount = reg.NetPay });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                System.Diagnostics.Debug.WriteLine(ex);
                return Json(new { success = false, message = "Payment failed. Please try again." });
            }
        }

        // ========== PAY ALL PENDING ==========
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Approve")]
        public async Task<IActionResult> PayAllPending(int runId, string paymentMethod, string? referenceNo)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var run = await _context.PayrollRuns.FindAsync(runId);
                if (run == null)
                    return Json(new { success = false, message = "Payroll run not found." });

                if (run.Status != "Approved" && run.Status != "Posted" && run.Status != "Paid")
                    return Json(new { success = false, message = "Payroll must be approved first." });

                var registers = await _context.PayrollRegisters
                    .Where(r => r.RunId == runId && r.IsDeleted != true)
                    .ToListAsync();

                var existingPaid = await _context.PayrollPayments
                    .Where(p => p.RunId == runId && p.IsDeleted != true)
                    .Select(p => p.EmployeeId)
                    .ToListAsync();

                var empIds = registers.Select(r => r.EmployeeId).ToList();
                var bankAccounts = await _context.EmployeeBankAccounts
                    .Where(b => empIds.Contains(b.EmployeeId) && b.IsDeleted != true && b.IsActive == true)
                    .ToListAsync();

                int created = 0;
                int skipped = 0;

                foreach (var reg in registers)
                {
                    if (existingPaid.Contains(reg.EmployeeId)) continue;

                    var bank = bankAccounts
                        .Where(b => b.EmployeeId == reg.EmployeeId)
                        .OrderByDescending(b => b.IsPrimary == true)
                        .ThenByDescending(b => b.BankAccountId)
                        .FirstOrDefault();

                    if (bank == null) { skipped++; continue; }

                    _context.PayrollPayments.Add(new PayrollPayment {
                        RunId = runId,
                        EmployeeId = reg.EmployeeId,
                        PaymentNo = $"PAY-{run.Year}{run.Month:D2}-{reg.EmployeeId:D5}",
                        Amount = reg.NetPay,
                        PaymentDate = DateOnly.FromDateTime(DateTime.Today),
                        PaymentMethod = paymentMethod ?? "BankTransfer",
                        ReferenceNo = referenceNo,
                        BankName = bank.BankName,
                        Status = "Paid",
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = Uid().ToString(),
                        CreatedDate = DateTime.Now
                    });
                    created++;
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                string msg = $"{created} payments processed!";
                if (skipped > 0) msg += $" ({skipped} skipped — no bank account)";
                return Json(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                System.Diagnostics.Debug.WriteLine(ex);
                return Json(new { success = false, message = "Bulk payment failed. No payments were made." });
            }
        }

        // ========== DELETE ==========
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _context.PayrollPayments.FindAsync(id);
            if (p == null)
                return Json(new { success = false, message = "Not found" });

            p.IsDeleted = true;
            p.IsActive = false;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }
    }
}
