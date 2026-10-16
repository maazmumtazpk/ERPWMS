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

namespace ERPWMS.Controllers.Inventory
{
    [Authorize]
    public class StockAdjustmentController : Controller
    {
        private readonly AppDbContext _context;
        public StockAdjustmentController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }

        [HttpGet] public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var l = await _context.StockAdjustments.Where(a => a.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(a => a.Status == "Approved" || a.Status == "Completed"),
                inactive = l.Count(a => a.Status == "Pending"),
                addedThisMonth = l.Count(a => a.CreatedDate.HasValue && a.CreatedDate.Value.Month == DateTime.Now.Month && a.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

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
                    productName = a.Product != null ? a.Product.ProductName : "N/A",
                    warehouseName = a.Warehouse != null ? a.Warehouse.WarehouseName : "N/A",
                    adjustmentType = a.AdjustmentType ?? "-",
                    quantity = a.Quantity,
                    previousQty = a.PreviousQty,
                    newQty = a.NewQty,
                    reason = a.Reason ?? "-",
                    status = a.Status ?? "Pending",
                    createdDate = a.CreatedDate
                }).ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var l = await _context.Products
                .Where(p => p.IsActive == true && p.IsDeleted != true)
                .Select(p => new { productId = p.ProductId, productName = p.ProductName })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouses()
        {
            var l = await _context.Warehouses
                .Where(w => w.IsActive == true && w.IsDeleted != true)
                .Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductBins(int productId)
        {
            try
            {
                var ledgerRows = await _context.StockLedgers
                    .Where(s => s.ProductId == productId && (s.IsDeleted == null || s.IsDeleted == false))
                    .Select(s => new { s.WarehouseId, s.BinId, s.Quantity, s.TransactionType })
                    .ToListAsync();

                if (ledgerRows.Count == 0)
                {
                    return Json(new { warehouseId = 0, warehouseName = "", bins = new List<Dictionary<string, object>>() });
                }

                var whId = ledgerRows.First().WarehouseId;
                var wh = await _context.Warehouses.FindAsync(whId);
                var binIds = ledgerRows.Where(r => r.WarehouseId == whId).Select(r => r.BinId).Distinct().ToList();

                var bins = new List<Dictionary<string, object>>();
                foreach (var binId in binIds)
                {
                    var bin = await _context.Bins.FindAsync(binId);

                    int inQty = ledgerRows
                        .Where(r => r.BinId == binId && (r.TransactionType ?? "").ToString().ToUpper() == "IN")
                        .Sum(r => r.Quantity);
                    int outQty = ledgerRows
                        .Where(r => r.BinId == binId && (r.TransactionType ?? "").ToString().ToUpper() == "OUT")
                        .Sum(r => r.Quantity);
                    int balance = inQty - outQty;

                    var dict = new Dictionary<string, object>();
                    dict["binId"] = binId;
                    dict["binName"] = bin != null ? bin.BinName : "";
                    dict["balance"] = balance;
                    bins.Add(dict);
                }

                return Json(new
                {
                    warehouseId = whId,
                    warehouseName = wh != null ? wh.WarehouseName : "",
                    bins = bins
                });
            }
            catch (Exception ex)
            {
                return Json(new { warehouseId = 0, warehouseName = "", bins = new List<Dictionary<string, object>>(), error = "Operation failed." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBins()
        {
            var l = await _context.Bins
                .Where(b => b.IsActive != false && (b.IsDeleted == null || b.IsDeleted == false))
                .Select(b => new { binId = b.BinId, binName = b.BinName })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet] public IActionResult AddPartial() => PartialView("_AddForm");

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var a = await _context.StockAdjustments.FindAsync(id);
            if (a == null) return NotFound();
            return PartialView("_EditForm", a);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int productId, int warehouseId, int binId, string adjustmentType, int quantity, int previousQty, string reason)
        {
            try
            {
                if (productId <= 0) return Json(new { success = false, message = "Product select karein." });
                if (quantity <= 0) return Json(new { success = false, message = "Quantity 0 se zyada honi chahiye." });

                int newQty = (adjustmentType == "Decrease") ? (previousQty - quantity) : (previousQty + quantity);

                _context.StockAdjustments.Add(new StockAdjustment
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    BinId = binId,
                    AdjustmentType = adjustmentType ?? "Increase",
                    Quantity = quantity,
                    PreviousQty = previousQty,
                    NewQty = newQty,
                    Reason = reason ?? "Manual adjustment",
                    Status = "Pending",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Stock Adjustment created! New Qty: " + newQty });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int adjustmentId, int quantity, string reason, string status)
        {
            try
            {
                var a = await _context.StockAdjustments.FindAsync(adjustmentId);
                if (a == null) return Json(new { success = false, message = "Not found" });
                a.Quantity = quantity;
                a.NewQty = (a.AdjustmentType == "Decrease") ? (a.PreviousQty - quantity) : (a.PreviousQty + quantity);
                a.Reason = reason;
                a.Status = status;
                a.UpdatedBy = Uid().ToString();
                a.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Updated!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await _context.StockAdjustments.FindAsync(id);
            if (a == null) return Json(new { success = false });
            a.IsDeleted = true;
            a.IsActive = false;
            a.DeletedBy = Uid().ToString();
            a.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Toggle(int id)
        {
            var a = await _context.StockAdjustments.FindAsync(id);
            if (a == null) return Json(new { success = false });
            var currentUid = Uid().ToString();
                if (a.CreatedBy == currentUid)
                    return Json(new { success = false, message = "You cannot approve your own entry." });
                a.Status = a.Status == "Pending" ? "Approved" : a.Status == "Approved" ? "Completed" : "Pending";
            a.UpdatedBy = Uid().ToString();
            a.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Status: " + a.Status });
        }

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

