using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Web.Controllers.HRMaster
{
    [Authorize]
    public class WeeklyOffController : Controller
    {
        private readonly AppDbContext _context;
        public WeeklyOffController(AppDbContext c) { _context = c; }

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
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.WeeklyOffs
                .Include(w => w.Employee)
                .Where(w => w.IsDeleted != true)
                .OrderByDescending(w => w.EffectiveFrom)
                .Select(w => new
                {
                    weeklyOffId = w.WeeklyOffId,
                    employeeId = w.EmployeeId,
                    employeeName = w.Employee.FirstName + " " + w.Employee.LastName,
                    employeeCode = w.Employee.EmployeeCode,
                    dayOfWeek = w.DayOfWeek,
                    effectiveFrom = w.EffectiveFrom.ToString("yyyy-MM-dd"),
                    effectiveTo = w.EffectiveTo.HasValue ? w.EffectiveTo.Value.ToString("yyyy-MM-dd") : null,
                    isActive = w.IsActive
                })
                .ToListAsync();
            return Json(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var list = await _context.Employees
                .Where(e => e.IsDeleted != true && e.IsActive == true)
                .Select(e => new {
                    employeeId = e.EmployeeId,
                    employeeCode = e.EmployeeCode,
                    fullName = e.FirstName + " " + e.LastName
                })
                .OrderBy(e => e.fullName)
                .ToListAsync();
            return Json(list);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Set(int employeeId, string dayOfWeek, string? remarks)
        {
            try
            {
                if (employeeId <= 0)
                    return Json(new { success = false, message = "Employee required." });

                if (string.IsNullOrWhiteSpace(dayOfWeek))
                    return Json(new { success = false, message = "Day required." });

                var emp = await _context.Employees.FindAsync(employeeId);
                if (emp == null)
                    return Json(new { success = false, message = "Employee not found." });

                // Close current active
                var current = await _context.WeeklyOffs
                    .Where(w => w.EmployeeId == employeeId && w.IsActive == true && w.IsDeleted != true)
                    .ToListAsync();

                foreach (var cur in current)
                {
                    cur.IsActive = false;
                    cur.EffectiveTo = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
                    cur.UpdatedBy = Uid().ToString();
                    cur.UpdatedDate = DateTime.Now;
                }

                _context.WeeklyOffs.Add(new WeeklyOff
                {
                    EmployeeId = employeeId,
                    DayOfWeek = dayOfWeek,
                    EffectiveFrom = DateOnly.FromDateTime(DateTime.Today),
                    Remarks = remarks,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Weekly off set!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var w = await _context.WeeklyOffs.FindAsync(id);
                if (w == null) return Json(new { success = false, message = "Not found" });

                w.IsDeleted = true;
                w.IsActive = false;
                w.DeletedBy = Uid().ToString();
                w.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
