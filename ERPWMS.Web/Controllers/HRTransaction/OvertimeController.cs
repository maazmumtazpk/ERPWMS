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
    public class OvertimeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IOvertimeCalculator _calc;

        public OvertimeController(AppDbContext c, IOvertimeCalculator calc) { _context = c; _calc = calc; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.Overtimes
                .Include(o => o.Employee)
                .Where(o => o.IsDeleted != true)
                .OrderByDescending(o => o.OvertimeDate)
                .Select(o => new {
                    overtimeId = o.OvertimeId,
                    employeeName = o.Employee.FirstName + " " + o.Employee.LastName,
                    employeeCode = o.Employee.EmployeeCode,
                    overtimeDate = o.OvertimeDate.ToString("yyyy-MM-dd"),
                    otHours = o.OTHours,
                    otRate = o.OTRate,
                    otMultiplier = o.OTMultiplier,
                    otAmount = o.OTAmount,
                    isHolidayWork = o.IsHolidayWork,
                    isWeeklyOffWork = o.IsWeeklyOffWork,
                    status = o.Status
                }).ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var l = await _context.Employees.Where(e => e.IsDeleted != true && e.IsActive == true)
                .Select(e => new { employeeId = e.EmployeeId, employeeCode = e.EmployeeCode, fullName = e.FirstName + " " + e.LastName })
                .OrderBy(e => e.fullName).ToListAsync();
            return Json(l);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Record(int employeeId, string overtimeDate, decimal otHours, bool isHolidayWork, bool isWeeklyOffWork, string? remarks)
        {
            try
            {
                if (employeeId <= 0) return Json(new { success = false, message = "Employee required." });
                if (!DateOnly.TryParse(overtimeDate, out var date)) return Json(new { success = false, message = "Invalid date." });
                if (otHours <= 0) return Json(new { success = false, message = "OT hours must be > 0." });

                var calc = await _calc.CalculateAsync(employeeId, otHours, isHolidayWork, isWeeklyOffWork);

                var entity = new Overtime {
                    EmployeeId = employeeId, OvertimeDate = date,
                    OTHours = otHours, OTRate = calc.OTRate, OTMultiplier = calc.OTMultiplier, OTAmount = calc.OTAmount,
                    IsHolidayWork = isHolidayWork, IsWeeklyOffWork = isWeeklyOffWork,
                    Status = "Pending", Remarks = remarks, IsActive = true, IsDeleted = false,
                    CreatedBy = Uid().ToString(), CreatedDate = DateTime.Now
                };
                _context.Overtimes.Add(entity);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"OT recorded! Amount: {calc.OTAmount:N2}" });
            } catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id) 
        {
            var o = await _context.Overtimes.FindAsync(id);
            if (o == null) return Json(new { success = false, message = "Not found" });
            o.Status = "Approved"; o.ApprovedBy = Uid().ToString(); o.ApprovedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Approved!" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var o = await _context.Overtimes.FindAsync(id);
            if (o == null) return Json(new { success = false, message = "Not found" });
            o.Status = "Rejected"; o.ApprovedBy = Uid().ToString(); o.ApprovedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Rejected!" });
        }
    }
}
