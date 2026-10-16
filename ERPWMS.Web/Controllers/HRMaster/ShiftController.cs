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
    public class ShiftController : Controller
    {
        private readonly AppDbContext _context;

        public ShiftController(AppDbContext c)
        {
            _context = c;
        }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }

        // -------------------------------------------------------
        // INDEX
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult Index() => View();

        // -------------------------------------------------------
        // STATISTICS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var l = await _context.Shifts.Where(s => s.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(s => s.IsActive == true),
                inactive = l.Count(s => s.IsActive != true),
                addedThisMonth = l.Count(s =>
                    s.CreatedDate.HasValue &&
                    s.CreatedDate.Value.Month == DateTime.Now.Month &&
                    s.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.Shifts
                .Where(s => s.IsDeleted != true)
                .OrderBy(s => s.ShiftId)
                .Select(s => new
                {
                    shiftId = s.ShiftId,
                    shiftName = s.ShiftName,
                    startTime = s.StartTime.ToString("HH:mm"),
                    endTime = s.EndTime.ToString("HH:mm"),
                    gracePeriodMinutes = s.GracePeriodMinutes,   // âœ… int? as-is
                    isActive = s.IsActive == true
                })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // ADD / EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult AddPartial() => PartialView("_AddForm");

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var s = await _context.Shifts.FindAsync(id);
            if (s == null) return NotFound();
            return PartialView("_EditForm", s);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Register(
            string shiftName,
            string startTime,
            string endTime,
            int? gracePeriodMinutes,
            bool isActive = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(shiftName))
                    return Json(new { success = false, message = "Shift name required." });

                var exists = await _context.Shifts.AnyAsync(s => s.ShiftName == shiftName.Trim() && s.IsDeleted != true);
                if (exists)
                    return Json(new { success = false, message = "A shift with this name already exists. Duplicate shifts not allowed." });

                if (!TimeOnly.TryParse(startTime, out var st))
                    return Json(new { success = false, message = "Invalid start time." });
                if (!TimeOnly.TryParse(endTime, out var et))
                    return Json(new { success = false, message = "Invalid end time." });
                if (st >= et)
                    return Json(new { success = false, message = "End time must be after start time." });

                _context.Shifts.Add(new Shift
                {
                    ShiftName = shiftName.Trim(),
                    StartTime = st,
                    EndTime = et,
                    GracePeriodMinutes = gracePeriodMinutes,   // âœ… int?
                    IsActive = isActive,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Shift created!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // -------------------------------------------------------
        // EDIT
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Edit(
            int shiftId,
            string shiftName,
            string startTime,
            string endTime,
            int? gracePeriodMinutes,
            bool isActive = true)
        {
            try
            {
                var s = await _context.Shifts.FindAsync(shiftId);
                if (s == null)
                    return Json(new { success = false, message = "Not found" });

                if (string.IsNullOrWhiteSpace(shiftName))
                    return Json(new { success = false, message = "Shift name required." });

                var exists = await _context.Shifts.AnyAsync(x =>
                    x.ShiftId != shiftId && x.ShiftName == shiftName.Trim() && x.IsDeleted != true);
                if (exists)
                    return Json(new { success = false, message = "Another shift with this name already exists." });

                if (!TimeOnly.TryParse(startTime, out var st))
                    return Json(new { success = false, message = "Invalid start time." });
                if (!TimeOnly.TryParse(endTime, out var et))
                    return Json(new { success = false, message = "Invalid end time." });
                if (st >= et)
                    return Json(new { success = false, message = "End time must be after start time." });

                s.ShiftName = shiftName.Trim();
                s.StartTime = st;
                s.EndTime = et;
                s.GracePeriodMinutes = gracePeriodMinutes;   // âœ… int?
                s.IsActive = isActive;
                s.UpdatedBy = Uid().ToString();
                s.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Shift updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // -------------------------------------------------------
        // DELETE
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var s = await _context.Shifts.FindAsync(id);
                if (s == null)
                    return Json(new { success = false, message = "Not found" });

                var inUse = await _context.Employees.AnyAsync(e => e.ShiftId == id && e.IsDeleted != true);
                if (inUse)
                    return Json(new { success = false, message = "Cannot delete: employees are assigned to this shift." });

                s.IsDeleted = true;
                s.IsActive = false;
                s.DeletedBy = Uid().ToString();
                s.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // -------------------------------------------------------
        // TOGGLE ACTIVE
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var s = await _context.Shifts.FindAsync(id);
                if (s == null)
                    return Json(new { success = false, message = "Not found" });

                s.IsActive = !(s.IsActive ?? false);
                s.UpdatedBy = Uid().ToString();
                s.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = s.IsActive == true ? "Activated" : "Deactivated" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }
    }
}


