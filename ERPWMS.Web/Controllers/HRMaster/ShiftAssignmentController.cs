using ERPWMS.Database;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPWMS.Web.Controllers.HRMaster
{
    [Authorize]
    public class ShiftAssignmentController : Controller
    {
        private readonly AppDbContext _context;
        public ShiftAssignmentController(AppDbContext c) { _context = c; }

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
            var list = await _context.ShiftAssignments
                .Include(a => a.Employee)
                .Include(a => a.Shift)
                .Where(a => a.IsDeleted != true)
                .OrderByDescending(a => a.EffectiveFrom)
                .Select(a => new {
                    assignmentId = a.ShiftAssignmentId,
                    employeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                    employeeCode = a.Employee.EmployeeCode,
                    shiftName = a.Shift.ShiftName,
                    effectiveFrom = a.EffectiveFrom.ToString("yyyy-MM-dd"),
                    effectiveTo = a.EffectiveTo.HasValue ? a.EffectiveTo.Value.ToString("yyyy-MM-dd") : null,
                    isActive = a.IsActive
                }).ToListAsync();
            return Json(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetDropdowns()
        {
            var emps = await _context.Employees
                .Where(e => e.IsDeleted != true && e.IsActive == true)
                .Select(e => new { id = e.EmployeeId, name = e.EmployeeCode + " - " + e.FirstName + " " + e.LastName })
                .OrderBy(e => e.name).ToListAsync();

            var shifts = await _context.Shifts
                .Where(s => s.IsDeleted != true)
                .Select(s => new { id = s.ShiftId, name = s.ShiftName })
                .OrderBy(s => s.name).ToListAsync();

            return Json(new { employees = emps, shifts = shifts });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int employeeId, int shiftId, string? remarks)
        {
            try
            {
                if (employeeId <= 0 || shiftId <= 0)
                    return Json(new { success = false, message = "Employee and Shift required." });

                var current = await _context.ShiftAssignments
                    .Where(a => a.EmployeeId == employeeId && a.IsActive == true && a.IsDeleted != true)
                    .ToListAsync();
                foreach (var c in current) {
                    c.IsActive = false;
                    c.EffectiveTo = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
                    c.UpdatedBy = Uid().ToString();
                    c.UpdatedDate = DateTime.Now;
                }

                _context.ShiftAssignments.Add(new ShiftAssignment {
                    EmployeeId = employeeId,
                    ShiftId = shiftId,
                    EffectiveFrom = DateOnly.FromDateTime(DateTime.Today),
                    Remarks = remarks,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });

                // Update Employee.ShiftId as well
                var emp = await _context.Employees.FindAsync(employeeId);
                if (emp != null) emp.ShiftId = shiftId;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Shift assigned!" });
            }
            catch (Exception ex) {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await _context.ShiftAssignments.FindAsync(id);
            if (a == null) return Json(new { success = false, message = "Not found" });
            a.IsDeleted = true; a.IsActive = false;
            a.UpdatedBy = Uid().ToString(); a.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }
    }
}
