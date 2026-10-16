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
    public class AdjustmentController : Controller
    {
        private readonly AppDbContext _context;

        public AdjustmentController(AppDbContext c)
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
            var l = await _context.StockAdjustments.Where(a => a.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(a => a.Status == "Approved"),
                inactive = l.Count(a => a.Status == "Pending"),
                addedThisMonth = l.Count(a =>
                    a.CreatedDate.HasValue &&
                    a.CreatedDate.Value.Month == DateTime.Now.Month &&
                    a.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.StockAdjustments
                .Include(a => a.Product)
                .Include(a => a.Warehouse)
                .Where(a => a.IsDeleted != true)
                .OrderBy(a => a.AdjustmentId)
                .Select(a => new
                {
                    adjustmentId = a.AdjustmentId,
                    adjustmentType = a.AdjustmentType,
                    productName = a.Product != null ? a.Product.ProductName : "N/A",
                    warehouseName = a.Warehouse != null ? a.Warehouse.WarehouseName : "N/A",
                    quantity = a.Quantity,
                    previousQty = a.PreviousQty,
                    newQty = a.NewQty,
                    reason = a.Reason,
                    status = a.Status ?? "Pending",
                    createdDate = a.CreatedDate
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
        // GET WAREHOUSES
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetWarehouses()
        {
            var l = await _context.Warehouses
                .Where(w => w.IsActive == true && w.IsDeleted != true)
                .Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // ADD / EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult AddAdjustmentPartial() => PartialView("_AddAdjustmentForm");

        [HttpGet]
        public async Task<IActionResult> EditAdjustmentPartial(int id)
        {
            var a = await _context.StockAdjustments.FindAsync(id);
            if (a == null) return NotFound();
            return PartialView("_EditAdjustmentForm", a);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int productId,
            int warehouseId,
            int binId,
            string? adjustmentType,
            int quantity,
            int previousQty,
            int newQty,
            string? reason)
        {
            try
            {
                if (productId <= 0)
                    return Json(new { success = false, message = "Please select product." });
                if (warehouseId <= 0)
                    return Json(new { success = false, message = "Please select warehouse." });

                _context.StockAdjustments.Add(new StockAdjustment
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    BinId = binId,
                    AdjustmentType = adjustmentType ?? "Increase",
                    Quantity = quantity,
                    PreviousQty = previousQty,
                    NewQty = newQty,
                    Reason = reason ?? "",
                    Status = "Pending",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Adjustment created!" });
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
            int adjustmentId,
            string? adjustmentType,
            int quantity,
            int previousQty,
            int newQty,
            string? reason,
            string? status)
        {
            try
            {
                var a = await _context.StockAdjustments.FindAsync(adjustmentId);
                if (a == null)
                    return Json(new { success = false, message = "Not found" });

                a.AdjustmentType = adjustmentType;
                a.Quantity = quantity;
                a.PreviousQty = previousQty;
                a.NewQty = newQty;
                a.Reason = reason;
                a.Status = status;
                a.UpdatedBy = Uid().ToString();
                a.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Adjustment updated!" });
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
                var a = await _context.StockAdjustments.FindAsync(id);
                if (a == null)
                    return Json(new { success = false, message = "Not found" });

                a.IsDeleted = true;
                a.IsActive = false;
                a.DeletedBy = Uid().ToString();
                a.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // -------------------------------------------------------
        // TOGGLE STATUS (Pending <-> Approved)
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var a = await _context.StockAdjustments.FindAsync(id);
                if (a == null)
                    return Json(new { success = false, message = "Not found" });

                var currentUid = Uid().ToString();
                if (a.CreatedBy == currentUid)
                    return Json(new { success = false, message = "You cannot approve your own entry." });
                a.Status = a.Status == "Pending" ? "Approved" : "Pending";
                a.UpdatedBy = Uid().ToString();
                a.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + a.Status });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // -------------------------------------------------------
        // PROFILE
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var a = await _context.StockAdjustments
                .Include(x => x.Product)
                .Include(x => x.Warehouse)
                .FirstOrDefaultAsync(x => x.AdjustmentId == id);
            if (a == null) return NotFound();
            return View(a);
        }
    }
}

