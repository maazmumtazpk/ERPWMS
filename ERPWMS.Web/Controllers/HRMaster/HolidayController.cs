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
    public class HolidayController : Controller
    {
        private readonly AppDbContext _context;
        public HolidayController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var year = DateTime.Now.Year;
            var l = await _context.Holidays
                .Where(h => h.IsDeleted != true && h.HolidayDate.Year == year)
                .ToListAsync();

            return Json(new
            {
                total = l.Count,
                paid = l.Count(h => h.IsPaid == true),
                upcoming = l.Count(h => h.HolidayDate >= DateOnly.FromDateTime(DateTime.Now)),
                thisYear = year
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int? year)
        {
            var y = year ?? DateTime.Now.Year;
            var l = await _context.Holidays
                .Where(h => h.IsDeleted != true && h.HolidayDate.Year == y)
                .OrderBy(h => h.HolidayDate)
                .Select(h => new
                {
                    holidayId = h.HolidayId,
                    holidayName = h.HolidayName,
                    holidayDate = h.HolidayDate.ToString("yyyy-MM-dd"),
                    holidayDateDisplay = h.HolidayDate.ToString("dd-MMM-yyyy"),
                    dayOfWeek = h.HolidayDate.DayOfWeek.ToString(),
                    holidayType = h.HolidayType,
                    scope = h.Scope,
                    isPaid = h.IsPaid,
                    isRecurring = h.IsRecurring,
                    isActive = h.IsActive
                })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var h = await _context.Holidays.FindAsync(id);
            if (h == null) return NotFound();
            return Json(new
            {
                holidayId = h.HolidayId,
                holidayName = h.HolidayName,
                holidayDate = h.HolidayDate.ToString("yyyy-MM-dd"),
                holidayType = h.HolidayType,
                scope = h.Scope,
                isPaid = h.IsPaid,
                isRecurring = h.IsRecurring,
                description = h.Description,
                isActive = h.IsActive
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Save(
            int holidayId,
            string holidayName,
            string holidayDate,
            string holidayType,
            string scope,
            bool isPaid,
            bool isRecurring,
            string? description,
            bool isActive)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(holidayName))
                    return Json(new { success = false, message = "Holiday name required." });

                if (!DateOnly.TryParse(holidayDate, out var date))
                    return Json(new { success = false, message = "Invalid date." });

                var exists = await _context.Holidays.AnyAsync(h =>
                    h.HolidayDate == date &&
                    h.HolidayName == holidayName.Trim() &&
                    h.HolidayId != holidayId &&
                    h.IsDeleted != true);

                if (exists)
                    return Json(new { success = false, message = "This holiday already exists on this date." });

                Holiday entity;
                if (holidayId > 0)
                {
                    entity = await _context.Holidays.FindAsync(holidayId);
                    if (entity == null)
                        return Json(new { success = false, message = "Holiday not found." });

                    entity.HolidayName = holidayName.Trim();
                    entity.HolidayDate = date;
                    entity.HolidayType = holidayType ?? "Public";
                    entity.Scope = scope ?? "All";
                    entity.IsPaid = isPaid;
                    entity.IsRecurring = isRecurring;
                    entity.Description = description;
                    entity.IsActive = isActive;
                    entity.UpdatedBy = Uid().ToString();
                    entity.UpdatedDate = DateTime.Now;
                }
                else
                {
                    entity = new Holiday
                    {
                        HolidayName = holidayName.Trim(),
                        HolidayDate = date,
                        HolidayType = holidayType ?? "Public",
                        Scope = scope ?? "All",
                        IsPaid = isPaid,
                        IsRecurring = isRecurring,
                        Description = description,
                        IsActive = isActive,
                        IsDeleted = false,
                        CreatedBy = Uid().ToString(),
                        CreatedDate = DateTime.Now
                    };
                    _context.Holidays.Add(entity);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = holidayId > 0 ? "Holiday updated!" : "Holiday added!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var h = await _context.Holidays.FindAsync(id);
                if (h == null) return Json(new { success = false, message = "Not found" });

                h.IsDeleted = true;
                h.IsActive = false;
                h.DeletedBy = Uid().ToString();
                h.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }
    }
}



