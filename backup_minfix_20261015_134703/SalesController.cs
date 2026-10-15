using ERPWMS.Database;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPWMS.Web.Controllers.Sales
{
    [Authorize(Policy = "Sales.Access")]
    public class SalesController : Controller
    {
        private readonly AppDbContext _context;
        public SalesController(AppDbContext c) { _context = c; }

        private int UserId
        {
            get
            {
                var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(cl, out int id) || id <= 0)
                    throw new UnauthorizedAccessException("User identity missing.");
                return id;
            }
        }

        private const int MaxLineItems = 500;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var now = DateTime.Now;
            var q = _context.SalesMasters.Where(s => s.IsDeleted != true);
            return Json(new
            {
                total = await q.CountAsync(),
                active = await q.CountAsync(s => s.Status == "Approved" || s.Status == "Completed"),
                inactive = await q.CountAsync(s => s.Status == "Pending" || s.Status == "Draft"),
                addedThisMonth = await q.CountAsync(s =>
                    s.CreatedDate.HasValue &&
                    s.CreatedDate.Value.Month == now.Month &&
                    s.CreatedDate.Value.Year == now.Year)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await (from s in _context.SalesMasters
                              join c in _context.Customers on s.CustomerId equals c.CustomerId into cj
                              from c in cj.DefaultIfEmpty()
                              join w in _context.Warehouses on s.WarehouseId equals w.WarehouseId into wj
                              from w in wj.DefaultIfEmpty()
                              where s.IsDeleted != true
                              orderby s.SalesId descending
                              select new
                              {
                                  salesId = s.SalesId,
                                  salesOrderNo = s.SalesOrderNo,
                                  customerId = s.CustomerId,
                                  customerName = c != null ? c.CustomerName : "-",
                                  warehouseId = s.WarehouseId,
                                  warehouseName = w != null ? w.WarehouseName : "-",
                                  orderDate = s.OrderDate,
                                  deliveryDate = s.DeliveryDate,
                                  netAmount = s.NetAmount,
                                  status = s.Status ?? "Draft"
                              }).ToListAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers() =>
            Json(await _context.Customers
                .Where(c => c.IsActive == true && c.IsDeleted != true)
                .OrderBy(c => c.CustomerName)
                .Select(c => new { customerId = c.CustomerId, customerName = c.CustomerName })
                .ToListAsync());

        [HttpGet]
        public async Task<IActionResult> GetWarehouses() =>
            Json(await _context.Warehouses
                .Where(w => w.IsActive == true && w.IsDeleted != true)
                .OrderBy(w => w.WarehouseName)
                .Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName })
                .ToListAsync());

        [HttpGet]
        public async Task<IActionResult> GetProducts() =>
            Json(await _context.Products
                .Where(p => p.IsActive == true && p.IsDeleted != true)
                .OrderBy(p => p.ProductName)
                .Select(p => new { productId = p.ProductId, productName = p.ProductName })
                .ToListAsync());

        private async Task<string> GenerateNextSONo()
        {
            var prefix = "SO-" + DateTime.Now.Year + "-";
            var existing = await _context.SalesMasters
                .Where(s => s.SalesOrderNo != null && s.SalesOrderNo.StartsWith(prefix))
                .Select(s => s.SalesOrderNo).ToListAsync();

            int max = 0;
            foreach (var no in existing)
            {
                if (string.IsNullOrEmpty(no)) continue;
                var parts = no.Split('-');
                if (parts.Length >= 3 && int.TryParse(parts[2], out int n) && n > max) max = n;
            }
            return prefix + (max + 1).ToString("D3");
        }

        [HttpGet]
        public async Task<IActionResult> AddPartial()
        {
            ViewBag.NextSONo = await GenerateNextSONo();
            ViewBag.Customers = await _context.Customers.Where(c => c.IsActive == true && c.IsDeleted != true)
                .OrderBy(c => c.CustomerName).Select(c => new { customerId = c.CustomerId, customerName = c.CustomerName }).ToListAsync();
            ViewBag.Warehouses = await _context.Warehouses.Where(w => w.IsActive == true && w.IsDeleted != true)
                .OrderBy(w => w.WarehouseName).Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName }).ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsActive == true && p.IsDeleted != true)
                .OrderBy(p => p.ProductName).Select(p => new { productId = p.ProductId, productName = p.ProductName }).ToListAsync();
            return PartialView("_AddForm");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register()
        {
            var f = Request.Form;

            if (!int.TryParse(f["CustomerId"], out int custId) || custId <= 0)
                return Json(new { success = false, message = "Please select Customer." });
            if (!int.TryParse(f["WarehouseId"], out int whId) || whId <= 0)
                return Json(new { success = false, message = "Please select Warehouse." });

            var soNo = (f["SalesOrderNo"].ToString() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(soNo)) soNo = await GenerateNextSONo();

            var orderDate = DateTime.TryParse(f["OrderDate"], out var od) ? od : DateTime.Now;
            DateOnly? deliveryDate = DateOnly.TryParse(f["DeliveryDate"], out var dd) ? dd : null;

            if (!decimal.TryParse(f["Discount"], out decimal disc) || disc < 0)
                return Json(new { success = false, message = "Invalid discount." });
            if (!decimal.TryParse(f["TaxAmount"], out decimal tax) || tax < 0)
                return Json(new { success = false, message = "Invalid tax." });

            var details = new List<SalesDetail>();
            decimal total = 0;

            for (int i = 0; i < MaxLineItems; i++)
            {
                var pidStr = f["SalesDetails[" + i + "].ProductId"].ToString();
                if (string.IsNullOrEmpty(pidStr)) break;
                if (!int.TryParse(pidStr, out int pid) || pid <= 0) continue;
                if (!int.TryParse(f["SalesDetails[" + i + "].Quantity"].ToString(), out int qty) || qty <= 0) continue;
                if (!decimal.TryParse(f["SalesDetails[" + i + "].UnitPrice"].ToString(), out decimal price) || price < 0) continue;

                decimal.TryParse(f["SalesDetails[" + i + "].DiscountPercent"].ToString(), out decimal dp);
                if (dp < 0 || dp > 100) dp = 0;

                decimal line = qty * price;
                decimal net = line - (line * dp / 100m);

                details.Add(new SalesDetail
                {
                    ProductId = pid,
                    Quantity = qty,
                    UnitPrice = price,
                    DiscountPercent = dp,
                    TotalPrice = net,
                    BatchNo = f["SalesDetails[" + i + "].BatchNo"].ToString(),
                    CreatedBy = UserId.ToString(),
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                });
                total += net;
            }

            if (details.Count == 0)
                return Json(new { success = false, message = "At least one item required." });

            var so = new SalesMaster
            {
                SalesOrderNo = soNo,
                CustomerId = custId,
                WarehouseId = whId,
                OrderDate = orderDate,
                DeliveryDate = deliveryDate,
                TotalAmount = total,
                Discount = disc,
                TaxAmount = tax,
                NetAmount = total - disc + tax,
                Status = "Draft",
                ApprovalStatus = "Pending",
                IsActive = true,
                IsDeleted = false,
                CreatedBy = UserId.ToString(),
                CreatedDate = DateTime.Now
            };

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.SalesMasters.Add(so);
                await _context.SaveChangesAsync();

                foreach (var d in details) d.SalesId = so.SalesId;
                _context.SalesDetails.AddRange(details);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                return Json(new { success = false, message = "Could not create sales order." });
            }

            return Json(new { success = true, message = "Sales Order created: " + so.SalesOrderNo, id = so.SalesId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Sales.Edit")]
        public async Task<IActionResult> Edit()
        {
            var f = Request.Form;
            if (!int.TryParse(f["SalesId"], out int id) || id <= 0)
                return Json(new { success = false, message = "Invalid ID." });

            var so = await _context.SalesMasters.Include(s => s.SalesDetails)
                .FirstOrDefaultAsync(s => s.SalesId == id && s.IsDeleted != true);
            if (so == null) return Json(new { success = false, message = "Not found." });
            if (so.Status == "Completed" || so.Status == "Cancelled")
                return Json(new { success = false, message = "Completed/Cancelled orders cannot be edited." });

            if (int.TryParse(f["CustomerId"], out int cid) && cid > 0) so.CustomerId = cid;
            if (int.TryParse(f["WarehouseId"], out int wid) && wid > 0) so.WarehouseId = wid;
            if (DateTime.TryParse(f["OrderDate"], out var od)) so.OrderDate = od;
            if (DateOnly.TryParse(f["DeliveryDate"], out var dd)) so.DeliveryDate = dd;

            decimal disc = 0, tax = 0;
            if (!decimal.TryParse(f["Discount"], out disc) || disc < 0)
                return Json(new { success = false, message = "Invalid discount." });
            if (!decimal.TryParse(f["TaxAmount"], out tax) || tax < 0)
                return Json(new { success = false, message = "Invalid tax." });
            so.Discount = disc;
            so.TaxAmount = tax;

            var st = f["Status"].ToString();
            if (!string.IsNullOrEmpty(st)) so.Status = st;

            if (so.SalesDetails != null)
                foreach (var d in so.SalesDetails) d.IsDeleted = true;

            var newDetails = new List<SalesDetail>();
            decimal total = 0;
            for (int i = 0; i < MaxLineItems; i++)
            {
                var pidStr = f["SalesDetails[" + i + "].ProductId"].ToString();
                if (string.IsNullOrEmpty(pidStr)) break;
                if (!int.TryParse(pidStr, out int pid) || pid <= 0) continue;
                if (!int.TryParse(f["SalesDetails[" + i + "].Quantity"].ToString(), out int qty) || qty <= 0) continue;
                if (!decimal.TryParse(f["SalesDetails[" + i + "].UnitPrice"].ToString(), out decimal price) || price < 0) continue;

                decimal.TryParse(f["SalesDetails[" + i + "].DiscountPercent"].ToString(), out decimal dp);
                if (dp < 0 || dp > 100) dp = 0;

                decimal line = qty * price;
                decimal net = line - (line * dp / 100m);
                newDetails.Add(new SalesDetail
                {
                    SalesId = so.SalesId,
                    ProductId = pid,
                    Quantity = qty,
                    UnitPrice = price,
                    DiscountPercent = dp,
                    TotalPrice = net,
                    BatchNo = f["SalesDetails[" + i + "].BatchNo"].ToString(),
                    CreatedBy = UserId.ToString(),
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                });
                total += net;
            }

            so.TotalAmount = total;
            so.NetAmount = total - (so.Discount ?? 0) + (so.TaxAmount ?? 0);
            so.UpdatedBy = UserId.ToString();
            so.UpdatedDate = DateTime.Now;

            if (newDetails.Count > 0) _context.SalesDetails.AddRange(newDetails);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Sales Order updated." });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Sales.Edit")]
        public async Task<IActionResult> Toggle(int id)
        {
            var so = await _context.SalesMasters.FindAsync(id);
            if (so == null || so.IsDeleted == true)
                return Json(new { success = false, message = "Not found." });

            string old = so.Status ?? "Draft";
            so.Status = (old == "Draft" || old == "Pending") ? "Completed" : "Pending";
            so.UpdatedBy = UserId.ToString();
            so.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Status: " + old + " -> " + so.Status });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Sales.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var so = await _context.SalesMasters.Include(s => s.SalesDetails)
                .FirstOrDefaultAsync(s => s.SalesId == id && s.IsDeleted != true);
            if (so == null) return Json(new { success = false, message = "Not found." });

            if (so.SalesDetails != null)
                foreach (var d in so.SalesDetails) d.IsDeleted = true;

            so.IsDeleted = true;
            so.DeletedBy = UserId.ToString();
            so.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Sales Order deleted." });
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var so = await _context.SalesMasters.Include(s => s.SalesDetails)
                .FirstOrDefaultAsync(s => s.SalesId == id && s.IsDeleted != true);
            if (so == null) return NotFound();

            ViewBag.Details = await _context.SalesDetails.Include(d => d.Product)
                .Where(d => d.SalesId == id && (d.IsDeleted == null || d.IsDeleted == false)).ToListAsync();

            var cust = await _context.Customers.FindAsync(so.CustomerId);
            ViewBag.CustomerName = cust?.CustomerName ?? "-";
            ViewBag.CustomerPhone = cust?.Phone ?? "-";

            var wh = await _context.Warehouses.FindAsync(so.WarehouseId);
            ViewBag.WarehouseName = wh?.WarehouseName ?? "-";

            return View("Profile", so);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductStock(int productId, int? warehouseId)
        {
            if (productId <= 0)
                return Json(new { success = false, message = "Invalid Product" });

            var q = _context.StockLedgers.Where(s => s.ProductId == productId && (s.IsDeleted == null || s.IsDeleted == false));
            if (warehouseId.HasValue && warehouseId > 0)
                q = q.Where(s => s.WarehouseId == warehouseId);

            int totalIn = await q.Where(x => x.TransactionType == "IN").SumAsync(x => (int?)x.Quantity) ?? 0;
            int totalOut = await q.Where(x => x.TransactionType == "OUT").SumAsync(x => (int?)x.Quantity) ?? 0;
            int available = totalIn - totalOut;

            int reserved = await (from sd in _context.SalesDetails
                                  join sm in _context.SalesMasters on sd.SalesId equals sm.SalesId
                                  where sd.ProductId == productId
                                     && sd.IsDeleted != true
                                     && sm.Status != "Completed"
                                     && sm.Status != "Cancelled"
                                     && sm.IsDeleted != true
                                  select (int?)sd.Quantity).SumAsync() ?? 0;

            var product = await _context.Products.FindAsync(productId);

            return Json(new
            {
                success = true,
                productId = productId,
                productName = product?.ProductName ?? "Product",
                totalIn, totalOut, available, reserved,
                status = available <= 0 ? "OUT_OF_STOCK" : (available <= 5 ? "LOW_STOCK" : "IN_STOCK")
            });
        }

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var so = await _context.SalesMasters.Include(s => s.SalesDetails)
                .FirstOrDefaultAsync(s => s.SalesId == id && s.IsDeleted != true);
            if (so == null) return NotFound();

            ViewBag.SO = so;
            ViewBag.Details = await _context.SalesDetails
                .Where(d => d.SalesId == id && (d.IsDeleted == null || d.IsDeleted == false)).ToListAsync();
            ViewBag.Customers = await _context.Customers.Where(c => c.IsActive == true && c.IsDeleted != true)
                .OrderBy(c => c.CustomerName).Select(c => new { customerId = c.CustomerId, customerName = c.CustomerName }).ToListAsync();
            ViewBag.Warehouses = await _context.Warehouses.Where(w => w.IsActive == true && w.IsDeleted != true)
                .OrderBy(w => w.WarehouseName).Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName }).ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsActive == true && p.IsDeleted != true)
                .OrderBy(p => p.ProductName).Select(p => new { productId = p.ProductId, productName = p.ProductName }).ToListAsync();
            return PartialView("_EditForm", so);
        }
    }
}
