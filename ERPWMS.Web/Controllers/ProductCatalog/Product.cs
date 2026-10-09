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
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
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
                var list = await _context.Products
                    .Where(p => p.IsDeleted != true)
                    .OrderBy(p => p.ProductName)
                    .Select(p => new
                    {
                        productId = p.ProductId,
                        productName = p.ProductName,
                        sku = p.Sku,            // ✅ FIX: Sku
                        barcode = p.Barcode,
                        categoryId = p.CategoryId,
                        isActive = p.IsActive,
                        createdDate = p.CreatedDate
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
        // GET DROPDOWNS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetDropdowns()
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.IsDeleted != true && c.IsActive == true)
                    .Select(c => new { categoryId = c.CategoryId, categoryName = c.CategoryName })
                    .ToListAsync();

                var brands = await _context.Brands
                    .Where(b => b.IsDeleted != true && b.IsActive == true)
                    .Select(b => new { brandId = b.BrandId, brandName = b.BrandName })
                    .ToListAsync();

                var units = await _context.Units
                    .Where(u => u.IsDeleted != true && u.IsActive == true)
                    .Select(u => new { unitId = u.UnitId, unitName = u.UnitName })
                    .ToListAsync();

                return Json(new { categories, brands, units });
            }
            catch
            {
                return Json(new
                {
                    categories = Array.Empty<object>(),
                    brands = Array.Empty<object>(),
                    units = Array.Empty<object>()
                });
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
                var total = await _context.Products.CountAsync(p => p.IsDeleted != true);
                var active = await _context.Products.CountAsync(p => p.IsActive == true && p.IsDeleted != true);
                var inactive = await _context.Products.CountAsync(p => p.IsActive != true && p.IsDeleted != true);
                var now = DateTime.Now;
                var som = new DateTime(now.Year, now.Month, 1);
                var addedThisMonth = await _context.Products.CountAsync(p => p.CreatedDate >= som && p.IsDeleted != true);
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
        public async Task<IActionResult> Register(
            string productName,
            string sku,
            string barcode,
            int categoryId,
            int? subCategoryId,
            int? brandId,
            int? unitId,
            string description,
            decimal? weight,
            decimal? length,
            decimal? width,
            decimal? height,
            int? minStockLevel,
            int? maxStockLevel,
            int? reorderLevel,
            bool isSerialized = false,
            bool isBatchManaged = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productName))
                    return Json(new { success = false, message = "Product name required." });
                if (categoryId <= 0)
                    return Json(new { success = false, message = "Select a category." });

                if (!string.IsNullOrWhiteSpace(sku))
                {
                    var dup = await _context.Products.AnyAsync(p => p.Sku == sku && p.IsDeleted != true);   // ✅ FIX
                    if (dup)
                        return Json(new { success = false, message = "SKU already exists." });
                }

                _context.Products.Add(new Product
                {
                    ProductName = productName.Trim(),
                    Sku = sku ?? "",                     // ✅ FIX
                    Barcode = barcode,
                    Description = description,
                    CategoryId = categoryId,
                    SubCategoryId = subCategoryId ?? 0,
                    BrandId = brandId ?? 0,
                    UnitId = unitId ?? 0,
                    Weight = weight,
                    Length = length,
                    Width = width,
                    Height = height,
                    MinStockLevel = minStockLevel,
                    MaxStockLevel = maxStockLevel,
                    ReorderLevel = reorderLevel,
                    IsSerialized = isSerialized,
                    IsBatchManaged = isBatchManaged,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = GetUserId().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Product registered!" });
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
        public async Task<IActionResult> Edit(
            int productId,
            string productName,
            string sku,
            string barcode,
            int categoryId,
            int? subCategoryId,
            int? brandId,
            int? unitId,
            string description,
            decimal? weight,
            decimal? length,
            decimal? width,
            decimal? height,
            int? minStockLevel,
            int? maxStockLevel,
            int? reorderLevel,
            bool isSerialized = false,
            bool isBatchManaged = false,
            bool isActive = true)
        {
            try
            {
                var p = await _context.Products.FindAsync(productId);
                if (p == null)
                    return Json(new { success = false, message = "Not found" });

                p.ProductName = productName.Trim();
                p.Sku = sku ?? "";                   // ✅ FIX
                p.Barcode = barcode;
                p.Description = description;
                p.CategoryId = categoryId;
                p.SubCategoryId = subCategoryId ?? 0;
                p.BrandId = brandId ?? 0;
                p.UnitId = unitId ?? 0;
                p.Weight = weight;
                p.Length = length;
                p.Width = width;
                p.Height = height;
                p.MinStockLevel = minStockLevel;
                p.MaxStockLevel = maxStockLevel;
                p.ReorderLevel = reorderLevel;
                p.IsSerialized = isSerialized;
                p.IsBatchManaged = isBatchManaged;
                p.IsActive = isActive;
                p.UpdatedBy = GetUserId().ToString();
                p.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Product updated!" });
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
                var p = await _context.Products.FindAsync(id);
                if (p == null)
                    return Json(new { success = false, message = "Not found" });

                p.IsDeleted = true;
                p.UpdatedBy = GetUserId().ToString();
                p.UpdatedDate = DateTime.Now;
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
                var p = await _context.Products.FindAsync(id);
                if (p == null)
                    return Json(new { success = false, message = "Not found" });

                p.IsActive = !(p.IsActive ?? false);
                p.UpdatedBy = GetUserId().ToString();
                p.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = p.IsActive == true ? "Activated" : "Deactivated" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}