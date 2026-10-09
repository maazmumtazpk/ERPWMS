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

namespace ERPWMS.Web.Controllers.Procurement
{
    [Authorize]
    public class GrnController : Controller
    {
        private readonly AppDbContext _context;

        public GrnController(AppDbContext c)
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

        // ---------------------------------------------------
        // INDEX
        // ---------------------------------------------------
        [HttpGet]
        public IActionResult Index() => View();

        // ---------------------------------------------------
        // GET STATISTICS
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var list = await _context.Grns.Where(g => g.IsDeleted != true).ToListAsync();
                var now = DateTime.Now;
                return Json(new
                {
                    total = list.Count,
                    active = list.Count(g => g.Status == "Approved" || g.Status == "Completed"),
                    inactive = list.Count(g => g.Status == "Pending"),
                    addedThisMonth = list.Count(g =>
                        g.CreatedDate.HasValue &&
                        g.CreatedDate.Value.Month == now.Month &&
                        g.CreatedDate.Value.Year == now.Year)
                });
            }
            catch
            {
                return Json(new { total = 0, active = 0, inactive = 0, addedThisMonth = 0 });
            }
        }

        // ---------------------------------------------------
        // GET ALL (JSON for table)
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await (from g in _context.Grns
                                  join p in _context.PurchaseMasters on g.PurchaseId equals p.PurchaseId into pj
                                  from p in pj.DefaultIfEmpty()
                                  join w in _context.Warehouses on g.WarehouseId equals w.WarehouseId into wj
                                  from w in wj.DefaultIfEmpty()
                                  where g.IsDeleted != true
                                  orderby g.GrnId
                                  select new
                                  {
                                      grnId = g.GrnId,
                                      grnNo = g.GrnNo,
                                      purchaseId = g.PurchaseId,
                                      purchaseOrderNo = p != null ? p.PurchaseOrderNo : "-",
                                      warehouseId = g.WarehouseId,
                                      warehouseName = w != null ? w.WarehouseName : "-",
                                      receiptDate = g.ReceiptDate,
                                      totalReceived = g.TotalReceivedQty,
                                      totalAccepted = g.TotalAcceptedQty,
                                      totalRejected = g.TotalRejectedQty ?? 0,   // ✅ int?
                                      status = g.Status ?? "Pending"
                                  }).ToListAsync();

                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // ---------------------------------------------------
        // GET DROPDOWNS
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetPurchases()
        {
            var l = await _context.PurchaseMasters
                .Where(p => p.IsDeleted != true)
                .OrderByDescending(p => p.PurchaseId)
                .Select(p => new { purchaseId = p.PurchaseId, purchaseOrderNo = p.PurchaseOrderNo })
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
        public async Task<IActionResult> GetBins()
        {
            var l = await _context.Bins
                .Where(b => b.IsDeleted != true && b.IsActive == true)
                .OrderBy(b => b.BinName)
                .Select(b => new { binId = b.BinId, binName = b.BinName, binCode = b.BinCode ?? "" })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchaseItems(int purchaseId)
        {
            try
            {
                var items = await _context.PurchaseDetails
                    .Include(d => d.Product)
                    .Where(d => d.PurchaseId == purchaseId)
                    .Select(d => new
                    {
                        purchaseDetailId = d.DetailId,
                        productId = d.ProductId,
                        productName = d.Product != null ? d.Product.ProductName : ("Product #" + d.ProductId),
                        quantity = d.Quantity,
                        unitPrice = d.UnitPrice
                    })
                    .ToListAsync();
                return Json(items);
            }
            catch
            {
                return Json(new List<object>());
            }
        }

        // ---------------------------------------------------
        // GENERATE NEXT GRN NUMBER
        // ---------------------------------------------------
        private async Task<string> GenerateNextGrnNo()
        {
            try
            {
                var year = DateTime.Now.Year;
                var prefix = "Grn-" + year + "-";

                var lastGrn = await _context.Grns
                    .Where(g => g.GrnNo != null && g.GrnNo.StartsWith(prefix))
                    .OrderByDescending(g => g.GrnId)
                    .Select(g => g.GrnNo)
                    .FirstOrDefaultAsync();

                int nextNum = 1;
                if (!string.IsNullOrEmpty(lastGrn))
                {
                    var parts = lastGrn.Split('-');
                    if (parts.Length >= 3 && int.TryParse(parts[2], out int n))
                        nextNum = n + 1;
                }
                return prefix + nextNum.ToString("D3");
            }
            catch
            {
                return "Grn-" + DateTime.Now.Year + "-001";
            }
        }

        // ---------------------------------------------------
        // ADD PARTIAL
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> AddPartial()
        {
            ViewBag.NextGrnNo = await GenerateNextGrnNo();
            ViewBag.Purchases = await _context.PurchaseMasters
                .Where(p => p.IsDeleted != true)
                .OrderByDescending(p => p.PurchaseId)
                .Select(p => new { purchaseId = p.PurchaseId, purchaseOrderNo = p.PurchaseOrderNo })
                .ToListAsync();
            ViewBag.Warehouses = await _context.Warehouses
                .Where(w => w.IsActive == true && w.IsDeleted != true)
                .OrderBy(w => w.WarehouseName)
                .Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName })
                .ToListAsync();
            ViewBag.Bins = await _context.Bins
                .Where(b => b.IsDeleted != true && b.IsActive == true)
                .OrderBy(b => b.BinName)
                .Select(b => new { binId = b.BinId, binName = b.BinName, binCode = b.BinCode ?? "" })
                .ToListAsync();

            return PartialView("_AddForm");
        }

        // ---------------------------------------------------
        // EDIT PARTIAL
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var grn = await _context.Grns
                .Include(g => g.Grndetails)
                .FirstOrDefaultAsync(g => g.GrnId == id && g.IsDeleted != true);

            if (grn == null) return NotFound();

            ViewBag.Grn = grn;
            ViewBag.Purchases = await _context.PurchaseMasters
                .Where(p => p.IsDeleted != true)
                .OrderByDescending(p => p.PurchaseId)
                .Select(p => new { purchaseId = p.PurchaseId, purchaseOrderNo = p.PurchaseOrderNo })
                .ToListAsync();
            ViewBag.Warehouses = await _context.Warehouses
                .Where(w => w.IsActive == true && w.IsDeleted != true)
                .OrderBy(w => w.WarehouseName)
                .Select(w => new { warehouseId = w.WarehouseId, warehouseName = w.WarehouseName })
                .ToListAsync();

            return PartialView("_EditForm", grn);
        }

        // ---------------------------------------------------
        // REGISTER (AJAX)
        // ---------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register()
        {
            try
            {
                var f = Request.Form;

                if (!int.TryParse(f["PurchaseId"], out int purchaseId) || purchaseId <= 0)
                    return Json(new { success = false, message = "Please select Purchase Order." });

                if (!int.TryParse(f["WarehouseId"], out int warehouseId) || warehouseId <= 0)
                    return Json(new { success = false, message = "Please select Warehouse." });

                var grnNo = (f["GrnNo"].ToString() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(grnNo))
                    grnNo = await GenerateNextGrnNo();

                if (await _context.Grns.AnyAsync(g => g.GrnNo == grnNo && g.IsDeleted != true))
                    return Json(new { success = false, message = "Grn Number already exists." });

                var receiptDate = DateTime.TryParse(f["ReceiptDate"], out var rd) ? rd : DateTime.Now;

                var details = new List<Grndetail>();
                int totalRec = 0, totalAcc = 0, totalRej = 0;

                for (int i = 0; ; i++)
                {
                    var pidStr = f["Grndetails[" + i + "].ProductId"].ToString();
                    if (string.IsNullOrEmpty(pidStr)) break;
                    if (!int.TryParse(pidStr, out int pid) || pid <= 0) continue;

                    int.TryParse(f["Grndetails[" + i + "].ReceivedQty"], out int received);
                    int.TryParse(f["Grndetails[" + i + "].AcceptedQty"], out int accepted);
                    int.TryParse(f["Grndetails[" + i + "].RejectedQty"], out int rejected);
                    int.TryParse(f["Grndetails[" + i + "].BinId"], out int binId);

                    if (received <= 0) continue;

                    var batchNo = f["Grndetails[" + i + "].BatchNo"].ToString();
                    var rejectionReason = f["Grndetails[" + i + "].RejectionReason"].ToString();

                    DateOnly? mfgDate = null;
                    DateOnly? expDate = null;
                    if (DateOnly.TryParse(f["Grndetails[" + i + "].MfgDate"], out var md)) mfgDate = md;
                    if (DateOnly.TryParse(f["Grndetails[" + i + "].ExpiryDate"], out var ed)) expDate = ed;

                    var detail = new Grndetail
                    {
                        ProductId = pid,
                        ReceivedQty = received,
                        AcceptedQty = accepted,
                        RejectedQty = rejected,
                        BinId = binId,
                        BatchNo = batchNo,
                        RejectionReason = rejectionReason,
                        MfgDate = mfgDate,
                        ExpiryDate = expDate,
                        CreatedBy = UserId.ToString(),
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };

                    var t = detail.GetType();
                    if (t.GetProperty("PurchaseDetailId") != null)
                        t.GetProperty("PurchaseDetailId")?.SetValue(detail, pid);

                    details.Add(detail);
                    totalRec += received;
                    totalAcc += accepted;
                    totalRej += rejected;
                }

                if (details.Count == 0)
                    return Json(new { success = false, message = "At least one item required." });

                var grn = new Grn
                {
                    GrnNo = grnNo,
                    PurchaseId = purchaseId,
                    WarehouseId = warehouseId,
                    ReceiptDate = receiptDate,
                    TotalReceivedQty = totalRec,
                    TotalAcceptedQty = totalAcc,
                    TotalRejectedQty = totalRej,              // ✅ int?
                    Status = "Pending",
                    IsDeleted = false,
                    CreatedBy = UserId.ToString(),
                    CreatedDate = DateTime.Now
                };

                _context.Grns.Add(grn);
                await _context.SaveChangesAsync();

                foreach (var d in details)
                {
                    d.GrnId = grn.GrnId;
                    _context.Grndetails.Add(d);
                }
                await _context.SaveChangesAsync();

                // Add Stock Ledger
                try
                {
                    foreach (var d in details)
                    {
                        if (d.AcceptedQty > 0)
                        {
                            _context.StockLedgers.Add(new StockLedger
                            {
                                ProductId = d.ProductId,
                                WarehouseId = grn.WarehouseId,
                                Quantity = d.AcceptedQty,
                                TransactionType = "IN",
                                ReferenceDocType = "Grn",
                                ReferenceDocNo = grn.GrnNo,
                                ReferenceId = grn.GrnId,
                                CreatedBy = UserId.ToString(),
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch { }

                return Json(new { success = true, message = "Grn created successfully!", grnId = grn.GrnId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // ---------------------------------------------------
        // EDIT (AJAX)
        // ---------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit()
        {
            try
            {
                var f = Request.Form;
                if (!int.TryParse(f["GrnId"], out int id) || id <= 0)
                    return Json(new { success = false, message = "Invalid Grn ID." });

                var grn = await _context.Grns.FindAsync(id);
                if (grn == null)
                    return Json(new { success = false, message = "Grn not found." });

                if (int.TryParse(f["PurchaseId"], out int pid)) grn.PurchaseId = pid;
                if (int.TryParse(f["WarehouseId"], out int wid)) grn.WarehouseId = wid;
                if (DateTime.TryParse(f["ReceiptDate"], out var rd)) grn.ReceiptDate = rd;
                if (int.TryParse(f["TotalReceivedQty"], out int tr)) grn.TotalReceivedQty = tr;
                if (int.TryParse(f["TotalAcceptedQty"], out int ta)) grn.TotalAcceptedQty = ta;
                if (int.TryParse(f["TotalRejectedQty"], out int tj)) grn.TotalRejectedQty = tj;   // ✅ int?

                var newStatus = f["Status"].ToString();
                if (!string.IsNullOrEmpty(newStatus)) grn.Status = newStatus;

                var t = grn.GetType();
                if (t.GetProperty("UpdatedBy") != null) t.GetProperty("UpdatedBy")?.SetValue(grn, UserId);
                if (t.GetProperty("UpdatedDate") != null) t.GetProperty("UpdatedDate")?.SetValue(grn, DateTime.Now);

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Grn updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // ---------------------------------------------------
        // TOGGLE STATUS
        // ---------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            try
            {
                var g = await _context.Grns.FindAsync(id);
                if (g == null)
                    return Json(new { success = false, message = "Grn not found." });

                string oldStatus = g.Status ?? "Pending";

                if (oldStatus == "Pending")
                    g.Status = "Completed";
                else if (oldStatus == "Completed")
                    g.Status = "Pending";
                else if (oldStatus == "Approved")
                    g.Status = "Pending";
                else
                    g.Status = "Pending";

                var t = g.GetType();
                if (t.GetProperty("UpdatedBy") != null) t.GetProperty("UpdatedBy")?.SetValue(g, UserId);
                if (t.GetProperty("UpdatedDate") != null) t.GetProperty("UpdatedDate")?.SetValue(g, DateTime.Now);

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + oldStatus + " -> " + g.Status });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
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
                var grn = await _context.Grns
                    .Include(g => g.Grndetails)
                    .FirstOrDefaultAsync(g => g.GrnId == id);

                if (grn == null)
                    return Json(new { success = false, message = "Grn not found." });

                if (grn.Grndetails != null)
                {
                    foreach (var d in grn.Grndetails)
                    {
                        d.IsDeleted = true;
                    }
                }

                grn.IsDeleted = true;
                var t = grn.GetType();
                if (t.GetProperty("DeletedBy") != null) t.GetProperty("DeletedBy")?.SetValue(grn, UserId);
                if (t.GetProperty("DeletedDate") != null) t.GetProperty("DeletedDate")?.SetValue(grn, DateTime.Now);

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Grn deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
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
                var grn = await _context.Grns
                    .Include(g => g.Grndetails)
                    .FirstOrDefaultAsync(g => g.GrnId == id && g.IsDeleted != true);

                if (grn == null) return NotFound();

                var details = await _context.Grndetails
                    .Where(d => d.GrnId == id && (d.IsDeleted == null || d.IsDeleted == false))
                    .ToListAsync();

                ViewBag.Details = details;

                var po = await _context.PurchaseMasters.FindAsync(grn.PurchaseId);
                ViewBag.PurchaseOrderNo = po != null ? po.PurchaseOrderNo : "-";

                var wh = await _context.Warehouses.FindAsync(grn.WarehouseId);
                ViewBag.WarehouseName = wh != null ? wh.WarehouseName : "-";

                return View("Profile", grn);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Profile");
            }
        }
    }
}