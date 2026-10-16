using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPWMS.Web.Controllers.HRTransaction
{
    [Authorize]
    public class PayrollRunController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPayrollCalculationEngine _engine;

        public PayrollRunController(AppDbContext c, IPayrollCalculationEngine engine)
        {
            _context = c; _engine = engine;
        }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.PayrollRuns
                .Where(r => r.IsDeleted != true)
                .OrderByDescending(r => r.RunId)
                .Select(r => new {
                    runId = r.RunId, runNo = r.RunNo, year = r.Year, month = r.Month,
                    totalEmployees = r.TotalEmployees,
                    totalGross = r.TotalGross, totalDeductions = r.TotalDeductions, totalNet = r.TotalNet,
                    status = r.Status
                }).ToListAsync();
            return Json(l);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Calculate(int year, int month)
        {
            try
            {
                var run = await _engine.CalculateAsync(year, month, Uid());
                return Json(new { success = true, message = $"Payroll calculated for {run.TotalEmployees} employees!", runId = run.RunId });
            } catch (Exception ex) {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var run = await _context.PayrollRuns.FirstOrDefaultAsync(r => r.RunId == id);
            if (run == null) return NotFound();

            var settings = await _context.PayrollSettings.OrderByDescending(p => p.SettingId).FirstOrDefaultAsync();
            int divisor = settings?.SalaryDivisor ?? 30;
            int lateHalfDayCount = settings?.LateHalfDayCount ?? 3;
            int lateFullDayCount = settings?.LateFullDayCount ?? 6;
            decimal otMultiplier = settings?.OTMultiplier ?? 1.5m;

            var regs = await _context.PayrollRegisters
                .Include(r => r.Employee)
                .Where(r => r.RunId == id)
                .OrderBy(r => r.Employee.EmployeeCode)
                .ToListAsync();

            var result = new List<object>();
            foreach (var r in regs)
            {
                decimal dailyRate = r.BasicSalary > 0 && divisor > 0 ? r.BasicSalary / divisor : 0;
                decimal hourlyRate = dailyRate / 8;

                // Get active loans for this employee
                var loans = await _context.Loans
                    .Where(l => l.EmployeeId == r.EmployeeId
                        && l.Status == "Active" && l.IsDeleted != true)
                    .Select(l => new {
                        loanNo = l.LoanNo,
                        loanAmount = l.LoanAmount,
                        totalRecovered = l.TotalRecovered,
                        remainingAmount = l.RemainingAmount,
                        monthlyRecovery = l.MonthlyRecovery
                    })
                    .ToListAsync();

                // Get approved OT in this month
                var monthStart = new DateOnly(run.Year, run.Month, 1);
                var monthEnd = new DateOnly(run.Year, run.Month, DateTime.DaysInMonth(run.Year, run.Month));

                var ots = await _context.Overtimes
                    .Where(o => o.EmployeeId == r.EmployeeId
                        && o.OvertimeDate >= monthStart && o.OvertimeDate <= monthEnd
                        && o.Status == "Approved" && o.IsDeleted != true)
                    .Select(o => new {
                        date = o.OvertimeDate.ToString("dd-MMM"),
                        hours = o.OTHours,
                        rate = o.OTRate,
                        multiplier = o.OTMultiplier,
                        amount = o.OTAmount,
                        isHoliday = o.IsHolidayWork,
                        isWeeklyOff = o.IsWeeklyOffWork
                    })
                    .ToListAsync();

                result.Add(new
                {
                    registerId = r.RegisterId,
                    employeeId = r.EmployeeId,
                    employeeCode = r.Employee.EmployeeCode,
                    employeeName = r.Employee.FirstName + " " + r.Employee.LastName,

                    dailyRate = dailyRate,
                    hourlyRate = hourlyRate,
                    divisor = divisor,
                    lateHalfDayCount = lateHalfDayCount,
                    lateFullDayCount = lateFullDayCount,
                    otMultiplier = otMultiplier,

                    basicSalary = r.BasicSalary,
                    houseRent = r.HouseRent,
                    utilityAllowance = r.UtilityAllowance,
                    medicalAllowance = r.MedicalAllowance,
                    otherAllowances = r.OtherAllowances,
                    grossSalary = r.GrossSalary,
                    overtimeAmount = r.OvertimeAmount,
                    overtimeHours = r.OvertimeHours,
                    bonusAmount = r.BonusAmount,
                    commissionAmount = r.CommissionAmount,
                    totalEarnings = r.TotalEarnings,

                    incomeTax = r.IncomeTax,
                    professionalTax = r.ProfessionalTax,
                    lateDeduction = r.LateDeduction,
                    lateDays = r.LateDays,
                    absenceDeduction = r.AbsenceDeduction,
                    absentDays = r.AbsentDays,
                    unpaidLeaveDeduction = r.UnpaidLeaveDeduction,
                    loanRecovery = r.LoanRecovery,
                    advanceRecovery = r.AdvanceRecovery,
                    otherDeductions = r.OtherDeductions,
                    totalDeductions = r.TotalDeductions,

                    netPay = r.NetPay,

                    workingDays = r.WorkingDays,
                    presentDays = r.PresentDays,
                    leaveDays = r.LeaveDays,

                    calculationNotes = r.CalculationNotes,
                    loans = loans,
                    overtimeDetails = ots
                });
            }

            return Json(new {
                run = new {
                    runId = run.RunId, runNo = run.RunNo, year = run.Year, month = run.Month,
                    totalEmployees = run.TotalEmployees,
                    totalGross = run.TotalGross, totalDeductions = run.TotalDeductions,
                    totalNet = run.TotalNet, status = run.Status,
                    divisor = divisor
                },
                registers = result
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var r = await _context.PayrollRuns.FindAsync(id);
            if (r == null) return Json(new { success = false, message = "Not found" });

            if (r.Status == "Approved" || r.Status == "Locked" || r.Status == "Paid")
                return Json(new { success = false, message = "Payroll already approved." });
            if (r.Status != "Draft" && r.Status != "Submitted")
                return Json(new { success = false, message = "Only Draft/Submitted can be approved." });
            if (r.CreatedBy == Uid().ToString())
                return Json(new { success = false, message = "You cannot approve your own payroll run." });

            using var tx = await _context.Database.BeginTransactionAsync();
            try {
            
            // ===== UPDATE LOAN RECOVERIES =====
            var registers = await _context.PayrollRegisters
                .Where(reg => reg.RunId == id)
                .ToListAsync();

            foreach (var reg in registers) {
                if (reg.LoanRecovery > 0) {
                    var loans = await _context.Loans
                        .Where(l => l.EmployeeId == reg.EmployeeId
                            && l.Status == "Active" && l.IsDeleted != true)
                        .ToListAsync();

                    decimal remainingToRecover = reg.LoanRecovery;
                    foreach (var loan in loans) {
                        if (remainingToRecover <= 0) break;
                        decimal recoverAmount = Math.Min(loan.MonthlyRecovery, Math.Min(loan.RemainingAmount, remainingToRecover));

                        // Add recovery record
                        _context.LoanRecoveries.Add(new LoanRecovery {
                            LoanId = loan.LoanId,
                            RecoveryDate = new DateOnly(r.Year, r.Month, DateTime.DaysInMonth(r.Year, r.Month)),
                            Amount = recoverAmount,
                            Status = "Paid",
                            IsProcessedInPayroll = true,
                            Remarks = $"Payroll {r.RunNo}",
                            CreatedBy = Uid().ToString(),
                            CreatedDate = DateTime.Now
                        });

                        // Update loan
                        loan.TotalRecovered += recoverAmount;
                        loan.RemainingAmount -= recoverAmount;
                        loan.PaidInstallments += 1;

                        if (loan.RemainingAmount <= 0) {
                            loan.RemainingAmount = 0;
                            loan.Status = "Closed";
                        }

                        remainingToRecover -= recoverAmount;
                    }
                }
            }

            r.Status = "Approved";
            r.ApprovedBy = Uid().ToString();
            r.ApprovedDate = DateTime.Now;
            r.UpdatedBy = Uid().ToString();
            r.UpdatedDate = DateTime.Now;
            _context.PayrollApprovalHistories.Add(new PayrollApprovalHistory {
                RunId = id, Action = "Approved", ToStatus = "Approved",
                ActionBy = Uid().ToString(), ActionDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return Json(new { success = true, message = "Payroll approved! Loan recoveries applied." });
            } catch (Exception ex) {
                await tx.RollbackAsync();
                System.Diagnostics.Debug.WriteLine(ex);
                return Json(new { success = false, message = "Approval failed. Please try again." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(int id)
        {
            var r = await _context.PayrollRuns.FindAsync(id);
            if (r == null) return Json(new { success = false, message = "Not found" });
            r.Status = "Locked";
            r.LockedDate = DateTime.Now;
            r.UpdatedBy = Uid().ToString();
            r.UpdatedDate = DateTime.Now;
            _context.PayrollApprovalHistories.Add(new PayrollApprovalHistory {
                RunId = id, Action = "Locked", ToStatus = "Locked",
                ActionBy = Uid().ToString(), ActionDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Payroll locked!" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _context.PayrollRuns.FindAsync(id);
            if (r == null) return Json(new { success = false, message = "Not found" });

            if (r.Status == "Approved" || r.Status == "Locked" || r.Status == "Paid")
                return Json(new { success = false, message = "Approved/Locked payrolls cannot be deleted." });

            r.IsDeleted = true; r.IsActive = false;
            r.UpdatedBy = Uid().ToString();
            r.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }
    }
}


