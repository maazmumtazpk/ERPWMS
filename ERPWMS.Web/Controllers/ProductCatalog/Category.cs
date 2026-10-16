using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;
using ERPWMS.Database.Models;

namespace ERPWMS.Web.Controllers.ProductCatalog
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context) { _context = context; }

        private int GetUserId()
        {
            var cl = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _context.Categories
                    .Where(c => c.IsDeleted != true)
                    .OrderBy(c => c.CategoryName)
                    .Select(c => new {
                        categoryId = c.CategoryId,
                        categoryName = c.CategoryName,
                        categoryCode = c.CategoryCode,
                        isActive = c.IsActive,
                        createdDate = c.CreatedDate
                    })
                    .ToListAsync();
                return Ok(list);
            }
            catch (Exception ex) { return StatusCode(500, new { error = "Operation failed." }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var total = await _context.Categories.CountAsync(c => c.IsDeleted != true);
                var active = await _context.Categories.CountAsync(c => c.IsActive == true && c.IsDeleted != true);
                var inactive = await _context.Categories.CountAsync(c => c.IsActive != true && c.IsDeleted != true);
                var now = DateTime.Now;
                var som = new DateTime(now.Year, now.Month, 1);
                var addedThisMonth = await _context.Categories.CountAsync(c => c.CreatedDate >= som && c.IsDeleted != true);
                return Json(new { total, active, inactive, addedThisMonth });
            }
            catch { return Json(new { total = 0, active = 0, inactive = 0, addedThisMonth = 0 }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string categoryName, string categoryCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryName))
                    return Json(new { success = false, message = "Category name required." });

                var dup = await _context.Categories.AnyAsync(c => c.CategoryName == categoryName && c.IsDeleted != true);
                if (dup) return Json(new { success = false, message = "Category already exists." });

                _context.Categories.Add(new Category
                {
                    CategoryName = categoryName.Trim(),
                    CategoryCode = string.IsNullOrWhiteSpace(categoryCode) ? null : categoryCode.Trim(),
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = GetUserId().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Category registered!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int categoryId, string categoryName, string categoryCode, bool isActive = true)
        {
            try
            {
                var c = await _context.Categories.FindAsync(categoryId);
                if (c == null) return Json(new { success = false, message = "Not found" });

                c.CategoryName = categoryName.Trim();
                c.CategoryCode = string.IsNullOrWhiteSpace(categoryCode) ? null : categoryCode.Trim();
                c.IsActive = isActive;
                c.UpdatedBy = GetUserId().ToString();
                c.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Category updated!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var c = await _context.Categories.FindAsync(id);
                if (c == null) return Json(new { success = false, message = "Not found" });
                c.IsDeleted = true;
                c.UpdatedBy = GetUserId().ToString();
                c.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var c = await _context.Categories.FindAsync(id);
                if (c == null) return Json(new { success = false, message = "Not found" });
                c.IsActive = !(c.IsActive ?? false);
                c.UpdatedBy = GetUserId().ToString();
                c.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = c.IsActive == true ? "Activated" : "Deactivated" });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }
    }
}

