using ERPWMS.Application.DTOs.Procurement;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;
using ICurrentUserService = ERPWMS.Application.Common.ICurrentUserService;
using ERPWMS.Application.Common;
using ERPWMS.Domain.Services;

namespace ERPWMS.Web.Controllers.Procurement
{
    [Authorize]
    public class PurchaseMasterController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _userSvc;

        public PurchaseMasterController(AppDbContext context, ICurrentUserService userSvc)
        {
            _context = context;
            _userSvc = userSvc;
        }

        // ✅ FIX: CompanyId int? hai — direct use karo
        private int CompanyId => _userSvc.CompanyId ?? 1;

        // ✅ FIX: UserId string? hai — TryParse use karo
        private int UserId => int.TryParse(_userSvc.UserId, out var u) ? u : 1;

        // ---------------------------------------------------
        // INDEX
        // ---------------------------------------------------
        public IActionResult Index() => View();

        // ---------------------------------------------------
        // GET ALL JSON
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAllJson()
        {
            try
            {
                var list = await _context.PurchaseMasters
                    .Include(p => p.Supplier)
                    .Include(p => p.Warehouse)
                    .Where(p => p.IsDeleted != true)
                    .OrderBy(p => p.PurchaseId)
                    .Select(p => new
                    {
                        purchaseId = p.PurchaseId,
                        purchaseOrderNo = p.PurchaseOrderNo,
                        supplierName = p.Supplier != null ? p.Supplier.SupplierName : "",
                        warehouseName = p.Warehouse != null ? p.Warehouse.WarehouseName : "",
                        orderDate = p.OrderDate,
                        netAmount = p.NetAmount,
                        status = p.Status ?? "Draft",
                        createdByName = p.CreatedByNavigation != null ? p.CreatedByNavigation.UserName : "",
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

        // ---------------------------------------------------
        // CREATE PARTIAL
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetCreatePartial()
        {
            ViewBag.Suppliers = await _context.Suppliers.Where(s => s.IsActive == true && s.IsDeleted != true).OrderBy(s => s.SupplierName).ToListAsync();
            ViewBag.Warehouses = await _context.Warehouses.Where(w => w.IsActive == true && w.IsDeleted != true).OrderBy(w => w.WarehouseName).ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsActive == true && p.IsDeleted != true).OrderBy(p => p.ProductName).ToListAsync();
            return PartialView("_CreateForm");
        }

        // ---------------------------------------------------
        // EDIT PARTIAL
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetEditPartial(int id)
        {
            var po = await _context.PurchaseMasters
                .Include(p => p.PurchaseDetails)
                .FirstOrDefaultAsync(p => p.PurchaseId == id && p.IsDeleted != true);
            if (po == null) return NotFound();

            ViewBag.Suppliers = await _context.Suppliers.Where(s => s.IsActive == true && s.IsDeleted != true).OrderBy(s => s.SupplierName).ToListAsync();
            ViewBag.Warehouses = await _context.Warehouses.Where(w => w.IsActive == true && w.IsDeleted != true).OrderBy(w => w.WarehouseName).ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsActive == true && p.IsDeleted != true).OrderBy(p => p.ProductName).ToListAsync();
            ViewBag.PurchaseId = po.PurchaseId;
            ViewBag.PurchaseDetails = po.PurchaseDetails.Where(d => d.IsDeleted != true).ToList();
            ViewBag.Purchase = po;
            return PartialView("_EditForm");
        }

        // ---------------------------------------------------
        // CREATE AJAX
        // ---------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax()
        {
            try
            {
                var f = Request.Form;
                if (!int.TryParse(f["SupplierId"], out var supplierId) || supplierId <= 0)
                    return Json(new { success = false, message = "Supplier required." });
                if (!int.TryParse(f["WarehouseId"], out var warehouseId) || warehouseId <= 0)
                    return Json(new { success = false, message = "Warehouse required." });
                var orderNo = (f["PurchaseOrderNo"].ToString() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(orderNo))
                    return Json(new { success = false, message = "PO Number required." });

                var po = new PurchaseMaster
                {
                    SupplierId = supplierId,
                    WarehouseId = warehouseId,
                    PurchaseOrderNo = orderNo,
                    OrderDate = DateTime.TryParse(f["OrderDate"], out var od) ? od : DateTime.UtcNow,
                    ExpectedDeliveryDate = DateOnly.TryParse(f["ExpectedDeliveryDate"], out var edd) ? edd : (DateOnly?)null,
                    Discount = decimal.TryParse(f["Discount"], out var disc) ? disc : 0,
                    TaxAmount = decimal.TryParse(f["TaxAmount"], out var tax) ? tax : 0,
                    Status = "Draft",
                    IsDeleted = false,
                    CreatedBy = UserId.ToString(),
                    CreatedDate = DateTime.UtcNow
                };

                var details = new List<PurchaseDetail>();
                for (int i = 0; ; i++)
                {
                    var pidStr = f["PurchaseDetails[" + i + "].ProductId"].ToString();
                    if (string.IsNullOrEmpty(pidStr)) break;
                    if (!int.TryParse(pidStr, out var pid) || pid <= 0) continue;
                    var qtyStr = f["PurchaseDetails[" + i + "].Quantity"].ToString();
                    var priceStr = f["PurchaseDetails[" + i + "].UnitPrice"].ToString();
                    if (!int.TryParse(qtyStr, out var qty) || qty <= 0) continue;
                    if (!decimal.TryParse(priceStr, out var price) || price <= 0) continue;

                    decimal? dp = null;
                    if (decimal.TryParse(f["PurchaseDetails[" + i + "].DiscountPercent"], out var dpv)) dp = dpv;

                    details.Add(new PurchaseDetail
                    {
                        ProductId = pid,
                        Quantity = qty,
                        UnitPrice = price,
                        DiscountPercent = dp,
                        BatchNo = f["PurchaseDetails[" + i + "].BatchNo"].ToString(),
                        CreatedBy = UserId.ToString(),
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }

                if (details.Count == 0)
                    return Json(new { success = false, message = "At least one item required." });

                po.TotalAmount = details.Sum(d =>
                {
                    decimal line = d.Quantity * d.UnitPrice;
                    decimal lineDisc = line * ((d.DiscountPercent ?? 0) / 100m);
                    return line - lineDisc;
                });

                po.NetAmount = po.TotalAmount - (po.Discount ?? 0) + (po.TaxAmount ?? 0);

                _context.PurchaseMasters.Add(po);
                await _context.SaveChangesAsync();

                foreach (var d in details)
                {
                    d.PurchaseId = po.PurchaseId;
                    d.TotalPrice = (d.Quantity * d.UnitPrice) * (1 - ((d.DiscountPercent ?? 0) / 100m));
                }
                _context.PurchaseDetails.AddRange(details);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "PO '" + po.PurchaseOrderNo + "' created.", id = po.PurchaseId });
            }
            catch (Exception ex)
            {
                var m = ex.Message;
                if (ex.InnerException != null) m += " | " + ex.InnerException.Message;
                return Json(new { success = false, message = m });
            }
        }

        // ---------------------------------------------------
        // UPDATE AJAX
        // ---------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAjax()
        {
            try
            {
                var f = Request.Form;
                if (!int.TryParse(f["PurchaseId"], out var id))
                    return Json(new { success = false, message = "Invalid ID." });

                var po = await _context.PurchaseMasters
                    .Include(p => p.PurchaseDetails)
                    .FirstOrDefaultAsync(p => p.PurchaseId == id);
                if (po == null) return Json(new { success = false, message = "Not found." });
                if (po.Status != "Draft") return Json(new { success = false, message = "Only Draft can be edited." });

                po.SupplierId = int.TryParse(f["SupplierId"], out var sid) ? sid : po.SupplierId;
                po.WarehouseId = int.TryParse(f["WarehouseId"], out var wid) ? wid : po.WarehouseId;
                po.PurchaseOrderNo = (f["PurchaseOrderNo"].ToString() ?? po.PurchaseOrderNo).Trim();
                if (DateTime.TryParse(f["OrderDate"], out var od)) po.OrderDate = od;
                if (DateOnly.TryParse(f["ExpectedDeliveryDate"], out var edd2)) po.ExpectedDeliveryDate = edd2;
                if (decimal.TryParse(f["Discount"], out var disc)) po.Discount = disc;
                if (decimal.TryParse(f["TaxAmount"], out var tax)) po.TaxAmount = tax;
                po.UpdatedBy = UserId.ToString();
                po.UpdatedDate = DateTime.UtcNow;

                foreach (var d in po.PurchaseDetails.ToList())
                {
                    _context.PurchaseDetails.Remove(d);
                }

                var newDetails = new List<PurchaseDetail>();
                for (int i = 0; ; i++)
                {
                    var pidStr = f["PurchaseDetails[" + i + "].ProductId"].ToString();
                    if (string.IsNullOrEmpty(pidStr)) break;
                    if (!int.TryParse(pidStr, out var pid) || pid <= 0) continue;
                    if (!int.TryParse(f["PurchaseDetails[" + i + "].Quantity"], out var qty) || qty <= 0) continue;
                    if (!decimal.TryParse(f["PurchaseDetails[" + i + "].UnitPrice"], out var price) || price <= 0) continue;

                    decimal? dp = null;
                    if (decimal.TryParse(f["PurchaseDetails[" + i + "].DiscountPercent"], out var dpv)) dp = dpv;

                    newDetails.Add(new PurchaseDetail
                    {
                        PurchaseId = po.PurchaseId,
                        ProductId = pid,
                        Quantity = qty,
                        UnitPrice = price,
                        DiscountPercent = dp,
                        BatchNo = f["PurchaseDetails[" + i + "].BatchNo"].ToString(),
                        TotalPrice = (qty * price) * (1 - ((dp ?? 0) / 100m)),
                        CreatedBy = UserId.ToString(),
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }

                if (newDetails.Count == 0)
                    return Json(new { success = false, message = "At least one item required." });

                po.TotalAmount = newDetails.Sum(d =>
                {
                    decimal line = d.Quantity * d.UnitPrice;
                    decimal lineDisc = line * ((d.DiscountPercent ?? 0) / 100m);
                    return line - lineDisc;
                });

                po.NetAmount = po.TotalAmount - (po.Discount ?? 0) + (po.TaxAmount ?? 0);

                _context.PurchaseDetails.AddRange(newDetails);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "PO updated." });
            }
            catch (Exception ex)
            {
                var m = ex.Message;
                if (ex.InnerException != null) m += " | " + ex.InnerException.Message;
                return Json(new { success = false, message = m });
            }
        }

        // ---------------------------------------------------
        // SUBMIT
        // ---------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int id)
        {
            try
            {
                var po = await _context.PurchaseMasters.FindAsync(id);
                if (po == null) return NotFound();
                if (po.Status != "Draft") throw new Exception("Only Draft can be submitted.");
                po.Status = "Submitted";
                po.UpdatedBy = UserId.ToString();
                po.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "PO submitted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
            }
            return RedirectToAction("Details", new { id });
        }

        // ---------------------------------------------------
        // APPROVE
        // ---------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var po = await _context.PurchaseMasters.FindAsync(id);
                if (po == null) return NotFound();
                if (po.Status != "Submitted") throw new Exception("Only Submitted can be approved.");
                po.Status = "Approved";
                po.UpdatedBy = UserId.ToString();
                po.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "PO approved.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
            }
            return RedirectToAction("Details", new { id });
        }

