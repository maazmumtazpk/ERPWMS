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
    [Authorize(Policy = "Payroll.Approve")]
    public class PayrollReportsController : Controller
    {
        private readonly AppDbContext _context;
        public PayrollReportsController(AppDbContext c) { _context = c; }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> AttendanceSummary(int year, int month)
        {
            var start = new DateOnly(year, month, 1);
            var end = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            var data = await _context.Employees
                .Where(e => e.IsDeleted != true && e.IsActive == true)
                .Select(e => new {
                    employeeCode = e.EmployeeCode,
                    employeeName = e.FirstName + " " + e.LastName,
                    presentDays = _context.EmployeeAttendances.Count(a =>
                        a.EmployeeId == e.EmployeeId && a.AttendanceDate >= start && a.AttendanceDate <= end
                        && a.Status != "Absent"),
                    absentDays = _context.EmployeeAttendances.Count(a =>
                        a.EmployeeId == e.EmployeeId && a.AttendanceDate >= start && a.AttendanceDate <= end
                        && a.Status == "Absent"),
                    lateDays = _context.EmployeeAttendances.Count(a =>
                        a.EmployeeId == e.EmployeeId && a.AttendanceDate >= start && a.AttendanceDate <= end
                        && a.LateMinutes > 0),
                    totalOT = _context.EmployeeAttendances
                        .Where(a => a.EmployeeId == e.EmployeeId && a.AttendanceDate >= start && a.AttendanceDate <= end)
                        .Sum(a => a.OTHours ?? 0)
                })
                .OrderBy(e => e.employeeCode)
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> PayrollRegister(int runId)
        {
            var data = await _context.PayrollRegisters
                .Include(r => r.Employee)
                .Where(r => r.RunId == runId)
                .OrderBy(r => r.Employee.EmployeeCode)
                .Select(r => new {
                    employeeCode = r.Employee.EmployeeCode,
                    employeeName = r.Employee.FirstName + " " + r.Employee.LastName,
                    basicSalary = r.BasicSalary,
                    grossSalary = r.GrossSalary,
                    overtimeAmount = r.OvertimeAmount,
                    bonusAmount = r.BonusAmount,
                    totalEarnings = r.TotalEarnings,
                    incomeTax = r.IncomeTax,
                    lateDeduction = r.LateDeduction,
                    absenceDeduction = r.AbsenceDeduction,
                    loanRecovery = r.LoanRecovery,
                    totalDeductions = r.TotalDeductions,
                    netPay = r.NetPay
                })
                .ToListAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> LeaveReport(int year)
        {
            var data = await _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Where(l => l.FromDate.Year == year && l.IsDeleted != true)
                .GroupBy(l => new { l.EmployeeId, l.Employee.EmployeeCode, l.Employee.FirstName, l.Employee.LastName })
                .Select(g => new {
                    employeeCode = g.Key.EmployeeCode,
                    employeeName = g.Key.FirstName + " " + g.Key.LastName,
                    totalRequests = g.Count(),
                    approvedDays = g.Where(l => l.Status == "Approved").Sum(l => l.TotalDays),
                    pendingDays = g.Where(l => l.Status == "Pending").Sum(l => l.TotalDays),
                    rejectedDays = g.Where(l => l.Status == "Rejected").Sum(l => l.TotalDays)
                })
                .OrderBy(x => x.employeeCode)
                .ToListAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> LoanReport()
        {
            var data = await _context.Loans
                .Include(l => l.Employee)
                .Where(l => l.IsDeleted != true)
                .OrderByDescending(l => l.LoanId)
                .Select(l => new {
                    loanNo = l.LoanNo,
                    employeeCode = l.Employee.EmployeeCode,
                    employeeName = l.Employee.FirstName + " " + l.Employee.LastName,
                    loanAmount = l.LoanAmount,
                    monthlyRecovery = l.MonthlyRecovery,
                    totalRecovered = l.TotalRecovered,
                    remainingAmount = l.RemainingAmount,
                    paidInstallments = l.PaidInstallments,
                    totalInstallments = l.TotalInstallments,
                    status = l.Status
                })
                .ToListAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> OvertimeReport(int year, int month)
        {
            var start = new DateOnly(year, month, 1);
            var end = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            var data = await _context.Overtimes
                .Include(o => o.Employee)
                .Where(o => o.OvertimeDate >= start && o.OvertimeDate <= end && o.IsDeleted != true)
                .OrderByDescending(o => o.OvertimeDate)
                .Select(o => new {
                    overtimeDate = o.OvertimeDate.ToString("yyyy-MM-dd"),
                    employeeCode = o.Employee.EmployeeCode,
                    employeeName = o.Employee.FirstName + " " + o.Employee.LastName,
                    otHours = o.OTHours,
                    otRate = o.OTRate,
                    otAmount = o.OTAmount,
                    isHoliday = o.IsHolidayWork,
                    isWeeklyOff = o.IsWeeklyOffWork,
                    status = o.Status
                })
                .ToListAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> SalaryReport()
        {
            var data = await _context.EmployeeSalaries1
                .Include(s => s.Employee)
                .Where(s => s.IsDeleted != true && s.IsActive == true)
                .OrderBy(s => s.Employee.EmployeeCode)
                .Select(s => new {
                    employeeCode = s.Employee.EmployeeCode,
                    employeeName = s.Employee.FirstName + " " + s.Employee.LastName,
                    effectiveDate = s.EffectiveDate.ToString("yyyy-MM-dd"),
                    basicSalary = s.BasicSalary,
                    totalAllowances = s.TotalAllowances,
                    totalDeductions = s.TotalDeductions,
                    netSalary = s.NetSalary
                })
                .ToListAsync();
            return Json(data);
        }
    }
}


