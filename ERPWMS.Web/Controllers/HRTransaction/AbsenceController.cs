using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Infrastructure.Services;
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
    public class AbsenceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAbsenceCalculator _calc;

        public AbsenceController(AppDbContext c, IAbsenceCalculator calc) { _context = c; _calc = calc; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var list = await _context.Employees
                .Where(e => e.IsDeleted != true && e.IsActive == true)
                .Select(e => new { employeeId = e.EmployeeId, employeeCode = e.EmployeeCode, fullName = e.FirstName + " " + e.LastName })
                .OrderBy(e => e.fullName).ToListAsync();
            return Json(list);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Mark(int employeeId, string absenceDate, decimal deductionDays, string? remarks)
        {
            try
            {
                if (employeeId <= 0) return Json(new { success = false, message = "Employee required." });
                if (!DateOnly.TryParse(absenceDate, out var date)) return Json(new { success = false, message = "Invalid date." });

                var exists = await _context.AbsenceRecords.AnyAsync(a =>
                    a.EmployeeId == employeeId && a.AbsenceDate == date && a.IsDeleted != true);
                if (exists) return Json(new { success = false, message = "Absence already marked for this date." });

                _context.AbsenceRecords.Add(new AbsenceRecord {
                    EmployeeId = employeeId,
                    AbsenceDate = date,
                    AbsenceType = "Unpaid",
                    DeductionDays = deductionDays > 0 ? deductionDays : 1.0m,
                    Remarks = remarks,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Absence marked!" });
            }
            catch (Exception ex) {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Calculate(int employeeId, int year, int month)
        {
            var r = await _calc.CalculateAsync(employeeId, year, month);
            return Json(new {
                success = true,
                absentDays = r.AbsentDays,
                deductionDays = r.DeductionDays,
                dailyRate = r.DailyRate,
                totalDeduction = r.TotalDeduction,
                summary = r.Summary
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.AbsenceRecords
                .Include(a => a.Employee)
                .Where(a => a.IsDeleted != true)
                .OrderByDescending(a => a.AbsenceDate)
                .Take(100)
                .Select(a => new {
                    absenceId = a.AbsenceId,
                    employeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                    employeeCode = a.Employee.EmployeeCode,
                    absenceDate = a.AbsenceDate.ToString("yyyy-MM-dd"),
                    absenceType = a.AbsenceType,
                    deductionDays = a.DeductionDays,
                    remarks = a.Remarks
                }).ToListAsync();
            return Json(list);
        }
    }
}
