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

namespace ERPWMS.Web.Controllers
{
    [Authorize]
    public class PickingListController : Controller
    {
        private readonly AppDbContext _context;
        public PickingListController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
        }

        [HttpGet] public IActionResult Index() => View();

        // STATISTICS
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var l = await _context.PickingLists.Where(p => p.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(p => p.Status == "Verified" || p.Status == "Completed"),
                inactive = l.Count(p => p.Status == "Created" || p.Status == "Pending"),
                addedThisMonth = l.Count(p => p.CreatedDate.HasValue
                    && p.CreatedDate.Value.Month == DateTime.Now.Month
                    && p.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.PickingLists
                .Include(p => p.Sales)
                .Include(p => p.Warehouse)
                .Where(p => p.IsDeleted != true)
                .OrderBy(p => p.PickingId)
                .Select(p => new
                {
                    pickingId = p.PickingId,
                    pickingNo = p.PickingNo,
                    salesOrderNo = p.Sales != null ? p.Sales.SalesOrderNo : "-",
                    warehouseName = p.Warehouse != null ? p.Warehouse.WarehouseName : "-",
                    pickingDate = p.PickingDate,
                    status = p.Status ?? "Created"
                })
                .ToListAsync();
            return Json(l);
        }

        // FILTERED: SIRF DELIVERED DISPATCH WALE SALES ORDERS
        [HttpGet]
        public async Task<IActionResult> GetSales()
        {
            try
            {
                var alreadyPickedSoIds = await _context.PickingLists
                    .Where(p => p.IsDeleted == null || p.IsDeleted == false)
                    .Select(p => p.SalesId)
                    .Distinct()
                    .ToListAsync();

                var deliveredSoIds = await _context.DispatchMasters
                    .Where(d => (d.IsDeleted == null || d.IsDeleted == false)
                                && d.Status != null
                                && d.Status.Trim().ToLower() == "delivered")
                    .Select(d => d.SalesId)
                    .Distinct()
                    .ToListAsync();

                var l = await _context.SalesMasters
                    .Where(s => (s.IsDeleted == null || s.IsDeleted == false)
                                && deliveredSoIds.Contains(s.SalesId)
                                && !alreadyPickedSoIds.Contains(s.SalesId))
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
                .Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName })
                .ToListAsync();
            return Json(l);
        }

        // AUTO-FILL
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

                private async Task<string> GenerateNextPickingNo()
        {
            try
            {
                var year = DateTime.Now.Year;
                var prefix = "PICK-" + year + "-";
                var existing = await _context.PickingLists
                    .Where(p => p.PickingNo != null && p.PickingNo.StartsWith(prefix))
                    .Select(p => p.PickingNo)
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
            catch { return "PICK-" + DateTime.Now.Year + "-" + DateTime.Now.ToString("HHmmss"); }
        }

        [HttpGet] public async Task<IActionResult> AddPartial() 
        { 
            ViewBag.NextPickingNo = await GenerateNextPickingNo(); 
            return PartialView("_AddForm"); 
        }

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var p = await _context.PickingLists
                .Include(x => x.PickingDetails)
                .FirstOrDefaultAsync(x => x.PickingId == id);
            if (p == null) return NotFound();
            return PartialView("_EditForm", p);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register()
        {
            try
            {
                var f = Request.Form;
                var pickingNo = (f["pickingNo"].ToString() ?? "").Trim();
                int.TryParse(f["salesId"], out int salesId);
                int.TryParse(f["warehouseId"], out int warehouseId);
                DateTime? pickingDate = DateTime.TryParse(f["pickingDate"], out var pd) ? pd : (DateTime?)null;

                if (string.IsNullOrWhiteSpace(pickingNo)) pickingNo = await GenerateNextPickingNo();
                if (salesId <= 0) return Json(new { success = false, message = "Please select Sales Order." });

                if (await _context.PickingLists.AnyAsync(p => p.SalesId == salesId && p.IsDeleted != true))
                    return Json(new { success = false, message = "Is Sales Order ki Picking List pehle se mojood hai!" });

                var picking = new PickingList
                {
                    PickingNo = pickingNo,
                    SalesId = salesId,
                    WarehouseId = warehouseId,
                    PickingDate = pickingDate ?? DateTime.Now,
                    Status = "Created",
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                };

                _context.PickingLists.Add(picking);
                await _context.SaveChangesAsync();

                // Save Items
                var details = new List<PickingDetail>();
                for (int i = 0; ; i++)
                {
                    var pidStr = f["PickingDetails[" + i + "].ProductId"].ToString();
                    if (string.IsNullOrEmpty(pidStr)) break;
                    if (!int.TryParse(pidStr, out int pid) || pid <= 0) continue;

                    int.TryParse(f["PickingDetails[" + i + "].SalesDetailId"].ToString(), out int sdid);
                    int.TryParse(f["PickingDetails[" + i + "].PickedQty"].ToString(), out int pickedQty);
                    if (pickedQty <= 0) continue;

                    details.Add(new PickingDetail
                    {
                        PickingId = picking.PickingId,
                        SalesDetailId = sdid,
                        ProductId = pid,
                        PickedQty = pickedQty,
                        BatchNo = f["PickingDetails[" + i + "].BatchNo"].ToString(),
                        SerialNumbers = f["PickingDetails[" + i + "].SerialNumbers"].ToString(),
                        Status = "Pending",
                        CreatedBy = Uid().ToString(),
                        CreatedDate = DateTime.Now
                    });
                }

                if (details.Count > 0)
                {
                    _context.PickingDetails.AddRange(details);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Picking list created with " + details.Count + " items!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int pickingId, string pickingNo, int salesId, int warehouseId, string status)
        {
            try
            {
                var p = await _context.PickingLists.FindAsync(pickingId);
                if (p == null) return Json(new { success = false, message = "Not found" });
                p.PickingNo = pickingNo.Trim();
                p.SalesId = salesId;
                p.WarehouseId = warehouseId;
                p.Status = status;
                p.UpdatedBy = Uid().ToString();
                p.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Picking updated!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var p = await _context.PickingLists.FindAsync(id);
                if (p == null) return Json(new { success = false });
                p.IsDeleted = true;
                p.DeletedBy = Uid().ToString();
                p.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            try
            {
                var p = await _context.PickingLists.FindAsync(id);
                if (p == null) return Json(new { success = false });
                p.Status = p.Status == "Created" ? "Verified" : p.Status == "Verified" ? "Completed" : "Created";
                p.UpdatedBy = Uid().ToString();
                p.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + p.Status });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        // PROFILE
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var p = await _context.PickingLists
                .Include(x => x.PickingDetails)
                .Include(x => x.Sales)
                .Include(x => x.Warehouse)
                .FirstOrDefaultAsync(x => x.PickingId == id && x.IsDeleted != true);
            if (p == null) return NotFound();

            var details = await _context.PickingDetails
                .Include(d => d.Product)
                .Where(d => d.PickingId == id)
                .ToListAsync();
            ViewBag.Details = details;

            var so = p.Sales;
            ViewBag.SalesOrderNo = so != null ? so.SalesOrderNo : "-";
            ViewBag.WarehouseName = p.Warehouse != null ? p.Warehouse.WarehouseName : "-";

            if (so != null)
            {
                var cust = await _context.Customers.FindAsync(so.CustomerId);
                ViewBag.CustomerName = cust != null ? cust.CustomerName : "-";
                ViewBag.CustomerPhone = cust != null ? cust.Phone : "-";
                ViewBag.CustomerAddress = cust != null ? cust.Address : "-";
            }

            return View("Profile", p);
        }
    }
}



