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

namespace ERPWMS.Web.Controllers.Sales
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly AppDbContext _context;

        public SalesController(AppDbContext c)
        {
            _context = c;
        }

        private int UserId
        {
            get
            {
                var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(cl, out int id) && id > 0) return id;
                var u = _context.Users.FirstOrDefault();
                return u != null ? u.Id : 1;
            }
        }

        [HttpGet]
        public IActionResult Index() => View();

        // -------------------------------------------------------
        // STATISTICS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var list = await _context.SalesMasters.Where(s => s.IsDeleted != true).ToListAsync();
                var now = DateTime.Now;
                return Json(new
                {
                    total = list.Count,
                    active = list.Count(s => s.Status == "Approved" || s.Status == "Completed"),
                    inactive = list.Count(s => s.Status == "Pending" || s.Status == "Draft"),
                    addedThisMonth = list.Count(s =>
                        s.CreatedDate.HasValue &&
                        s.CreatedDate.Value.Month == now.Month &&
                        s.CreatedDate.Value.Year == now.Year)
                });
            }
            catch
            {
                return Json(new { total = 0, active = 0, inactive = 0, addedThisMonth = 0 });
            }
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await (from s in _context.SalesMasters
                                  join c in _context.Customers on s.CustomerId equals c.CustomerId into cj
                                  from c in cj.DefaultIfEmpty()
                                  join w in _context.Warehouses on s.WarehouseId equals w.WarehouseId into wj
                                  from w in wj.DefaultIfEmpty()
                                  where s.IsDeleted != true
                                  orderby s.SalesId
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
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // -------------------------------------------------------
        // DROPDOWNS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var l = await _context.Customers
                .Where(c => c.IsActive == true && c.IsDeleted != true)
                .OrderBy(c => c.CustomerName)
                .Select(c => new { customerId = c.CustomerId, customerName = c.CustomerName })
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

        // -------------------------------------------------------
        // AUTO-GENERATE SO NUMBER
        // -------------------------------------------------------
        private async Task<string> GenerateNextSONo()
        {
            try
            {
                var year = DateTime.Now.Year;
                var prefix = "SO-" + year + "-";
                var existing = await _context.SalesMasters
                    .Where(s => s.SalesOrderNo != null && s.SalesOrderNo.StartsWith(prefix))
                    .Select(s => s.SalesOrderNo)
                    .ToListAsync();

                int maxNum = 0;
                foreach (var no in existing)
                {
                    if (string.IsNullOrEmpty(no)) continue;
                    var parts = no.Split('-');
                    if (parts.Length >= 3 && int.TryParse(parts[2], out int n) && n > maxNum)
                        maxNum = n;
                }
                return prefix + (maxNum + 1).ToString("D3");
            }
            catch
            {
                return "SO-" + DateTime.Now.Year + "-" + DateTime.Now.ToString("HHmmss");
            }
        }

        // -------------------------------------------------------
        // ADD PARTIAL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> AddPartial()
        {
            ViewBag.NextSONo = await GenerateNextSONo();

            ViewBag.Customers = await _context.Customers
                .Where(c => c.IsActive == true && c.IsDeleted != true)
                .OrderBy(c => c.CustomerName)
                .Select(c => new { customerId = c.CustomerId, customerName = c.CustomerName })
                .ToListAsync();

            ViewBag.Warehouses = await _context.Warehouses
                .Where(w => w.IsActive == true && w.IsDeleted != true)
                .OrderBy(w => w.WarehouseName)
                .Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName })
                .ToListAsync();

            ViewBag.Products = await _context.Products
                .Where(p => p.IsActive == true && p.IsDeleted != true)
                .OrderBy(p => p.ProductName)
                .Select(p => new { productId = p.ProductId, productName = p.ProductName })
                .ToListAsync();

            return PartialView("_AddForm");
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register()
        {
            try
            {
                var f = Request.Form;

                if (!int.TryParse(f["CustomerId"], out int custId) || custId <= 0)
                    return Json(new { success = false, message = "Please select Customer." });
                if (!int.TryParse(f["WarehouseId"], out int whId) || whId <= 0)
                    return Json(new { success = false, message = "Please select Warehouse." });

                var soNo = (f["SalesOrderNo"].ToString() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(soNo) || await _context.SalesMasters.AnyAsync(s => s.SalesOrderNo == soNo))
                    soNo = await GenerateNextSONo();
                if (await _context.SalesMasters.AnyAsync(s => s.SalesOrderNo == soNo))
                    soNo = "SO-" + DateTime.Now.Year + "-" + DateTime.Now.ToString("HHmmssfff");

                var orderDate = DateTime.TryParse(f["OrderDate"], out var od) ? od : DateTime.Now;
                DateOnly? deliveryDate = null;
                if (DateOnly.TryParse(f["DeliveryDate"], out var dd)) deliveryDate = dd;

                decimal.TryParse(f["Discount"], out decimal disc);
                decimal.TryParse(f["TaxAmount"], out decimal tax);

                var details = new List<SalesDetail>();
                decimal totalAmount = 0;

                for (int i = 0; ; i++)
                {
                    var pidStr = f["SalesDetails[" + i + "].ProductId"].ToString();
                    if (string.IsNullOrEmpty(pidStr)) break;
                    if (!int.TryParse(pidStr, out int pid) || pid <= 0) continue;

                    int.TryParse(f["SalesDetails[" + i + "].Quantity"].ToString(), out int qty);
                    decimal.TryParse(f["SalesDetails[" + i + "].UnitPrice"].ToString(), out decimal price);
                    decimal.TryParse(f["SalesDetails[" + i + "].DiscountPercent"].ToString(), out decimal discPct);
                    if (qty <= 0) continue;

                    var batchNo = f["SalesDetails[" + i + "].BatchNo"].ToString();

                    decimal lineTotal = qty * price;
                    decimal lineDisc = lineTotal * (discPct / 100m);
                    decimal netLine = lineTotal - lineDisc;

                    var d = new SalesDetail
                    {
                        ProductId = pid,
                        Quantity = qty,
                        UnitPrice = price,
                        DiscountPercent = discPct,
                        TotalPrice = netLine,
                        BatchNo = batchNo,
                        CreatedBy = UserId.ToString(),
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    details.Add(d);
                    totalAmount += netLine;
                }

                if (details.Count == 0)
                    return Json(new { success = false, message = "At least one item required." });

                var netAmount = totalAmount - disc + tax;

                var so = new SalesMaster
                {
                    SalesOrderNo = soNo,
                    CustomerId = custId,
                    WarehouseId = whId,
                    OrderDate = orderDate,
                    DeliveryDate = deliveryDate,
                    TotalAmount = totalAmount,
                    Discount = disc,
                    TaxAmount = tax,
                    NetAmount = netAmount,
                    Status = "Draft",
                    ApprovalStatus = "Pending",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = UserId.ToString(),
                    CreatedDate = DateTime.Now
                };

                _context.SalesMasters.Add(so);
                await _context.SaveChangesAsync();

                foreach (var d in details)
                {
                    d.SalesId = so.SalesId;
                    _context.SalesDetails.Add(d);
                }
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Sales Order created: " + so.SalesOrderNo, id = so.SalesId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // -------------------------------------------------------
        // EDIT
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit()
        {
            try
            {
                var f = Request.Form;
                if (!int.TryParse(f["SalesId"], out int id) || id <= 0)
                    return Json(new { success = false, message = "Invalid ID." });

                var so = await _context.SalesMasters
                    .Include(s => s.SalesDetails)
                    .FirstOrDefaultAsync(s => s.SalesId == id);
                if (so == null)
                    return Json(new { success = false, message = "Not found." });

                if (int.TryParse(f["CustomerId"], out int cid)) so.CustomerId = cid;
                if (int.TryParse(f["WarehouseId"], out int wid)) so.WarehouseId = wid;
                if (DateTime.TryParse(f["OrderDate"], out var od)) so.OrderDate = od;
                if (DateOnly.TryParse(f["DeliveryDate"], out var dd)) so.DeliveryDate = dd;
                if (decimal.TryParse(f["Discount"], out decimal disc)) so.Discount = disc;
                if (decimal.TryParse(f["TaxAmount"], out decimal tax)) so.TaxAmount = tax;

                var st = f["Status"].ToString();
                if (!string.IsNullOrEmpty(st)) so.Status = st;

                if (so.SalesDetails != null)
                {
                    foreach (var d in so.SalesDetails)
                        d.IsDeleted = true;
                }

                var newDetails = new List<SalesDetail>();
                decimal total = 0;

                for (int i = 0; ; i++)
                {
                    var pidStr = f["SalesDetails[" + i + "].ProductId"].ToString();
                    if (string.IsNullOrEmpty(pidStr)) break;
                    if (!int.TryParse(pidStr, out int pid) || pid <= 0) continue;

                    int.TryParse(f["SalesDetails[" + i + "].Quantity"].ToString(), out int qty);
                    decimal.TryParse(f["SalesDetails[" + i + "].UnitPrice"].ToString(), out decimal price);
                    decimal.TryParse(f["SalesDetails[" + i + "].DiscountPercent"].ToString(), out decimal dp);
                    if (qty <= 0) continue;

                    decimal lineTotal = qty * price;
                    decimal netLine = lineTotal - (lineTotal * dp / 100m);

                    newDetails.Add(new SalesDetail
                    {
                        SalesId = so.SalesId,
                        ProductId = pid,
                        Quantity = qty,
                        UnitPrice = price,
                        DiscountPercent = dp,
                        TotalPrice = netLine,
                        BatchNo = f["SalesDetails[" + i + "].BatchNo"].ToString(),
                        CreatedBy = UserId.ToString(),
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    });
                    total += netLine;
                }

                so.TotalAmount = total;
                so.NetAmount = total - (so.Discount ?? 0) + (so.TaxAmount ?? 0);   // ✅ decimal?
                so.UpdatedBy = UserId.ToString();
                so.UpdatedDate = DateTime.Now;

                if (newDetails.Count > 0)
                    _context.SalesDetails.AddRange(newDetails);

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Sales Order updated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // -------------------------------------------------------
        // TOGGLE
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            try
            {
                var so = await _context.SalesMasters.FindAsync(id);
                if (so == null)
                    return Json(new { success = false, message = "Not found." });

                string old = so.Status ?? "Draft";
                if (old == "Draft" || old == "Pending")
                    so.Status = "Completed";
                else
                    so.Status = "Pending";

                so.UpdatedBy = UserId.ToString();
                so.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + old + " -> " + so.Status });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // -------------------------------------------------------
        // DELETE
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var so = await _context.SalesMasters
                    .Include(s => s.SalesDetails)
                    .FirstOrDefaultAsync(s => s.SalesId == id);
                if (so == null)
                    return Json(new { success = false, message = "Not found." });

                if (so.SalesDetails != null)
                {
                    foreach (var d in so.SalesDetails)
                        d.IsDeleted = true;
                }

                so.IsDeleted = true;
                so.DeletedBy = UserId.ToString();
                so.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Sales Order deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------------------------------------------------------
        // PROFILE
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var so = await _context.SalesMasters
                .Include(s => s.SalesDetails)
                .FirstOrDefaultAsync(s => s.SalesId == id && s.IsDeleted != true);
            if (so == null) return NotFound();

            var details = await _context.SalesDetails
                .Include(d => d.Product)
                .Where(d => d.SalesId == id && (d.IsDeleted == null || d.IsDeleted == false))
                .ToListAsync();
            ViewBag.Details = details;

            var cust = await _context.Customers.FindAsync(so.CustomerId);
            ViewBag.CustomerName = cust != null ? cust.CustomerName : "-";
            ViewBag.CustomerPhone = cust != null ? cust.Phone : "-";

            var wh = await _context.Warehouses.FindAsync(so.WarehouseId);
            ViewBag.WarehouseName = wh != null ? wh.WarehouseName : "-";

            return View("Profile", so);
        }

        // -------------------------------------------------------
        // GET PRODUCT STOCK
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetProductStock(int productId, int? warehouseId)
        {
            try
            {
                if (productId <= 0)
                    return Json(new { success = false, message = "Invalid Product" });

                var q = _context.StockLedgers
                    .Where(s => s.ProductId == productId && (s.IsDeleted == null || s.IsDeleted == false));

                if (warehouseId.HasValue && warehouseId > 0)
                    q = q.Where(s => s.WarehouseId == warehouseId);

                var ledger = await q.ToListAsync();

                int totalIn = 0, totalOut = 0;
                foreach (var l in ledger)
                {
                    var tt = (l.TransactionType ?? "").ToUpper();
                    if (tt == "IN") totalIn += l.Quantity;
                    else if (tt == "OUT") totalOut += l.Quantity;
                }

                int available = totalIn - totalOut;

                int reserved = 0;
                try
                {
                    reserved = await (from sd in _context.SalesDetails
                                      join sm in _context.SalesMasters on sd.SalesId equals sm.SalesId
                                      where sd.ProductId == productId
                                         && sd.IsDeleted != true
                                         && sm.Status != "Completed"
                                         && sm.Status != "Cancelled"
                                         && sm.IsDeleted != true
                                      select sd.Quantity).SumAsync();
                }
                catch { }

                var product = await _context.Products.FindAsync(productId);

                return Json(new
                {
                    success = true,
                    productId = productId,
                    productName = product != null ? product.ProductName : "Product",
                    totalIn = totalIn,
                    totalOut = totalOut,
                    available = available,
                    reserved = reserved,
                    status = available <= 0 ? "OUT_OF_STOCK"
                           : (available <= 5 ? "LOW_STOCK" : "IN_STOCK")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------------------------------------------------------
        // EDIT PARTIAL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            try
            {
                var so = await _context.SalesMasters
                    .Include(s => s.SalesDetails)
                    .FirstOrDefaultAsync(s => s.SalesId == id && s.IsDeleted != true);
                if (so == null) return NotFound();

                ViewBag.SO = so;

                var details = await _context.SalesDetails
                    .Where(d => d.SalesId == id && (d.IsDeleted == null || d.IsDeleted == false))
                    .ToListAsync();
                ViewBag.Details = details;

                ViewBag.Customers = await _context.Customers
                    .Where(c => c.IsActive == true && c.IsDeleted != true)
                    .OrderBy(c => c.CustomerName)
                    .Select(c => new { customerId = c.CustomerId, customerName = c.CustomerName })
                    .ToListAsync();

                ViewBag.Warehouses = await _context.Warehouses
                    .Where(w => w.IsActive == true && w.IsDeleted != true)
                    .OrderBy(w => w.WarehouseName)
                    .Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName })
                    .ToListAsync();

                ViewBag.Products = await _context.Products
                    .Where(p => p.IsActive == true && p.IsDeleted != true)
                    .OrderBy(p => p.ProductName)
                    .Select(p => new { productId = p.ProductId, productName = p.ProductName })
                    .ToListAsync();

                return PartialView("_EditForm", so);
            }
            catch (Exception ex)
            {
                return Content("<div class='text-center py-4 text-danger'>Error: " + ex.Message + "</div>");
            }
        }
    }
}