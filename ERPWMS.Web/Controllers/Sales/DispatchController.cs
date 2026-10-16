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
    public class DispatchController : Controller
    {
        private readonly AppDbContext _context;
        public DispatchController(AppDbContext c) { _context = c; }

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

        [HttpGet] public IActionResult Index() => View();

        // --- STATISTICS ---
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var list = await _context.DispatchMasters.Where(d => d.IsDeleted != true).ToListAsync();
                var now = DateTime.Now;
                return Json(new
                {
                    total = list.Count,
                    active = list.Count(d => d.Status == "Delivered" || d.Status == "Completed" || d.Status == "Shipped"),
                    inactive = list.Count(d => d.Status == "Pending" || d.Status == "Draft"),
                    addedThisMonth = list.Count(d => d.CreatedDate.HasValue
                        && d.CreatedDate.Value.Month == now.Month
                        && d.CreatedDate.Value.Year == now.Year)
                });
            }
            catch
            {
                return Json(new { total = 0, active = 0, inactive = 0, addedThisMonth = 0 });
            }
        }

        // --- GET ALL (ascending) ---
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await (from d in _context.DispatchMasters
                                  join s in _context.SalesMasters on d.SalesId equals s.SalesId into sj
                                  from s in sj.DefaultIfEmpty()
                                  join w in _context.Warehouses on d.WarehouseId equals w.WarehouseId into wj
                                  from w in wj.DefaultIfEmpty()
                                  join c in _context.Customers on s.CustomerId equals c.CustomerId into cj
                                  from c in cj.DefaultIfEmpty()
                                  where d.IsDeleted != true
                                  orderby d.DispatchId
                                  select new
                                  {
                                      dispatchId = d.DispatchId,
                                      dispatchNo = d.DispatchNo,
                                      salesId = d.SalesId,
                                      salesOrderNo = s != null ? s.SalesOrderNo : "-",
                                      customerName = c != null ? c.CustomerName : "-",
                                      warehouseName = w != null ? w.WarehouseName : "-",
                                      dispatchDate = d.DispatchDate,
                                      deliveryDate = d.DeliveryDate,
                                      carrierName = d.CarrierName ?? "",
                                      trackingNo = d.TrackingNo ?? "",
                                      status = d.Status ?? "Pending"
                                  }).ToListAsync();
                return Json(data);
            }
            catch (Exception ex) { return Json(new { error = ex.Message }); }
        }

        // --- HELPER: Filtered Sales Orders ---
        // Sirf wohi Sales Orders jo: (1) Delete na hue hon, (2) Jinki Invoice Paid ho, (3) Jinki Dispatch na bani ho
        private async Task<List<int>> GetDispatchedSoIds()
        {
            return await _context.DispatchMasters
                .Where(d => d.IsDeleted == null || d.IsDeleted == false)
                .Select(d => d.SalesId)
                .Distinct()
                .ToListAsync();
        }

        private async Task<List<int>> GetPaidInvoiceSoIds()
        {
            return await _context.Invoices
                .Where(i => (i.IsDeleted == null || i.IsDeleted == false)
                            && i.Status != null
                            && i.Status.Trim().ToLower() == "paid")
                .Select(i => i.SalesId)
                .Distinct()
                .ToListAsync();
        }

        // --- DROPDOWNS ---
        [HttpGet]
        public async Task<IActionResult> GetSales()
        {
            try
            {
                var dispatchedSoIds = await GetDispatchedSoIds();
                var paidInvoiceSoIds = await GetPaidInvoiceSoIds();

                var l = await _context.SalesMasters
                    .Where(s => (s.IsDeleted == null || s.IsDeleted == false)
                                && paidInvoiceSoIds.Contains(s.SalesId)
                                && !dispatchedSoIds.Contains(s.SalesId))
                    .OrderBy(s => s.SalesId)
                    .Select(s => new { salesId = s.SalesId, salesOrderNo = s.SalesOrderNo })
                    .ToListAsync();

                return Json(l);
            }
            catch (Exception ex) { return Json(new { error = ex.Message }); }
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
        public async Task<IActionResult> GetSalesDetails(int salesId)
        {
            try
            {
                var details = await _context.SalesDetails
                    .Include(d => d.Product)
                    .Where(d => d.SalesId == salesId && (d.IsDeleted == null || d.IsDeleted == false))
                    .Select(d => new
                    {
                        salesDetailId = d.DetailId,
                        productId = d.ProductId,
                        productName = d.Product != null ? d.Product.ProductName : ("Product #" + d.ProductId),
                        quantity = d.Quantity,
                        unitPrice = d.UnitPrice,
                        batchNo = d.BatchNo ?? ""
                    })
                    .ToListAsync();
                return Json(details);
            }
            catch { return Json(new List<object>()); }
        }

        // --- AUTO-GENERATE DISPATCH NO ---
        private async Task<string> GenerateNextDispatchNo()
        {
            try
            {
                var year = DateTime.Now.Year;
                var prefix = "DSP-" + year + "-";
                var existing = await _context.DispatchMasters
                    .Where(d => d.DispatchNo != null && d.DispatchNo.StartsWith(prefix))
                    .Select(d => d.DispatchNo)
                    .ToListAsync();

                int maxNum = 0;
                foreach (var no in existing)
                {
                    var parts = no.Split('-');
                    if (parts.Length >= 3 && int.TryParse(parts[2], out int n) && n > maxNum)
                        maxNum = n;
                }
                return prefix + (maxNum + 1).ToString("D3");
            }
            catch { return "DSP-" + DateTime.Now.Year + "-" + DateTime.Now.ToString("HHmmss"); }
        }

        // --- ADD PARTIAL ---
        [HttpGet]
        public async Task<IActionResult> AddPartial()
        {
            ViewBag.NextDispatchNo = await GenerateNextDispatchNo();

            // FILTERED: sirf wohi Sales Orders jinki Invoice Paid hai aur Dispatch nahi bani
            var dispatchedSoIds = await GetDispatchedSoIds();
            var paidInvoiceSoIds = await GetPaidInvoiceSoIds();

            ViewBag.Sales = await _context.SalesMasters
                .Where(s => (s.IsDeleted == null || s.IsDeleted == false)
                            && paidInvoiceSoIds.Contains(s.SalesId)
                            && !dispatchedSoIds.Contains(s.SalesId))
                .OrderBy(s => s.SalesId)
                .Select(s => new { salesId = s.SalesId, salesOrderNo = s.SalesOrderNo })
                .ToListAsync();

            ViewBag.Warehouses = await _context.Warehouses
                .Where(w => w.IsActive == true && w.IsDeleted != true)
                .OrderBy(w => w.WarehouseName)
                .Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName })
                .ToListAsync();

            return PartialView("_AddForm");
        }

        // --- EDIT PARTIAL ---
        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var dsp = await _context.DispatchMasters
                .Include(d => d.DispatchDetails)
                .FirstOrDefaultAsync(d => d.DispatchId == id && d.IsDeleted != true);
            if (dsp == null) return NotFound();

            ViewBag.Dispatch = dsp;

            // FILTERED + current SalesId bhi include karo (taake edit ke waqt current SO dropdown mein nazar aaye)
            var dispatchedSoIds = await GetDispatchedSoIds();
            var paidInvoiceSoIds = await GetPaidInvoiceSoIds();

            ViewBag.Sales = await _context.SalesMasters
                .Where(s => (s.IsDeleted == null || s.IsDeleted == false)
                            && (s.SalesId == dsp.SalesId || (paidInvoiceSoIds.Contains(s.SalesId) && !dispatchedSoIds.Contains(s.SalesId))))
                .OrderBy(s => s.SalesId)
                .Select(s => new { salesId = s.SalesId, salesOrderNo = s.SalesOrderNo })
                .ToListAsync();

            ViewBag.Warehouses = await _context.Warehouses
                .Where(w => w.IsActive == true && w.IsDeleted != true)
                .OrderBy(w => w.WarehouseName)
                .Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName })
                .ToListAsync();

            return PartialView("_EditForm", dsp);
        }

        // --- REGISTER ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Sales.Create")]
        public async Task<IActionResult> Register()
        {
            try
            {
                var f = Request.Form;

                if (!int.TryParse(f["SalesId"], out int salesId) || salesId <= 0)
                    return Json(new { success = false, message = "Please select Sales Order." });
                if (!int.TryParse(f["WarehouseId"], out int whId) || whId <= 0)
                    return Json(new { success = false, message = "Please select Warehouse." });

                var dspNo = (f["DispatchNo"].ToString() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(dspNo) || await _context.DispatchMasters.AnyAsync(d => d.DispatchNo == dspNo))
                    dspNo = await GenerateNextDispatchNo();
                if (await _context.DispatchMasters.AnyAsync(d => d.DispatchNo == dspNo))
                    dspNo = "DSP-" + DateTime.Now.Year + "-" + DateTime.Now.ToString("HHmmssfff");

                var dispatchDate = DateTime.TryParse(f["DispatchDate"], out var dd) ? dd : DateTime.Now;
                DateTime? deliveryDate = null;
                if (DateTime.TryParse(f["DeliveryDate"], out var dlv)) deliveryDate = dlv;

                var carrierName = f["CarrierName"].ToString() ?? "";
                var trackingNo = f["TrackingNo"].ToString() ?? "";

                var details = new List<DispatchDetail>();
                for (int i = 0; ; i++)
                {
                    var pidStr = f["DispatchDetails[" + i + "].ProductId"].ToString();
                    if (string.IsNullOrEmpty(pidStr)) break;
                    if (!int.TryParse(pidStr, out int pid) || pid <= 0) continue;

                    int.TryParse(f["DispatchDetails[" + i + "].SalesDetailId"].ToString(), out int sdid);
                    int.TryParse(f["DispatchDetails[" + i + "].Quantity"].ToString(), out int qty);
                    if (qty <= 0) continue;

                    details.Add(new DispatchDetail
                    {
                        SalesDetailId = sdid,
                        ProductId = pid,
                        Quantity = qty,
                        BatchNo = f["DispatchDetails[" + i + "].BatchNo"].ToString(),
                        SerialNumbers = f["DispatchDetails[" + i + "].SerialNumbers"].ToString(),
                        CreatedBy = UserId.ToString(),
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    });
                }

                if (details.Count == 0)
                    return Json(new { success = false, message = "At least one item required." });

                var dsp = new DispatchMaster
                {
                    DispatchNo = dspNo,
                    SalesId = salesId,
                    WarehouseId = whId,
                    DispatchDate = dispatchDate,
                    DeliveryDate = deliveryDate,
                    CarrierName = carrierName,
                    TrackingNo = trackingNo,
                    Status = "Pending",
                    IsDeleted = false,
                    CreatedBy = UserId.ToString(),
                    CreatedDate = DateTime.Now
                };

                _context.DispatchMasters.Add(dsp);
                await _context.SaveChangesAsync();

                foreach (var d in details)
                {
                    d.DispatchId = dsp.DispatchId;
                    _context.DispatchDetails.Add(d);
                }
                await _context.SaveChangesAsync();

                // --- STOCK DEDUCT (OUT) ---
                try
                {
                    foreach (var d in details)
                    {
                        var ledger = await _context.StockLedgers
                            .Where(s => s.ProductId == d.ProductId && (s.IsDeleted == null || s.IsDeleted == false))
                            .ToListAsync();

                        int inQty = ledger.Where(s => (s.TransactionType ?? "").ToString() == "IN").Sum(s => s.Quantity);
                        int outQty = ledger.Where(s => (s.TransactionType ?? "").ToString() == "OUT").Sum(s => s.Quantity);
                        int available = inQty - outQty;

                        if (available < d.Quantity)
                        {
                            return Json(new
                            {
                                success = false,
                                message = "Insufficient stock for Product #" + d.ProductId +
                                          ". Available: " + available + ", Requested: " + d.Quantity
                            });
                        }
                    }

                    foreach (var d in details)
                    {
                        _context.StockLedgers.Add(new StockLedger
                        {
                            ProductId = d.ProductId,
                            WarehouseId = dsp.WarehouseId,
                            Quantity = d.Quantity,
                            TransactionType = "OUT",
                            ReferenceDocType = "Dispatch",
                            ReferenceDocNo = dsp.DispatchNo,
                            ReferenceId = dsp.DispatchId,
                            CreatedBy = UserId.ToString(),
                            CreatedDate = DateTime.Now,
                            IsDeleted = false
                        });
                    }
                    await _context.SaveChangesAsync();
                }
                catch { }
                return Json(new { success = true, message = "Dispatch created: " + dsp.DispatchNo, id = dsp.DispatchId });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        // --- EDIT ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Sales.Edit")]
        public async Task<IActionResult> Edit()
        {
            try
            {
                var f = Request.Form;
                if (!int.TryParse(f["DispatchId"], out int id) || id <= 0)
                    return Json(new { success = false, message = "Invalid ID." });

                var dsp = await _context.DispatchMasters
                    .Include(d => d.DispatchDetails)
                    .FirstOrDefaultAsync(d => d.DispatchId == id);
                if (dsp == null) return Json(new { success = false, message = "Not found." });

                if (int.TryParse(f["SalesId"], out int sid)) dsp.SalesId = sid;
                if (int.TryParse(f["WarehouseId"], out int wid)) dsp.WarehouseId = wid;
                if (DateTime.TryParse(f["DispatchDate"], out var dd)) dsp.DispatchDate = dd;
                if (DateTime.TryParse(f["DeliveryDate"], out var dlv)) dsp.DeliveryDate = dlv;

                dsp.CarrierName = f["CarrierName"].ToString() ?? dsp.CarrierName;
                dsp.TrackingNo = f["TrackingNo"].ToString() ?? dsp.TrackingNo;

                var st = f["Status"].ToString();
                if (!string.IsNullOrEmpty(st)) dsp.Status = st;

                if (dsp.DispatchDetails != null)
                    foreach (var d in dsp.DispatchDetails) d.IsDeleted = true;

                var newDetails = new List<DispatchDetail>();
                for (int i = 0; ; i++)
                {
                    var pidStr = f["DispatchDetails[" + i + "].ProductId"].ToString();
                    if (string.IsNullOrEmpty(pidStr)) break;
                    if (!int.TryParse(pidStr, out int pid) || pid <= 0) continue;

                    int.TryParse(f["DispatchDetails[" + i + "].SalesDetailId"].ToString(), out int sdid);
                    int.TryParse(f["DispatchDetails[" + i + "].Quantity"].ToString(), out int qty);
                    if (qty <= 0) continue;

                    newDetails.Add(new DispatchDetail
                    {
                        DispatchId = dsp.DispatchId,
                        SalesDetailId = sdid,
                        ProductId = pid,
                        Quantity = qty,
                        BatchNo = f["DispatchDetails[" + i + "].BatchNo"].ToString(),
                        SerialNumbers = f["DispatchDetails[" + i + "].SerialNumbers"].ToString(),
                        CreatedBy = UserId.ToString(),
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    });
                }

                dsp.UpdatedBy = UserId.ToString();
                dsp.UpdatedDate = DateTime.Now;

                if (newDetails.Count > 0) _context.DispatchDetails.AddRange(newDetails);

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Dispatch updated." });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        // --- TOGGLE ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Sales.Approve")]
        public async Task<IActionResult> Toggle(int id)
        {
            try
            {
                var dsp = await _context.DispatchMasters.FindAsync(id);
                if (dsp == null) return Json(new { success = false, message = "Not found." });

                string old = dsp.Status ?? "Pending";
                if (old == "Pending" || old == "Draft")
                    dsp.Status = "Delivered";
                else
                    dsp.Status = "Pending";

                dsp.UpdatedBy = UserId.ToString();
                dsp.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + old + " ? " + dsp.Status });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        // --- DELETE ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Sales.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var dsp = await _context.DispatchMasters
                    .Include(d => d.DispatchDetails)
                    .FirstOrDefaultAsync(d => d.DispatchId == id);
                if (dsp == null) return Json(new { success = false, message = "Not found." });

                if (dsp.DispatchDetails != null)
                    foreach (var d in dsp.DispatchDetails) d.IsDeleted = true;

                dsp.IsDeleted = true;
                dsp.DeletedBy = UserId.ToString();
                dsp.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Dispatch deleted." });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        // --- PROFILE ---
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var dsp = await _context.DispatchMasters
                .Include(d => d.DispatchDetails)
                .FirstOrDefaultAsync(d => d.DispatchId == id && d.IsDeleted != true);
            if (dsp == null) return NotFound();

            var details = await _context.DispatchDetails
                .Include(d => d.Product)
                .Where(d => d.DispatchId == id && (d.IsDeleted == null || d.IsDeleted == false))
                .ToListAsync();
            ViewBag.Details = details;

            var so = await _context.SalesMasters.FindAsync(dsp.SalesId);
            ViewBag.SalesOrderNo = so != null ? so.SalesOrderNo : "-";

            if (so != null)
            {
                var cust = await _context.Customers.FindAsync(so.CustomerId);
                ViewBag.CustomerName = cust != null ? cust.CustomerName : "-";
                ViewBag.CustomerPhone = cust != null ? cust.Phone : "-";
                ViewBag.CustomerAddress = cust != null ? cust.Address : "-";
            }

            var wh = await _context.Warehouses.FindAsync(dsp.WarehouseId);
            ViewBag.WarehouseName = wh != null ? wh.WarehouseName : "-";

            return View("Profile", dsp);
        }
    }
}


