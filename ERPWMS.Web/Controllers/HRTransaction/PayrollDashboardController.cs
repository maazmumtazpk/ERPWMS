using ERPWMS.Database;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Web.Controllers.HRTransaction
{
    [Authorize]
    public class PayrollDashboardController : Controller
    {
        private readonly AppDbContext _context;
        public PayrollDashboardController(AppDbContext c) { _context = c; }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetSummary()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            var totalEmp = await _context.Employees.CountAsync(e => e.IsDeleted != true && e.IsActive == true);
            var totalPresent = await _context.EmployeeAttendances.CountAsync(a => a.AttendanceDate == today && a.Status != "Absent");
            var totalAbsent = await _context.EmployeeAttendances.CountAsync(a => a.AttendanceDate == today && a.Status == "Absent");
            var totalLate = await _context.EmployeeAttendances.CountAsync(a => a.AttendanceDate == today && a.LateMinutes > 0);
            var onLeave = await _context.LeaveRequests.CountAsync(l =>
                l.Status == "Approved" &&
                l.FromDate <= today && l.ToDate >= today &&
                l.IsDeleted != true);

            var pendingLeaves = await _context.LeaveRequests.CountAsync(l => l.Status == "Pending" && l.IsDeleted != true);
            var pendingOT = await _context.Overtimes.CountAsync(o => o.Status == "Pending" && o.IsDeleted != true);
            var pendingCorrections = await _context.AttendanceCorrectionRequests.CountAsync(c => c.Status == "Pending" && c.IsDeleted != true);
            var pendingMissing = await _context.MissingPunchRequests.CountAsync(m => m.Status == "Pending" && m.IsDeleted != true);

            var currentPayroll = await _context.PayrollRuns
                .Where(r => r.Year == year && r.Month == month && r.IsDeleted != true)
                .OrderByDescending(r => r.RunId)
                .FirstOrDefaultAsync();

            var totalLoans = await _context.Loans
                .Where(l => l.Status == "Active" && l.IsDeleted != true)
                .SumAsync(l => l.RemainingAmount);

            var thisMonthBonuses = await _context.Bonuses
                .Where(b => b.BonusDate.Year == year && b.BonusDate.Month == month && b.IsDeleted != true)
                .SumAsync(b => b.Amount);

            return Json(new {
                totalEmployees = totalEmp,
                presentToday = totalPresent,
                absentToday = totalAbsent,
                lateToday = totalLate,
                onLeaveToday = onLeave,
                pendingLeaves = pendingLeaves,
                pendingOT = pendingOT,
                pendingCorrections = pendingCorrections,
                pendingMissing = pendingMissing,
                currentPayroll = currentPayroll == null ? null : new {
                    runNo = currentPayroll.RunNo,
                    totalEmployees = currentPayroll.TotalEmployees,
                    totalGross = currentPayroll.TotalGross,
                    totalDeductions = currentPayroll.TotalDeductions,
                    totalNet = currentPayroll.TotalNet,
                    status = currentPayroll.Status
                },
                totalLoanOutstanding = totalLoans,
                monthlyBonuses = thisMonthBonuses
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyTrend()
        {
            var year = DateTime.Now.Year;
            var data = await _context.PayrollRuns
                .Where(r => r.Year == year && r.IsDeleted != true)
                .OrderBy(r => r.Month)
                .Select(r => new {
                    month = r.Month,
                    totalGross = r.TotalGross,
                    totalNet = r.TotalNet,
                    employees = r.TotalEmployees
                })
                .ToListAsync();
            return Json(data);
        }
    }
}