        // ---------------------------------------------------
        // REJECT
        // ---------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var po = await _context.PurchaseMasters.FindAsync(id);
                if (po == null) return NotFound();
                if (po.Status != "Submitted") throw new Exception("Only Submitted can be rejected.");
                po.Status = "Rejected";
                po.UpdatedBy = UserId.ToString();
                po.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "PO rejected.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
            }
            return RedirectToAction("Details", new { id });
        }

        // ---------------------------------------------------
        // DELETE
        // ---------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var po = await _context.PurchaseMasters.FindAsync(id);
                if (po == null) return Json(new { success = false });
                po.IsDeleted = true;
                po.DeletedBy = UserId.ToString();
                po.DeletedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ---------------------------------------------------
        // DETAILS
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var po = await _context.PurchaseMasters
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .FirstOrDefaultAsync(p => p.PurchaseId == id && p.IsDeleted != true);
            if (po == null) return NotFound();

            var details = await _context.PurchaseDetails
                .Include(d => d.Product)
                .Where(d => d.PurchaseId == id && d.IsDeleted != true)
                .ToListAsync();

            ViewBag.Details = details;
            return View(po);
        }

        // ---------------------------------------------------
        // GET PURCHASE ITEMS
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetPurchaseItems(int purchaseId)
        {
            if (purchaseId <= 0) return Json(new List<object>());
            var items = await _context.PurchaseDetails
                .Include(d => d.Product)
                .Where(d => d.PurchaseId == purchaseId && d.IsDeleted != true)
                .Select(d => new
                {
                    detailId = d.DetailId,
                    productId = d.ProductId,
                    productName = d.Product != null ? d.Product.ProductName : "N/A",
                    quantity = d.Quantity,
                    unitPrice = d.UnitPrice
                }).ToListAsync();
            return Json(items);
        }

        // ---------------------------------------------------
        // ALIASES
        // ---------------------------------------------------
        [HttpGet]
        public Task<IActionResult> GetAll() => GetAllJson();

        [HttpGet]
        public Task<IActionResult> AddPurchasePartial() => GetCreatePartial();

        [HttpGet]
        public Task<IActionResult> EditPurchasePartial(int id) => GetEditPartial(id);

        // ---------------------------------------------------
        // STATISTICS
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var list = await _context.PurchaseMasters
                    .Where(p => p.IsDeleted != true)
                    .ToListAsync();
                var now = DateTime.Now;
                return Json(new
                {
                    total = list.Count,
                    active = list.Count(p => p.Status == "Approved"),
                    inactive = list.Count(p => p.Status == "Pending" || p.Status == "Draft"),
                    addedThisMonth = list.Count(p =>
                        p.CreatedDate.HasValue &&
                        p.CreatedDate.Value.Month == now.Month &&
                        p.CreatedDate.Value.Year == now.Year)
                });
            }
            catch (Exception ex)
            {
                return Json(new { total = 0, active = 0, inactive = 0, addedThisMonth = 0, error = ex.Message });
            }
        }

        // ---------------------------------------------------
        // PROFILE
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            try
            {
                var pm = await _context.PurchaseMasters
                    .Include(p => p.Supplier)
                    .Include(p => p.Warehouse)
                    .FirstOrDefaultAsync(p => p.PurchaseId == id);

                if (pm == null) return NotFound();

                var details = await _context.PurchaseDetails
                    .Include(d => d.Product)
                    .Where(d => d.PurchaseId == id && d.IsDeleted != true)
                    .ToListAsync();

                ViewBag.Details = details;

                return View("Profile", pm);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // ---------------------------------------------------
        // DROPDOWNS
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetSuppliers()
        {
            var l = await _context.Suppliers
                .Where(s => s.IsActive == true && s.IsDeleted != true)
                .OrderBy(s => s.SupplierName)
                .Select(s => new { supplierId = s.SupplierId, supplierName = s.SupplierName })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouses()
        {
            var l = await _context.Warehouses
                .Where(w => w.IsActive == true && w.IsDeleted != true)
                .OrderBy(w => w.WarehouseName)
                .Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var l = await _context.Products
                .Where(p => p.IsActive == true && p.IsDeleted != true)
                .OrderBy(p => p.ProductName)
                .Select(p => new { productId = p.ProductId, productName = p.ProductName })
                .ToListAsync();
            return Json(l);
        }

        // ---------------------------------------------------
        // TOGGLE STATUS
        // ---------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var p = await _context.PurchaseMasters.FindAsync(id);
                if (p == null)
                    return Json(new { success = false, message = "Purchase order not found." });

                if (p.Status == "Approved" || p.Status == "Completed")
                    return Json(new { success = false, message = "Approved/Completed POs cannot be toggled." });

                string oldStatus = p.Status ?? "Draft";
                p.Status = "Approved";
                p.ApprovalStatus = "Approved";
                p.UpdatedBy = UserId.ToString();
                p.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status changed: " + oldStatus + " -> " + p.Status });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}