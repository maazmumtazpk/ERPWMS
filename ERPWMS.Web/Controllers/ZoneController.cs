using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Web.Controllers
{
    [Authorize]
    public class ZoneController : Controller
    {
        private readonly AppDbContext _context;

        public ZoneController(AppDbContext c)
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

        // ============================================================
        // INDEX
        // ============================================================
        [HttpGet]
        public IActionResult Index() => View();

        // ============================================================
        // STATISTICS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetStatistics(int warehouseId)
        {
            if (warehouseId <= 0)
                return Json(new { total = 0, active = 0, inactive = 0, addedThisMonth = 0 });

            var l = await _context.Zones
                .Where(z => z.WarehouseId == warehouseId && z.IsDeleted != true)
                .ToListAsync();

            return Json(new
            {
                total = l.Count,
                active = l.Count(z => z.IsActive == true),
                inactive = l.Count(z => z.IsActive != true),
                addedThisMonth = l.Count(z =>
                    z.CreatedDate.HasValue &&
                    z.CreatedDate.Value.Month == DateTime.Now.Month &&
                    z.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // ============================================================
        // GET ALL
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll(int warehouseId)
        {
            if (warehouseId <= 0)
                return Json(new List<object>());

            var l = await _context.Zones
                .Where(z => z.WarehouseId == warehouseId && z.IsDeleted != true)
                .OrderBy(z => z.ZoneId)
                .Select(z => new
                {
                    zoneId = z.ZoneId,
                    zoneName = z.ZoneName,
                    zoneCode = z.ZoneCode ?? "",
                    isActive = z.IsActive ?? false,
                    createdDate = z.CreatedDate
                })
                .ToListAsync();
            return Json(l);
        }

        // ============================================================
        // GET FOR DROPDOWN
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetForDropdown(int warehouseId)
        {
            var l = await _context.Zones
                .Where(z => z.WarehouseId == warehouseId && z.IsActive == true && z.IsDeleted != true)
                .Select(z => new { zoneId = z.ZoneId, zoneName = z.ZoneName })
                .ToListAsync();
            return Json(l);
        }

        // ============================================================
        // ADD / EDIT PARTIALS
        // ============================================================
        [HttpGet]
        public IActionResult AddPartial(int warehouseId)
        {
            ViewBag.WarehouseId = warehouseId;
            return PartialView("_AddForm");
        }

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var z = await _context.Zones.FindAsync(id);
            if (z == null) return NotFound();
            return PartialView("_EditForm", z);
        }

        // ============================================================
        // REGISTER
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int warehouseId, string zoneName, string? zoneCode)
        {
            if (string.IsNullOrWhiteSpace(zoneName))
                return Json(new { success = false, message = "Zone name required." });
            if (warehouseId <= 0)
                return Json(new { success = false, message = "Invalid warehouse." });

            if (await _context.Zones.AnyAsync(z =>
                z.WarehouseId == warehouseId &&
                z.ZoneName == zoneName &&
                z.IsDeleted != true))
                return Json(new { success = false, message = "Zone already exists." });

            try
            {
                _context.Zones.Add(new Zone
                {
                    WarehouseId = warehouseId,
                    ZoneName = zoneName.Trim(),
                    ZoneCode = string.IsNullOrWhiteSpace(zoneCode) ? null : zoneCode.Trim(),
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Zone registered!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // ============================================================
        // EDIT
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int zoneId, string zoneName, string? zoneCode, bool isActive = true)
        {
            var z = await _context.Zones.FindAsync(zoneId);
            if (z == null)
                return Json(new { success = false, message = "Not found" });

            try
            {
                z.ZoneName = zoneName.Trim();
                z.ZoneCode = string.IsNullOrWhiteSpace(zoneCode) ? null : zoneCode.Trim();
                z.IsActive = isActive;
                z.UpdatedBy = Uid().ToString();
                z.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Zone updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // ============================================================
        // DELETE
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            var z = await _context.Zones.FindAsync(id);
            if (z == null)
                return Json(new { success = false, message = "Not found" });

            try
            {
                z.IsDeleted = true;
                z.IsActive = false;
                z.DeletedBy = Uid().ToString();
                z.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Zone deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // ============================================================
        // TOGGLE ACTIVE
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Toggle(int id)
        {
            var z = await _context.Zones.FindAsync(id);
            if (z == null)
                return Json(new { success = false, message = "Not found" });

            z.IsActive = !(z.IsActive ?? false);
            z.UpdatedBy = Uid().ToString();
            z.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = z.IsActive == true ? "Activated." : "Deactivated." });
        }
    }
}
