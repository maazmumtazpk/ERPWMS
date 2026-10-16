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
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }

        private const decimal MaxOTPerDay = 12m;
        private const decimal MaxOTPerMonth = 100m;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.Overtimes.Include(o => o.Employee)
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
        public async Task<IActionResult> GetEmployees() =>
            Json(await _context.Employees.Where(e => e.IsDeleted != true && e.IsActive == true)
                .Select(e => new {
                    employeeId = e.EmployeeId,
                    employeeCode = e.EmployeeCode,
                    fullName = e.FirstName + " " + e.LastName
                }).OrderBy(e => e.fullName).ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Edit")]
        public async Task<IActionResult> Record(int employeeId, string overtimeDate, decimal otHours,
            bool isHolidayWork, bool isWeeklyOffWork, string? remarks)
        {
            try
            {
                if (employeeId <= 0) return Json(new { success = false, message = "Employee required." });
                if (!DateOnly.TryParse(overtimeDate, out var date)) return Json(new { success = false, message = "Invalid date." });

                if (date > DateOnly.FromDateTime(DateTime.Today))
                    return Json(new { success = false, message = "Future date not allowed." });

                if (otHours <= 0 || otHours > MaxOTPerDay)
                    return Json(new { success = false, message = $"OT hours must be 0-{MaxOTPerDay}." });

                var dup = await _context.Overtimes.AnyAsync(o => o.EmployeeId == employeeId
                    && o.OvertimeDate == date && o.Status != "Rejected" && o.IsDeleted != true);
                if (dup)
                    return Json(new { success = false, message = "OT already recorded for this date." });

                var monthStart = new DateOnly(date.Year, date.Month, 1);
                var monthEnd = new DateOnly(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

                decimal monthTotal = await _context.Overtimes
                    .Where(o => o.EmployeeId == employeeId
                        && o.OvertimeDate >= monthStart && o.OvertimeDate <= monthEnd
                        && o.Status != "Rejected" && o.IsDeleted != true)
                    .SumAsync(o => (decimal?)o.OTHours) ?? 0;

                if (monthTotal + otHours > MaxOTPerMonth)
                    return Json(new { success = false, message = $"Monthly OT limit exceeded. Used: {monthTotal}h." });

                var calc = await _calc.CalculateAsync(employeeId, otHours, isHolidayWork, isWeeklyOffWork);

                _context.Overtimes.Add(new Overtime
                {
                    EmployeeId = employeeId,
                    OvertimeDate = date,
                    OTHours = otHours,
                    OTRate = calc.OTRate,
                    OTMultiplier = calc.OTMultiplier,
                    OTAmount = calc.OTAmount,
                    IsHolidayWork = isHolidayWork,
                    IsWeeklyOffWork = isWeeklyOffWork,
                    Status = "Pending",
                    Remarks = remarks,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"OT recorded! Amount: {calc.OTAmount:N2}" });
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
            var o = await _context.Overtimes.FindAsync(id);
            if (o == null) return Json(new { success = false, message = "Not found" });
            if (o.Status != "Pending") return Json(new { success = false, message = "Only pending can be approved." });

            var currentUid = Uid().ToString();
            if (o.CreatedBy == currentUid)
                return Json(new { success = false, message = "You cannot approve your own OT entry." });

            o.Status = "Approved";
            o.ApprovedBy = currentUid;
            o.ApprovedDate = DateTime.Now;
            o.UpdatedBy = currentUid;
            o.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Approved!" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Approve")]
        public async Task<IActionResult> Reject(int id)
        {
            var o = await _context.Overtimes.FindAsync(id);
            if (o == null) return Json(new { success = false, message = "Not found" });
            if (o.Status != "Pending") return Json(new { success = false, message = "Only pending can be rejected." });

            o.Status = "Rejected";
            o.ApprovedBy = Uid().ToString();
            o.ApprovedDate = DateTime.Now;
            o.UpdatedBy = Uid().ToString();
            o.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Rejected!" });
        }
    }
}
