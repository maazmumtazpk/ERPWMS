using ERPWMS.Application.Common;
using ERPWMS.Database;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICurrentUserService = ERPWMS.Application.Common.ICurrentUserService;

namespace ERPWMS.Web.Controllers.Procurement
{
    [Authorize(Policy = "Purchase.Access")]
    public class PurchaseMasterController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _userSvc;

        public PurchaseMasterController(AppDbContext context, ICurrentUserService userSvc)
        {
            _context = context;
            _userSvc = userSvc;
        }

        private int CompanyId
        {
            get
            {
                if (!_userSvc.CompanyId.HasValue || _userSvc.CompanyId.Value <= 0)
                    throw new UnauthorizedAccessException("Company context missing.");
                return _userSvc.CompanyId.Value;
            }
        }

        private int UserId
        {
            get
            {
                if (!int.TryParse(_userSvc.UserId, out var u) || u <= 0)
                    throw new UnauthorizedAccessException("User identity missing.");
                return u;
            }
        }

        private const int MaxLineItems = 500;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAllJson()
        {
            var list = await _context.PurchaseMasters
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .Where(p => p.IsDeleted != true)
                .OrderByDescending(p => p.PurchaseId)
                .Select(p => new
                {
                    purchaseId = p.PurchaseId,
                    purchaseOrderNo = p.PurchaseOrderNo,
                    supplierName = p.Supplier != null ? p.Supplier.SupplierName : "",
                    warehouseName = p.Warehouse != null ? p.Warehouse.WarehouseName : "",
                    orderDate = p.OrderDate,
                    netAmount = p.NetAmount,
                    status = p.Status ?? "Draft",
                    createdDate = p.CreatedDate
                }).ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetCreatePartial()
        {
            ViewBag.Suppliers = await _context.Suppliers.Where(s => s.IsActive == true && s.IsDeleted != true).OrderBy(s => s.SupplierName).ToListAsync();
            ViewBag.Warehouses = await _context.Warehouses.Where(w => w.IsActive == true && w.IsDeleted != true).OrderBy(w => w.WarehouseName).ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsActive == true && p.IsDeleted != true).OrderBy(p => p.ProductName).ToListAsync();
            return PartialView("_CreateForm");
        }

