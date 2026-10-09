using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;
using ERPWMS.Database.Models;

namespace ERPWMS.Web.Controllers.ProductCatalog
{
    [Authorize]
    public class UnitController : Controller
    {
        private readonly AppDbContext _context;
        public UnitController(AppDbContext context) { _context = context; }

        private int GetUserId()
        {
            var cl = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(cl, out var i) && i > 0) return i;
            return 1;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _context.Units
                    .Where(u => u.IsDeleted != true)
                    .OrderBy(u => u.UnitName)
                    .Select(u => new {
                        unitId = u.UnitId,
                        unitName = u.UnitName,
                        unitCode = u.UnitCode,
                        conversionFactor = u.ConversionFactor,
                        isBaseUnit = u.IsBaseUnit,
                        isActive = u.IsActive,
                        createdDate = u.CreatedDate
                    })
                    .ToListAsync();
                return Ok(list);
            }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var total = await _context.Units.CountAsync(u => u.IsDeleted != true);
                var active = await _context.Units.CountAsync(u => u.IsActive == true && u.IsDeleted != true);
                var inactive = await _context.Units.CountAsync(u => u.IsActive != true && u.IsDeleted != true);
                var now = DateTime.Now;
                var som = new DateTime(now.Year, now.Month, 1);
                var addedThisMonth = await _context.Units.CountAsync(u => u.CreatedDate >= som && u.IsDeleted != true);
                return Json(new { total, active, inactive, addedThisMonth });
            }
            catch { return Json(new { total = 0, active = 0, inactive = 0, addedThisMonth = 0 }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string unitName, string unitCode, decimal? conversionFactor, bool isBaseUnit = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(unitName))
                    return Json(new { success = false, message = "Unit name required." });

                var dup = await _context.Units.AnyAsync(u => u.UnitName == unitName && u.IsDeleted != true);
                if (dup) return Json(new { success = false, message = "Unit already exists." });

                _context.Units.Add(new Unit
                {
                    UnitName = unitName.Trim(),
                    UnitCode = string.IsNullOrWhiteSpace(unitCode) ? null : unitCode.Trim(),
                    ConversionFactor = conversionFactor,
                    IsBaseUnit = isBaseUnit,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = GetUserId().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Unit registered!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int unitId, string unitName, string unitCode, decimal? conversionFactor, bool isBaseUnit = false, bool isActive = true)
        {
            try
            {
                var u = await _context.Units.FindAsync(unitId);
                if (u == null) return Json(new { success = false, message = "Not found" });

                u.UnitName = unitName.Trim();
                u.UnitCode = string.IsNullOrWhiteSpace(unitCode) ? null : unitCode.Trim();
                u.ConversionFactor = conversionFactor;
                u.IsBaseUnit = isBaseUnit;
                u.IsActive = isActive;
                u.UpdatedBy = GetUserId().ToString();
                u.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Unit updated!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var u = await _context.Units.FindAsync(id);
                if (u == null) return Json(new { success = false, message = "Not found" });
                u.IsDeleted = true;
                u.UpdatedBy = GetUserId().ToString();
                u.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var u = await _context.Units.FindAsync(id);
                if (u == null) return Json(new { success = false, message = "Not found" });
                u.IsActive = !(u.IsActive ?? false);
                u.UpdatedBy = GetUserId().ToString();
                u.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = u.IsActive == true ? "Activated" : "Deactivated" });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }
    }
}