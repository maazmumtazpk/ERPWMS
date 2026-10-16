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
    public class CycleCountController : Controller
    {
        private readonly AppDbContext _context;

        public CycleCountController(AppDbContext c)
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
            var l = await _context.CycleCounts.Where(c => c.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(c => c.Status == "Verified"),
                inactive = l.Count(c => c.Status == "Pending"),
                addedThisMonth = l.Count(c =>
                    c.CountedDate.HasValue &&
                    c.CountedDate.Value.Month == DateTime.Now.Month &&
                    c.CountedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.CycleCounts
                .Include(c => c.Product)
                .Include(c => c.Warehouse)
                .Include(c => c.Bin)
                .Where(c => c.IsDeleted != true)
                .OrderBy(c => c.CycleCountId)
                .Select(c => new
                {
                    cycleCountId = c.CycleCountId,
                    productName = c.Product != null ? c.Product.ProductName : "N/A",
                    warehouseName = c.Warehouse != null ? c.Warehouse.WarehouseName : "N/A",
                    binName = c.Bin != null ? c.Bin.BinName : "N/A",
                    systemQty = c.SystemQty,
                    actualQty = c.ActualQty,
                    variance = c.Variance,
                    countedDate = c.CountedDate,
                    status = c.Status ?? "Pending"
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
        // GET PRODUCT BINS
        // -------------------------------------------------------
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
                        .Where(r => r.BinId == binId && (r.TransactionType ?? "").ToUpper() == "IN")
                        .Sum(r => r.Quantity);

                    int outQty = ledgerRows
                        .Where(r => r.BinId == binId && (r.TransactionType ?? "").ToUpper() == "OUT")
                        .Sum(r => r.Quantity);

                    int balance = inQty - outQty;

                    var dict = new Dictionary<string, object>
                    {
                        ["binId"] = binId,
                        ["binName"] = bin != null ? bin.BinName : "",
                        ["balance"] = balance
                    };
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

        // -------------------------------------------------------
        // ADD / EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult AddCycleCountPartial() => PartialView("_AddCycleCountForm");

        [HttpGet]
        public async Task<IActionResult> EditCycleCountPartial(int id)
        {
            var c = await _context.CycleCounts.FindAsync(id);
            if (c == null) return NotFound();
            return PartialView("_EditCycleCountForm", c);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int productId,
            int warehouseId,
            int binId,
            int systemQty,
            int actualQty,
            string? remarks)
        {
            try
            {
                if (productId <= 0)
                    return Json(new { success = false, message = "Please select product." });

                _context.CycleCounts.Add(new CycleCount
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    BinId = binId,
                    SystemQty = systemQty,
                    ActualQty = actualQty,
                    Variance = actualQty - systemQty,
                    CountedBy = Uid().ToString(),     // âœ… FIX: int â†’ string
                    CountedDate = DateTime.Now,
                    Status = "Pending",
                    Remarks = remarks ?? "",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),     // âœ… string
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cycle count created!" });
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
            int cycleCountId,
            int actualQty,
            string? remarks,
            string? status)
        {
            try
            {
                var c = await _context.CycleCounts.FindAsync(cycleCountId);
                if (c == null)
                    return Json(new { success = false, message = "Not found" });

                c.ActualQty = actualQty;
                c.Variance = actualQty - c.SystemQty;
                c.Remarks = remarks;
                c.Status = status;
                c.UpdatedBy = Uid().ToString();
                c.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cycle count updated!" });
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
                var c = await _context.CycleCounts.FindAsync(id);
                if (c == null)
                    return Json(new { success = false, message = "Not found" });

                c.IsDeleted = true;
                c.IsActive = false;
                c.DeletedBy = Uid().ToString();
                c.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // -------------------------------------------------------
        // TOGGLE STATUS (Pending <-> Verified)
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var c = await _context.CycleCounts.FindAsync(id);
                if (c == null)
                    return Json(new { success = false, message = "Not found" });

                var currentUid = Uid().ToString();
                if (c.CreatedBy == currentUid)
                    return Json(new { success = false, message = "You cannot approve your own entry." });
                c.Status = c.Status == "Pending" ? "Verified" : "Pending";
                c.UpdatedBy = Uid().ToString();
                c.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + c.Status });
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
            var c = await _context.CycleCounts
                .Include(x => x.Product)
                .Include(x => x.Warehouse)
                .Include(x => x.Bin)
                .FirstOrDefaultAsync(x => x.CycleCountId == id);
            if (c == null) return NotFound();
            return View(c);
        }
    }
}

