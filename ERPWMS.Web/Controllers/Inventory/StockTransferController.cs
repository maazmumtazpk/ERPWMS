using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Controllers.Inventory
{
    [Authorize]
    public class StockTransferController : Controller
    {
        private readonly AppDbContext _context;
        public StockTransferController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
        }

        private async Task<string> GenerateTransferNo()
        {
            try
            {
                var year = DateTime.Now.Year;
                var prefix = "TRF-" + year + "-";
                var existing = await _context.StockTransfers
                    .Where(t => t.TransferNo != null && t.TransferNo.StartsWith(prefix))
                    .Select(t => t.TransferNo).ToListAsync();
                int maxNum = 0;
                foreach (var no in existing)
                {
                    var parts = no.Split('-');
                    if (parts.Length >= 3 && int.TryParse(parts[2], out int n) && n > maxNum) maxNum = n;
                }
                return prefix + (maxNum + 1).ToString("D3");
            }
            catch { return "TRF-" + DateTime.Now.Year + "-" + DateTime.Now.ToString("HHmmss"); }
        }

        [HttpGet] public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var l = await _context.StockTransfers.Where(t => t.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(t => t.Status == "Completed" || t.Status == "Received"),
                inactive = l.Count(t => t.Status == "Pending" || t.Status == "Draft"),
                addedThisMonth = l.Count(t => t.CreatedDate.HasValue && t.CreatedDate.Value.Month == DateTime.Now.Month && t.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.StockTransfers
                .Include(t => t.Product)
                .Include(t => t.FromWarehouse)
                .Include(t => t.ToWarehouse)
                .Where(t => t.IsDeleted != true)
                .OrderBy(t => t.TransferId)
                .Select(t => new {
                    transferId = t.TransferId,
                    transferNo = t.TransferNo ?? "-",
                    productName = t.Product != null ? t.Product.ProductName : "N/A",
                    fromWarehouseName = t.FromWarehouse != null ? t.FromWarehouse.WarehouseName : "N/A",
                    toWarehouseName = t.ToWarehouse != null ? t.ToWarehouse.WarehouseName : "N/A",
                    quantity = t.Quantity,
                    status = t.Status ?? "Pending",
                    transferDate = t.TransferDate,
                    createdDate = t.CreatedDate
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
                    return Json(new { warehouseId = 0, warehouseName = "", balance = 0 });
                }

                var whId = ledgerRows.First().WarehouseId;
                var wh = await _context.Warehouses.FindAsync(whId);

                int inQty = ledgerRows.Where(r => (r.TransactionType ?? "").ToString().ToUpper() == "IN").Sum(r => r.Quantity);
                int outQty = ledgerRows.Where(r => (r.TransactionType ?? "").ToString().ToUpper() == "OUT").Sum(r => r.Quantity);
                int balance = inQty - outQty;

                return Json(new
                {
                    warehouseId = whId,
                    warehouseName = wh != null ? wh.WarehouseName : "",
                    balance = balance
                });
            }
            catch (Exception ex)
            {
                return Json(new { warehouseId = 0, warehouseName = "", balance = 0, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNextTransferNo()
        {
            return Json(new { transferNo = await GenerateTransferNo() });
        }

        [HttpGet] public IActionResult AddPartial() => PartialView("_AddForm");

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var t = await _context.StockTransfers.FindAsync(id);
            if (t == null) return NotFound();
            return PartialView("_EditForm", t);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int productId, int fromWarehouseId, int toWarehouseId, int quantity)
        {
            try
            {
                if (productId <= 0) return Json(new { success = false, message = "Product select karein." });
                if (quantity <= 0) return Json(new { success = false, message = "Quantity 0 se zyada honi chahiye." });
                if (fromWarehouseId == toWarehouseId) return Json(new { success = false, message = "From aur To warehouse same nahi ho sakte." });

                var transferNo = await GenerateTransferNo();

                _context.StockTransfers.Add(new StockTransfer
                {
                    TransferNo = transferNo,
                    ProductId = productId,
                    FromWarehouseId = fromWarehouseId,
                    ToWarehouseId = toWarehouseId,
                    Quantity = quantity,
                    TransferDate = DateTime.Now,
                    Status = "Pending",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Stock Transfer created: " + transferNo });
            }
            catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int transferId, int quantity, string status)
        {
            try
            {
                var t = await _context.StockTransfers.FindAsync(transferId);
                if (t == null) return Json(new { success = false, message = "Not found" });
                t.Quantity = quantity;
                t.Status = status;
                t.UpdatedBy = Uid().ToString();
                t.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Updated!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _context.StockTransfers.FindAsync(id);
            if (t == null) return Json(new { success = false });
            t.IsDeleted = true;
            t.IsActive = false;
            t.DeletedBy = Uid().ToString();
            t.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var t = await _context.StockTransfers.FindAsync(id);
            if (t == null) return Json(new { success = false });
            t.Status = t.Status == "Pending" ? "Completed" : "Pending";
            t.UpdatedBy = Uid().ToString();
            t.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Status: " + t.Status });
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var t = await _context.StockTransfers
                .Include(x => x.Product)
                .Include(x => x.FromWarehouse)
                .Include(x => x.ToWarehouse)
                .FirstOrDefaultAsync(x => x.TransferId == id);
            if (t == null) return NotFound();
            return View(t);
        }
    }
}