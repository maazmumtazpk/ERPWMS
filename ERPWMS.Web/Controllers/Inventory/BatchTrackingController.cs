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

namespace ERPWMS.Web.Controllers.Inventory
{
    [Authorize]
    public class BatchTrackingController : Controller
    {
        private readonly AppDbContext _context;

        public BatchTrackingController(AppDbContext c)
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
            var l = await _context.BatchTrackings.Where(b => b.IsActive == true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(b => b.RemainingQty > 0),
                inactive = l.Count(b => b.RemainingQty <= 0),
                addedThisMonth = l.Count(b =>
                    b.CreatedDate.HasValue &&
                    b.CreatedDate.Value.Month == DateTime.Now.Month &&
                    b.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.BatchTrackings
                .Include(b => b.Product)
                .Where(b => b.IsActive == true)
                .OrderBy(b => b.BatchId)
                .Select(b => new
                {
                    batchId = b.BatchId,
                    batchNo = b.BatchNo,
                    productName = b.Product != null ? b.Product.ProductName : "N/A",
                    expiryDate = b.ExpiryDate,
                    quantity = b.Quantity,
                    remainingQty = b.RemainingQty,
                    createdDate = b.CreatedDate
                })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // GET PRODUCTS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var l = await _context.Products
                .Where(p => p.IsActive == true && p.IsDeleted != true)
                .Select(p => new { productId = p.ProductId, productName = p.ProductName })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // ADD / EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult AddBatchPartial() => PartialView("_AddBatchForm");

        [HttpGet]
        public async Task<IActionResult> EditBatchPartial(int id)
        {
            var b = await _context.BatchTrackings.FindAsync(id);
            if (b == null) return NotFound();
            return PartialView("_EditBatchForm", b);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int productId,
            int grnDetailId,
            string batchNo,
            DateOnly expiryDate,
            int quantity)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(batchNo))
                    return Json(new { success = false, message = "Batch No required." });
                if (productId <= 0)
                    return Json(new { success = false, message = "Please select product." });

                _context.BatchTrackings.Add(new BatchTracking
                {
                    ProductId = productId,
                    GrndetailId = grnDetailId,
                    BatchNo = batchNo.Trim(),
                    ExpiryDate = expiryDate,
                    Quantity = quantity,
                    RemainingQty = quantity,
                    IsActive = true,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Batch created!" });
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
        public async Task<IActionResult> Edit(
            int batchId,
            string batchNo,
            DateOnly expiryDate,
            int quantity,
            int remainingQty)
        {
            try
            {
                var b = await _context.BatchTrackings.FindAsync(batchId);
                if (b == null)
                    return Json(new { success = false, message = "Not found" });

                b.BatchNo = batchNo.Trim();
                b.ExpiryDate = expiryDate;
                b.Quantity = quantity;
                b.RemainingQty = remainingQty;
                b.UpdatedBy = Uid().ToString();
                b.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Batch updated!" });
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
                var b = await _context.BatchTrackings.FindAsync(id);
                if (b == null)
                    return Json(new { success = false, message = "Not found" });

                b.IsActive = false;
                b.DeletedBy = Uid().ToString();
                b.DeletedDate = DateTime.Now;
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
