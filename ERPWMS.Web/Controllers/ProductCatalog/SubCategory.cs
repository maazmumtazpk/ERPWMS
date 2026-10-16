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
    public class SubCategoryController : Controller
    {
        private readonly AppDbContext _context;
        public SubCategoryController(AppDbContext context) { _context = context; }

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
                var list = await _context.SubCategories
                    .Where(s => s.IsDeleted != true)
                    .OrderBy(s => s.SubCategoryName)
                    .Select(s => new {
                        subCategoryId = s.SubCategoryId,
                        subCategoryName = s.SubCategoryName,
                        subCategoryCode = s.SubCategoryCode,
                        categoryId = s.CategoryId,
                        description = s.Description,
                        isActive = s.IsActive,
                        createdDate = s.CreatedDate
                    })
                    .ToListAsync();
                return Ok(list);
            }
            catch (Exception ex) { return StatusCode(500, new { error = "Operation failed." }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var list = await _context.Categories
                    .Where(c => c.IsDeleted != true && c.IsActive == true)
                    .OrderBy(c => c.CategoryName)
                    .Select(c => new { categoryId = c.CategoryId, categoryName = c.CategoryName })
                    .ToListAsync();
                return Json(list);
            }
            catch { return Json(new System.Collections.Generic.List<object>()); }
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var total = await _context.SubCategories.CountAsync(s => s.IsDeleted != true);
                var active = await _context.SubCategories.CountAsync(s => s.IsActive == true && s.IsDeleted != true);
                var inactive = await _context.SubCategories.CountAsync(s => s.IsActive != true && s.IsDeleted != true);
                var now = DateTime.Now;
                var som = new DateTime(now.Year, now.Month, 1);
                var addedThisMonth = await _context.SubCategories.CountAsync(s => s.CreatedDate >= som && s.IsDeleted != true);
                return Json(new { total, active, inactive, addedThisMonth });
            }
            catch { return Json(new { total = 0, active = 0, inactive = 0, addedThisMonth = 0 }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int categoryId, string subCategoryName, string subCategoryCode, string description)
        {
            try
            {
                if (categoryId <= 0) return Json(new { success = false, message = "Select category." });
                if (string.IsNullOrWhiteSpace(subCategoryName)) return Json(new { success = false, message = "Sub category name required." });

                var dup = await _context.SubCategories.AnyAsync(s => s.CategoryId == categoryId && s.SubCategoryName == subCategoryName && s.IsDeleted != true);
                if (dup) return Json(new { success = false, message = "Sub category already exists." });

                _context.SubCategories.Add(new SubCategory
                {
                    CategoryId = categoryId,
                    SubCategoryName = subCategoryName.Trim(),
                    SubCategoryCode = string.IsNullOrWhiteSpace(subCategoryCode) ? null : subCategoryCode.Trim(),
                    Description = description ?? "",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = GetUserId().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Sub category registered!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int subCategoryId, string subCategoryName, string subCategoryCode, string description, bool isActive = true)
        {
            try
            {
                var s = await _context.SubCategories.FindAsync(subCategoryId);
                if (s == null) return Json(new { success = false, message = "Not found" });

                s.SubCategoryName = subCategoryName.Trim();
                s.SubCategoryCode = string.IsNullOrWhiteSpace(subCategoryCode) ? null : subCategoryCode.Trim();
                s.Description = description ?? "";
                s.IsActive = isActive;
                s.UpdatedBy = GetUserId().ToString();
                s.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Sub category updated!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var s = await _context.SubCategories.FindAsync(id);
                if (s == null) return Json(new { success = false, message = "Not found" });
                s.IsDeleted = true;
                s.UpdatedBy = GetUserId().ToString();
                s.UpdatedDate = DateTime.Now;
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
                var s = await _context.SubCategories.FindAsync(id);
                if (s == null) return Json(new { success = false, message = "Not found" });
                s.IsActive = !(s.IsActive ?? false);
                s.UpdatedBy = GetUserId().ToString();
                s.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = s.IsActive == true ? "Activated" : "Deactivated" });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }
    }
}

