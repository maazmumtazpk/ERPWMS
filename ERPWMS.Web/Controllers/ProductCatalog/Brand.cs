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
    public class BrandController : Controller
    {
        private readonly AppDbContext _context;

        public BrandController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var cl = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(cl, out var i) && i > 0) return i;
            return 1;
        }

        // -------------------------------------------------------
        // INDEX
        // -------------------------------------------------------
        public IActionResult Index() => View();

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _context.Brands
                    .Where(b => b.IsDeleted != true)
                    .OrderBy(b => b.BrandName)
                    .Select(b => new
                    {
                        brandId = b.BrandId,
                        brandName = b.BrandName,
                        brandCode = b.BrandCode,
                        logoUrl = b.LogoUrl,        // ✅ FIX: LogoUrl (chhota rl)
                        isActive = b.IsActive,
                        createdDate = b.CreatedDate
                    })
                    .ToListAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // -------------------------------------------------------
        // STATISTICS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var total = await _context.Brands.CountAsync(b => b.IsDeleted != true);
                var active = await _context.Brands.CountAsync(b => b.IsActive == true && b.IsDeleted != true);
                var inactive = await _context.Brands.CountAsync(b => b.IsActive != true && b.IsDeleted != true);
                var now = DateTime.Now;
                var som = new DateTime(now.Year, now.Month, 1);
                var addedThisMonth = await _context.Brands.CountAsync(b => b.CreatedDate >= som && b.IsDeleted != true);
                return Json(new { total, active, inactive, addedThisMonth });
            }
            catch
            {
                return Json(new { total = 0, active = 0, inactive = 0, addedThisMonth = 0 });
            }
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string brandName, string brandCode, string logoUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(brandName))
                    return Json(new { success = false, message = "Brand name required." });

                var dup = await _context.Brands.AnyAsync(b => b.BrandName == brandName && b.IsDeleted != true);
                if (dup)
                    return Json(new { success = false, message = "Brand already exists." });

                _context.Brands.Add(new Brand
                {
                    BrandName = brandName.Trim(),
                    BrandCode = string.IsNullOrWhiteSpace(brandCode) ? null : brandCode.Trim(),
                    LogoUrl = string.IsNullOrWhiteSpace(logoUrl) ? null : logoUrl.Trim(),   // ✅ FIX
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = GetUserId().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Brand registered!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // -------------------------------------------------------
        // EDIT
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int brandId, string brandName, string brandCode, string logoUrl, bool isActive = true)
        {
            try
            {
                var b = await _context.Brands.FindAsync(brandId);
                if (b == null)
                    return Json(new { success = false, message = "Not found" });

                b.BrandName = brandName.Trim();
                b.BrandCode = string.IsNullOrWhiteSpace(brandCode) ? null : brandCode.Trim();
                b.LogoUrl = string.IsNullOrWhiteSpace(logoUrl) ? null : logoUrl.Trim();   // ✅ FIX
                b.IsActive = isActive;
                b.UpdatedBy = GetUserId().ToString();
                b.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Brand updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // -------------------------------------------------------
        // DELETE
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var b = await _context.Brands.FindAsync(id);
                if (b == null)
                    return Json(new { success = false, message = "Not found" });

                b.IsDeleted = true;
                b.UpdatedBy = GetUserId().ToString();
                b.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------------------------------------------------------
        // TOGGLE ACTIVE
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var b = await _context.Brands.FindAsync(id);
                if (b == null)
                    return Json(new { success = false, message = "Not found" });

                b.IsActive = !(b.IsActive ?? false);
                b.UpdatedBy = GetUserId().ToString();
                b.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = b.IsActive == true ? "Activated" : "Deactivated" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}