        [HttpGet]
        public async Task<IActionResult> GetEditPartial(int id)
        {
            var po = await _context.PurchaseMasters.Include(p => p.PurchaseDetails)
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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax()
        {
            var f = Request.Form;
            if (!int.TryParse(f["SupplierId"], out var supplierId) || supplierId <= 0)
                return Json(new { success = false, message = "Supplier required." });
            if (!int.TryParse(f["WarehouseId"], out var warehouseId) || warehouseId <= 0)
                return Json(new { success = false, message = "Warehouse required." });

            var orderNo = (f["PurchaseOrderNo"].ToString() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(orderNo))
                return Json(new { success = false, message = "PO Number required." });

            if (await _context.PurchaseMasters.AnyAsync(p => p.PurchaseOrderNo == orderNo && p.IsDeleted != true))
                return Json(new { success = false, message = "PO Number already exists." });

            decimal disc = 0, tax = 0;
            if (!decimal.TryParse(f["Discount"], out disc) || disc < 0) disc = 0;
            if (!decimal.TryParse(f["TaxAmount"], out tax) || tax < 0) tax = 0;

            var po = new PurchaseMaster
            {
                SupplierId = supplierId,
                WarehouseId = warehouseId,
                PurchaseOrderNo = orderNo,
                OrderDate = DateTime.TryParse(f["OrderDate"], out var od) ? od : DateTime.UtcNow,
                ExpectedDeliveryDate = DateOnly.TryParse(f["ExpectedDeliveryDate"], out var edd) ? edd : (DateOnly?)null,
                Discount = disc,
                TaxAmount = tax,
                Status = "Draft",
                IsDeleted = false,
                CreatedBy = UserId.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            var details = new List<PurchaseDetail>();
            for (int i = 0; i < MaxLineItems; i++)
            {
                var pidStr = f["PurchaseDetails[" + i + "].ProductId"].ToString();
                if (string.IsNullOrEmpty(pidStr)) break;
                if (!int.TryParse(pidStr, out var pid) || pid <= 0) continue;
                if (!int.TryParse(f["PurchaseDetails[" + i + "].Quantity"].ToString(), out var qty) || qty <= 0) continue;
                if (!decimal.TryParse(f["PurchaseDetails[" + i + "].UnitPrice"].ToString(), out var price) || price < 0) continue;

                decimal? dp = null;
                if (decimal.TryParse(f["PurchaseDetails[" + i + "].DiscountPercent"], out var dpv) && dpv >= 0 && dpv <= 100) dp = dpv;

                decimal line = qty * price;
                decimal net = line - (line * ((dp ?? 0) / 100m));

                details.Add(new PurchaseDetail
                {
                    ProductId = pid,
                    Quantity = qty,
                    UnitPrice = price,
                    DiscountPercent = dp,
                    TotalPrice = net,
                    BatchNo = f["PurchaseDetails[" + i + "].BatchNo"].ToString(),
                    CreatedBy = UserId.ToString(),
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                });
            }

            if (details.Count == 0)
                return Json(new { success = false, message = "At least one item required." });

            po.TotalAmount = details.Sum(d => d.TotalPrice);
            po.NetAmount = po.TotalAmount - (po.Discount ?? 0) + (po.TaxAmount ?? 0);

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.PurchaseMasters.Add(po);
                await _context.SaveChangesAsync();
                foreach (var d in details) d.PurchaseId = po.PurchaseId;
                _context.PurchaseDetails.AddRange(details);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                return Json(new { success = false, message = "Could not create PO." });
            }

            return Json(new { success = true, message = "PO '" + po.PurchaseOrderNo + "' created.", id = po.PurchaseId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Purchase.Edit")]
        public async Task<IActionResult> UpdateAjax()
        {
            var f = Request.Form;
            if (!int.TryParse(f["PurchaseId"], out var id))
                return Json(new { success = false, message = "Invalid ID." });

            var po = await _context.PurchaseMasters.Include(p => p.PurchaseDetails)
                .FirstOrDefaultAsync(p => p.PurchaseId == id && p.IsDeleted != true);
            if (po == null) return Json(new { success = false, message = "Not found." });
            if (po.Status != "Draft") return Json(new { success = false, message = "Only Draft can be edited." });

            if (int.TryParse(f["SupplierId"], out var sid) && sid > 0) po.SupplierId = sid;
            if (int.TryParse(f["WarehouseId"], out var wid) && wid > 0) po.WarehouseId = wid;
            po.PurchaseOrderNo = (f["PurchaseOrderNo"].ToString() ?? po.PurchaseOrderNo).Trim();
            if (DateTime.TryParse(f["OrderDate"], out var od)) po.OrderDate = od;
            if (DateOnly.TryParse(f["ExpectedDeliveryDate"], out var edd2)) po.ExpectedDeliveryDate = edd2;

            decimal disc = 0, tax = 0;
            if (!decimal.TryParse(f["Discount"], out disc) || disc < 0) disc = 0;
            if (!decimal.TryParse(f["TaxAmount"], out tax) || tax < 0) tax = 0;
            po.Discount = disc;
            po.TaxAmount = tax;
            po.UpdatedBy = UserId.ToString();
            po.UpdatedDate = DateTime.UtcNow;

            foreach (var d in po.PurchaseDetails.ToList()) d.IsDeleted = true;

            var newDetails = new List<PurchaseDetail>();
            for (int i = 0; i < MaxLineItems; i++)
            {
                var pidStr = f["PurchaseDetails[" + i + "].ProductId"].ToString();
                if (string.IsNullOrEmpty(pidStr)) break;
                if (!int.TryParse(pidStr, out var pid) || pid <= 0) continue;
                if (!int.TryParse(f["PurchaseDetails[" + i + "].Quantity"].ToString(), out var qty) || qty <= 0) continue;
                if (!decimal.TryParse(f["PurchaseDetails[" + i + "].UnitPrice"].ToString(), out var price) || price < 0) continue;

                decimal? dp = null;
                if (decimal.TryParse(f["PurchaseDetails[" + i + "].DiscountPercent"], out var dpv) && dpv >= 0 && dpv <= 100) dp = dpv;

                decimal line = qty * price;
                decimal net = line - (line * ((dp ?? 0) / 100m));
                newDetails.Add(new PurchaseDetail
                {
                    PurchaseId = po.PurchaseId,
                    ProductId = pid,
                    Quantity = qty,
                    UnitPrice = price,
                    DiscountPercent = dp,
                    BatchNo = f["PurchaseDetails[" + i + "].BatchNo"].ToString(),
                    TotalPrice = net,
                    CreatedBy = UserId.ToString(),
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                });
            }

            if (newDetails.Count == 0)
                return Json(new { success = false, message = "At least one item required." });

            po.TotalAmount = newDetails.Sum(d => d.TotalPrice);
            po.NetAmount = po.TotalAmount - (po.Discount ?? 0) + (po.TaxAmount ?? 0);
            _context.PurchaseDetails.AddRange(newDetails);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "PO updated." });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Purchase.Submit")]
        public async Task<IActionResult> Submit(int id)
        {
            var po = await _context.PurchaseMasters.FindAsync(id);
            if (po == null || po.IsDeleted == true) return NotFound();
            if (po.Status != "Draft")
            {
                TempData["Error"] = "Only Draft can be submitted.";
                return RedirectToAction("Details", new { id });
            }
            po.Status = "Submitted";
            po.UpdatedBy = UserId.ToString();
            po.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            TempData["Success"] = "PO submitted.";
            return RedirectToAction("Details", new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Purchase.Approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var po = await _context.PurchaseMasters.FindAsync(id);
            if (po == null || po.IsDeleted == true) return NotFound();
            if (po.Status != "Submitted")
            {
                TempData["Error"] = "Only Submitted can be approved.";
                return RedirectToAction("Details", new { id });
            }
            if (po.CreatedBy == UserId.ToString())
            {
                TempData["Error"] = "You cannot approve your own PO.";
                return RedirectToAction("Details", new { id });
            }
            po.Status = "Approved";
            po.UpdatedBy = UserId.ToString();
            po.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            TempData["Success"] = "PO approved.";
            return RedirectToAction("Details", new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Purchase.Approve")]
        public async Task<IActionResult> Reject(int id)
        {
            var po = await _context.PurchaseMasters.FindAsync(id);
            if (po == null || po.IsDeleted == true) return NotFound();
            if (po.Status != "Submitted")
            {
                TempData["Error"] = "Only Submitted can be rejected.";
                return RedirectToAction("Details", new { id });
            }
            po.Status = "Rejected";
            po.UpdatedBy = UserId.ToString();
            po.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            TempData["Success"] = "PO rejected.";
            return RedirectToAction("Details", new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Purchase.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var po = await _context.PurchaseMasters.FindAsync(id);
            if (po == null || po.IsDeleted == true) return Json(new { success = false });
            if (po.Status == "Approved" || po.Status == "Completed")
                return Json(new { success = false, message = "Approved/Completed POs cannot be deleted." });

            po.IsDeleted = true;
            po.DeletedBy = UserId.ToString();
            po.DeletedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var po = await _context.PurchaseMasters.Include(p => p.Supplier).Include(p => p.Warehouse)
                .FirstOrDefaultAsync(p => p.PurchaseId == id && p.IsDeleted != true);
            if (po == null) return NotFound();

            ViewBag.Details = await _context.PurchaseDetails.Include(d => d.Product)
                .Where(d => d.PurchaseId == id && d.IsDeleted != true).ToListAsync();
            return View(po);
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchaseItems(int purchaseId)
        {
            if (purchaseId <= 0) return Json(new List<object>());
            var items = await _context.PurchaseDetails.Include(d => d.Product)
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

        [HttpGet] public Task<IActionResult> GetAll() => GetAllJson();
        [HttpGet] public Task<IActionResult> AddPurchasePartial() => GetCreatePartial();
        [HttpGet] public Task<IActionResult> EditPurchasePartial(int id) => GetEditPartial(id);

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var now = DateTime.Now;
            var q = _context.PurchaseMasters.Where(p => p.IsDeleted != true);
            return Json(new
            {
                total = await q.CountAsync(),
                active = await q.CountAsync(p => p.Status == "Approved"),
                inactive = await q.CountAsync(p => p.Status == "Pending" || p.Status == "Draft"),
                addedThisMonth = await q.CountAsync(p =>
                    p.CreatedDate.HasValue &&
                    p.CreatedDate.Value.Month == now.Month &&
                    p.CreatedDate.Value.Year == now.Year)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var pm = await _context.PurchaseMasters.Include(p => p.Supplier).Include(p => p.Warehouse)
                .FirstOrDefaultAsync(p => p.PurchaseId == id && p.IsDeleted != true);
            if (pm == null) return NotFound();

            ViewBag.Details = await _context.PurchaseDetails.Include(d => d.Product)
                .Where(d => d.PurchaseId == id && d.IsDeleted != true).ToListAsync();
            return View("Profile", pm);
        }

        [HttpGet]
        public async Task<IActionResult> GetSuppliers() =>
            Json(await _context.Suppliers.Where(s => s.IsActive == true && s.IsDeleted != true)
                .OrderBy(s => s.SupplierName)
                .Select(s => new { supplierId = s.SupplierId, supplierName = s.SupplierName }).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> GetWarehouses() =>
            Json(await _context.Warehouses.Where(w => w.IsActive == true && w.IsDeleted != true)
                .OrderBy(w => w.WarehouseName)
                .Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName }).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> GetProducts() =>
            Json(await _context.Products.Where(p => p.IsActive == true && p.IsDeleted != true)
                .OrderBy(p => p.ProductName)
                .Select(p => new { productId = p.ProductId, productName = p.ProductName }).ToListAsync());
    }
}
