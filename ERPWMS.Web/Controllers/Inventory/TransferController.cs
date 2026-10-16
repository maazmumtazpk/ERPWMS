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
    public class TransferController : Controller
    {
        private readonly AppDbContext _context;

        public TransferController(AppDbContext c)
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
            var l = await _context.StockTransfers.Where(t => t.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(t => t.Status == "Approved" || t.Status == "Received"),
                inactive = l.Count(t => t.Status == "Pending"),
                addedThisMonth = l.Count(t =>
                    t.CreatedDate.HasValue &&
                    t.CreatedDate.Value.Month == DateTime.Now.Month &&
                    t.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.StockTransfers
                .Include(t => t.Product)
                .Include(t => t.FromWarehouse)
                .Include(t => t.ToWarehouse)
                .Where(t => t.IsDeleted != true)
                .OrderBy(t => t.TransferId)
                .Select(t => new
                {
                    transferId = t.TransferId,
                    transferNo = t.TransferNo,
                    productName = t.Product != null ? t.Product.ProductName : "N/A",
                    fromWarehouse = t.FromWarehouse != null ? t.FromWarehouse.WarehouseName : "N/A",
                    toWarehouse = t.ToWarehouse != null ? t.ToWarehouse.WarehouseName : "N/A",
                    quantity = t.Quantity,
                    transferDate = t.TransferDate,
                    status = t.Status ?? "Pending"
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
        public IActionResult AddTransferPartial() => PartialView("_AddTransferForm");

        [HttpGet]
        public async Task<IActionResult> EditTransferPartial(int id)
        {
            var t = await _context.StockTransfers.FindAsync(id);
            if (t == null) return NotFound();
            return PartialView("_EditTransferForm", t);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string transferNo,
            int fromWarehouseId,
            int toWarehouseId,
            int productId,
            int quantity,
            DateTime? transferDate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(transferNo))
                    return Json(new { success = false, message = "Transfer No required." });
                if (fromWarehouseId <= 0 || toWarehouseId <= 0)
                    return Json(new { success = false, message = "Please select both warehouses." });
                if (fromWarehouseId == toWarehouseId)
                    return Json(new { success = false, message = "From and To cannot be same." });

                _context.StockTransfers.Add(new StockTransfer
                {
                    TransferNo = transferNo.Trim(),
                    FromWarehouseId = fromWarehouseId,
                    ToWarehouseId = toWarehouseId,
                    ProductId = productId,
                    Quantity = quantity,
                    TransferDate = transferDate ?? DateTime.Now,
                    Status = "Pending",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Transfer created!" });
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
            int transferId,
            string transferNo,
            int fromWarehouseId,
            int toWarehouseId,
            int productId,
            int quantity,
            string? status)
        {
            try
            {
                var t = await _context.StockTransfers.FindAsync(transferId);
                if (t == null)
                    return Json(new { success = false, message = "Not found" });

                t.TransferNo = transferNo.Trim();
                t.FromWarehouseId = fromWarehouseId;
                t.ToWarehouseId = toWarehouseId;
                t.ProductId = productId;
                t.Quantity = quantity;
                t.Status = status;
                t.UpdatedBy = Uid().ToString();
                t.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Transfer updated!" });
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
                var t = await _context.StockTransfers.FindAsync(id);
                if (t == null)
                    return Json(new { success = false, message = "Not found" });

                t.IsDeleted = true;
                t.IsActive = false;
                t.DeletedBy = Uid().ToString();
                t.DeletedDate = DateTime.Now;
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
                var t = await _context.StockTransfers.FindAsync(id);
                if (t == null)
                    return Json(new { success = false, message = "Not found" });

                var currentUid = Uid().ToString();
                if (t.CreatedBy == currentUid)
                    return Json(new { success = false, message = "You cannot approve your own entry." });
                t.Status = t.Status == "Pending" ? "Approved" : "Pending";
                t.UpdatedBy = Uid().ToString();
                t.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + t.Status });
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

