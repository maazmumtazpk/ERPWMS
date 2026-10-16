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
    public class LoanController : Controller
    {
        private readonly AppDbContext _context;
        public LoanController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }

        private const decimal MinLoanAmount = 1000m;
        private const decimal MaxLoanAmount = 5_000_000m;
        private const int MaxInstallments = 120;
        private const decimal MaxRecoveryRatio = 0.5m;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.Loans.Include(x => x.Employee)
                .Where(x => x.IsDeleted != true)
                .OrderByDescending(x => x.LoanId)
                .Select(x => new {
                    loanId = x.LoanId, loanNo = x.LoanNo,
                    employeeName = x.Employee.FirstName + " " + x.Employee.LastName,
                    employeeCode = x.Employee.EmployeeCode,
                    loanAmount = x.LoanAmount,
                    monthlyRecovery = x.MonthlyRecovery,
                    totalRecovered = x.TotalRecovered,
                    remainingAmount = x.RemainingAmount,
                    paidInstallments = x.PaidInstallments,
                    totalInstallments = x.TotalInstallments,
                    status = x.Status,
                    issuedDate = x.IssuedDate.ToString("yyyy-MM-dd")
                }).ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees() =>
            Json(await _context.Employees.Where(e => e.IsDeleted != true && e.IsActive == true)
                .Select(e => new {
                    employeeId = e.EmployeeId,
                    employeeCode = e.EmployeeCode,
                    fullName = e.FirstName + " " + e.LastName
                }).OrderBy(e => e.fullName).ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Edit")]
        public async Task<IActionResult> Save(int loanId, int employeeId, decimal loanAmount,
            decimal monthlyRecovery, int totalInstallments, string issuedDate, string? reason)
        {
            try
            {
                if (employeeId <= 0) return Json(new { success = false, message = "Employee required." });
                if (!DateOnly.TryParse(issuedDate, out var dt)) return Json(new { success = false, message = "Invalid date." });

                if (loanAmount < MinLoanAmount || loanAmount > MaxLoanAmount)
                    return Json(new { success = false, message = $"Amount must be between {MinLoanAmount:N0} and {MaxLoanAmount:N0}." });

                if (monthlyRecovery <= 0)
                    return Json(new { success = false, message = "Monthly recovery must be > 0." });

                if (totalInstallments <= 0 || totalInstallments > MaxInstallments)
                    return Json(new { success = false, message = $"Installments must be 1-{MaxInstallments}." });

                decimal expectedTotal = monthlyRecovery * totalInstallments;
                decimal tolerance = loanAmount * 0.05m;
                if (Math.Abs(expectedTotal - loanAmount) > tolerance)
                    return Json(new { success = false, message = "Amount does not match recovery × installments." });

                var emp = await _context.Employees.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.IsActive == true && e.IsDeleted != true);
                if (emp == null) return Json(new { success = false, message = "Employee not found." });

                if (emp.BaseSalary > 0 && monthlyRecovery > emp.BaseSalary * MaxRecoveryRatio)
                    return Json(new { success = false, message = $"Recovery cannot exceed {MaxRecoveryRatio:P0} of salary." });

                var activeExists = await _context.Loans.AnyAsync(l => l.EmployeeId == employeeId
                    && l.Status == "Active" && l.IsDeleted != true);
                if (activeExists && loanId <= 0)
                    return Json(new { success = false, message = "Employee already has an active loan." });

                Loan loan;
                if (loanId > 0)
                {
                    loan = await _context.Loans.FindAsync(loanId);
                    if (loan == null) return Json(new { success = false, message = "Not found." });
                    if (loan.Status != "Pending")
                        return Json(new { success = false, message = "Only pending loans can be edited." });
                }
                else
                {
                    loan = new Loan
                    {
                        LoanNo = "LN-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                        RemainingAmount = loanAmount,
                        TotalRecovered = 0,
                        PaidInstallments = 0,
                        Status = "Pending",
                        IsDeleted = false,
                        IsActive = true,
                        CreatedBy = Uid().ToString(),
                        CreatedDate = DateTime.Now
                    };
                    _context.Loans.Add(loan);
                }

                loan.EmployeeId = employeeId;
                loan.LoanAmount = loanAmount;
                loan.MonthlyRecovery = monthlyRecovery;
                loan.TotalInstallments = totalInstallments;
                loan.IssuedDate = dt;
                loan.Reason = reason;

                if (loanId > 0)
                {
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = loanId > 0 ? "Updated!" : "Created!" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var l = await _context.Loans.FindAsync(id);
            if (l == null) return Json(new { success = false, message = "Not found" });
            if (l.Status != "Pending") return Json(new { success = false, message = "Only pending can be approved." });

            var currentUid = Uid().ToString();
            if (l.CreatedBy == currentUid)
                return Json(new { success = false, message = "You cannot approve your own loan request." });

            l.Status = "Active";
            l.StartRecoveryDate = DateOnly.FromDateTime(DateTime.Today);
            l.ApprovedBy = currentUid;
            l.ApprovedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Loan approved!" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Approve")]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var l = await _context.Loans.FindAsync(id);
            if (l == null) return Json(new { success = false, message = "Not found" });
            if (l.Status != "Pending") return Json(new { success = false, message = "Only pending can be rejected." });
            if (string.IsNullOrWhiteSpace(reason)) return Json(new { success = false, message = "Reason required." });

            l.Status = "Rejected";
            l.Reason = (l.Reason ?? "") + " | Rejected: " + reason;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Loan rejected." });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var l = await _context.Loans.FindAsync(id);
            if (l == null) return Json(new { success = false, message = "Not found" });

            if (l.Status == "Active" || l.Status == "Closed")
                return Json(new { success = false, message = "Active/Closed loans cannot be deleted." });

            l.IsDeleted = true;
            l.IsActive = false;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }
    }
}


