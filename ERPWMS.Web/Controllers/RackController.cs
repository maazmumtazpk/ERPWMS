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
    public class RackController : Controller
    {
        private readonly AppDbContext _context;

        public RackController(AppDbContext c)
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

            var l = await _context.Racks
                .Include(r => r.Zone)
                .Where(r => r.Zone.WarehouseId == warehouseId && r.IsDeleted != true)
                .ToListAsync();

            return Json(new
            {
                total = l.Count,
                active = l.Count(r => r.IsActive == true),
                inactive = l.Count(r => r.IsActive != true),
                addedThisMonth = l.Count(r =>
                    r.CreatedDate.HasValue &&
                    r.CreatedDate.Value.Month == DateTime.Now.Month &&
                    r.CreatedDate.Value.Year == DateTime.Now.Year)
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

            var l = await _context.Racks
                .Include(r => r.Zone)
                .Where(r => r.Zone.WarehouseId == warehouseId && r.IsDeleted != true)
                .OrderBy(r => r.RackId)
                .Select(r => new
                {
                    rackId = r.RackId,
                    rackName = r.RackName,
                    rackCode = r.RackCode ?? "",
                    zoneName = r.Zone != null ? r.Zone.ZoneName : "-",
                    isActive = r.IsActive ?? false,
                    createdDate = r.CreatedDate
                })
                .ToListAsync();
            return Json(l);
        }

        // ============================================================
        // GET FOR ZONE
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetForZone(int zoneId)
        {
            var l = await _context.Racks
                .Where(r => r.ZoneId == zoneId && r.IsActive == true && r.IsDeleted != true)
                .Select(r => new { rackId = r.RackId, rackName = r.RackName })
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
            var x = await _context.Racks
                .Include(r => r.Zone)
                .FirstOrDefaultAsync(r => r.RackId == id);
            if (x == null) return NotFound();
            return PartialView("_EditForm", x);
        }

        // ============================================================
        // REGISTER
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int zoneId, string rackName, string? rackCode)
        {
            if (string.IsNullOrWhiteSpace(rackName))
                return Json(new { success = false, message = "Rack name required." });
            if (zoneId <= 0)
                return Json(new { success = false, message = "Please select a zone." });

            if (await _context.Racks.AnyAsync(r => r.ZoneId == zoneId && r.RackName == rackName && r.IsDeleted != true))
                return Json(new { success = false, message = "Rack already exists." });

            try
            {
                _context.Racks.Add(new Rack
                {
                    ZoneId = zoneId,
                    RackName = rackName.Trim(),
                    RackCode = string.IsNullOrWhiteSpace(rackCode) ? null : rackCode.Trim(),
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Rack registered!" });
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
        public async Task<IActionResult> Edit(int rackId, string rackName, string? rackCode, bool isActive = true)
        {
            var x = await _context.Racks.FindAsync(rackId);
            if (x == null)
                return Json(new { success = false, message = "Not found" });

            try
            {
                x.RackName = rackName.Trim();
                x.RackCode = string.IsNullOrWhiteSpace(rackCode) ? null : rackCode.Trim();
                x.IsActive = isActive;
                x.UpdatedBy = Uid().ToString();
                x.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Rack updated!" });
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
            var x = await _context.Racks.FindAsync(id);
            if (x == null)
                return Json(new { success = false, message = "Not found" });

            try
            {
                x.IsDeleted = true;
                x.IsActive = false;
                x.DeletedBy = Uid().ToString();
                x.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Rack deleted." });
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
            var x = await _context.Racks.FindAsync(id);
            if (x == null)
                return Json(new { success = false, message = "Not found" });

            x.IsActive = !(x.IsActive ?? false);
            x.UpdatedBy = Uid().ToString();
            x.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = x.IsActive == true ? "Activated." : "Deactivated." });
        }
    }
}
