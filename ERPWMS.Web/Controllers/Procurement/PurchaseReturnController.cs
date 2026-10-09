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
    public class PurchaseReturnController : Controller
    {
        private readonly AppDbContext _context;
        public PurchaseReturnController(AppDbContext c) { _context = c; }

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

        [HttpGet] public IActionResult Index() => View();

        // ---------------------------------------------------
        // STATISTICS
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var list = await _context.PurchaseReturns.Where(r => r.IsDeleted != true).ToListAsync();
                var now = DateTime.Now;
                return Json(new
                {
                    total = list.Count,
                    active = list.Count(r => r.Status == "Approved" || r.Status == "Completed"),
                    inactive = list.Count(r => r.Status == "Pending" || r.Status == "Draft"),
                    addedThisMonth = list.Count(r => r.CreatedDate.HasValue
                        && r.CreatedDate.Value.Month == now.Month
                        && r.CreatedDate.Value.Year == now.Year)
                });
            }
            catch
            {
                return Json(new { total = 0, active = 0, inactive = 0, addedThisMonth = 0 });
            }
        }

        // ---------------------------------------------------
        // GET ALL (JSON, ascending order)
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await (from r in _context.PurchaseReturns
                                  join p in _context.PurchaseMasters on r.PurchaseId equals p.PurchaseId into pj
                                  from p in pj.DefaultIfEmpty()
                                  where r.IsDeleted != true
                                  orderby r.ReturnId
                                  select new
                                  {
                                      returnId = r.ReturnId,
                                      returnNo = r.ReturnNo,
                                      purchaseId = r.PurchaseId,
                                      purchaseOrderNo = p != null ? p.PurchaseOrderNo : "-",
                                      returnDate = r.ReturnDate,
                                      returnReason = r.ReturnReason ?? "",
                                      totalAmount = r.TotalAmount,
                                      status = r.Status ?? "Pending"
                                  }).ToListAsync();
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // ---------------------------------------------------
        // DROPDOWNS
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetPurchases()
        {
            var l = await _context.PurchaseMasters
                .Where(p => p.IsDeleted != true)
                .OrderBy(p => p.PurchaseId)
                .Select(p => new { purchaseId = p.PurchaseId, purchaseOrderNo = p.PurchaseOrderNo })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetGrns(int? purchaseId)
        {
            var q = _context.Grns.Where(g => g.IsDeleted != true);
            if (purchaseId.HasValue && purchaseId > 0)
                q = q.Where(g => g.PurchaseId == purchaseId);

            var l = await q.OrderBy(g => g.GrnId)
                .Select(g => new { grnId = g.GrnId, grnNo = g.GrnNo })
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
                        detailId = d.DetailId,
                        productId = d.ProductId,
                        productName = d.Product != null ? d.Product.ProductName : ("Product #" + d.ProductId),
                        quantity = d.Quantity,
                        unitPrice = d.UnitPrice
                    })
                    .ToListAsync();
                return Json(items);
            }
            catch { return Json(new List<object>()); }
        }

        // ---------------------------------------------------
        // AUTO-GENERATE RETURN NO
        // ---------------------------------------------------
        private async Task<string> GenerateNextReturnNo()
        {
            try
            {
                var year = DateTime.Now.Year;
                var prefix = "PR-" + year + "-";

                // Get ALL existing Return Nos for this year (from DB)
                var existing = await _context.PurchaseReturns
                    .Where(r => r.ReturnNo != null && r.ReturnNo.StartsWith(prefix))
                    .Select(r => r.ReturnNo)
                    .ToListAsync();

                // Find highest numeric value
                int maxNum = 0;
                foreach (var no in existing)
                {
                    if (string.IsNullOrEmpty(no)) continue;
                    var parts = no.Split('-');
                    if (parts.Length >= 3)
                    {
                        var numPart = parts[2];
                        if (int.TryParse(numPart, out int n) && n > maxNum)
                            maxNum = n;
                    }
                }

                // Return next available number
                return prefix + (maxNum + 1).ToString("D3");
            }
            catch
            {
                // Fallback � timestamp
                return "PR-" + DateTime.Now.Year + "-" + DateTime.Now.ToString("HHmmss");
            }
        }

        // ---------------------------------------------------
        // ADD PARTIAL
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> AddPartial()
        {
            // --- PEEK DB for next available Return No ---
            ViewBag.NextReturnNo = await GenerateNextReturnNo();
            
            ViewBag.Purchases = await _context.PurchaseMasters
                .Where(p => p.IsDeleted != true)
                .OrderBy(p => p.PurchaseId)
                .Select(p => new { purchaseId = p.PurchaseId, purchaseOrderNo = p.PurchaseOrderNo })
                .ToListAsync();
            
            return PartialView("_AddForm");
        }

        // ---------------------------------------------------
        // EDIT PARTIAL
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var pr = await _context.PurchaseReturns
                .Include(r => r.PurchaseReturnDetails)
                .FirstOrDefaultAsync(r => r.ReturnId == id && r.IsDeleted != true);

            if (pr == null) return NotFound();

            ViewBag.PR = pr;
            ViewBag.Purchases = await _context.PurchaseMasters
                .Where(p => p.IsDeleted != true)
                .OrderBy(p => p.PurchaseId)
                .Select(p => new { purchaseId = p.PurchaseId, purchaseOrderNo = p.PurchaseOrderNo })
                .ToListAsync();
            ViewBag.Grns = await _context.Grns
                .Where(g => g.IsDeleted != true)
                .OrderBy(g => g.GrnId)
                .Select(g => new { grnId = g.GrnId, grnNo = g.GrnNo })
                .ToListAsync();

            return PartialView("_EditForm", pr);
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

                var returnDate = DateTime.TryParse(f["ReturnDate"], out var rd) ? rd : DateTime.Now;
                int? grnId = null;
                if (int.TryParse(f["GrnId"], out int gid) && gid > 0) grnId = gid;
                var returnReason = f["ReturnReason"].ToString() ?? "";

                // --- Parse details ---
                var details = new List<PurchaseReturnDetail>();
                decimal totalAmount = 0;

                // Pre-load valid PurchaseDetailIds for this PO
                var validPurchaseDetails = await _context.PurchaseDetails
                    .Where(pd => pd.PurchaseId == purchaseId)
                    .ToListAsync();

                for (int i = 0; ; i++)
                {
                    var pidStr = f["ReturnDetails[" + i + "].ProductId"].ToString();
                    if (string.IsNullOrEmpty(pidStr)) break;
                    if (!int.TryParse(pidStr, out int pid) || pid <= 0) continue;

                    int.TryParse(f["ReturnDetails[" + i + "].PurchaseDetailId"].ToString(), out int pdid);
                    int.TryParse(f["ReturnDetails[" + i + "].Quantity"].ToString(), out int qty);
                    decimal.TryParse(f["ReturnDetails[" + i + "].UnitPrice"].ToString(), out decimal price);
                    if (qty <= 0) continue;

                    // --- FIX: Validate PurchaseDetailId against DB ---
                    var matchedPd = validPurchaseDetails.FirstOrDefault(x => x.DetailId == pdid);
                    
                    if (matchedPd == null)
                    {
                        // Fallback: find by ProductId
                        matchedPd = validPurchaseDetails.FirstOrDefault(x => x.ProductId == pid);
                    }
                    
                    int finalPdid = matchedPd != null ? matchedPd.DetailId : 0;

                    var batchNo = f["ReturnDetails[" + i + "].BatchNo"].ToString();
                    DateOnly? expDate = null;
                    if (DateOnly.TryParse(f["ReturnDetails[" + i + "].ExpiryDate"], out var ed)) expDate = ed;

                    var d = new PurchaseReturnDetail
                    {
                        PurchaseDetailId = finalPdid,
                        ProductId = pid,
                        Quantity = qty,
                        UnitPrice = price,
                        TotalPrice = qty * price,
                        BatchNo = batchNo,
                        ExpiryDate = expDate,
                        CreatedBy = UserId.ToString(),
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    details.Add(d);
                    totalAmount += d.TotalPrice;
                }

                if (details.Count == 0)
                    return Json(new { success = false, message = "At least one item required." });

                // --- Return No � Use client value if free, else auto-generate ---
                var returnNo = (f["ReturnNo"].ToString() ?? "").Trim();

                // If empty OR already exists ? fetch fresh number from DB
                if (string.IsNullOrWhiteSpace(returnNo) ||
                    await _context.PurchaseReturns.AnyAsync(r => r.ReturnNo == returnNo))
                {
                    returnNo = await GenerateNextReturnNo();
                }

                // Last-chance check (race condition) � if still duplicate, use timestamp
                if (await _context.PurchaseReturns.AnyAsync(r => r.ReturnNo == returnNo))
                {
                    returnNo = "PR-" + DateTime.Now.Year + "-" + DateTime.Now.ToString("HHmmssfff");
                }

                // --- Save ---
                var pr = new PurchaseReturn
                {
                    ReturnNo = returnNo,
                    PurchaseId = purchaseId,
                    GrnId = grnId,
                    ReturnDate = returnDate,
                    ReturnReason = returnReason,
                    TotalAmount = totalAmount,
                    Status = "Draft",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = UserId.ToString(),
                    CreatedDate = DateTime.Now
                };

                _context.PurchaseReturns.Add(pr);
                await _context.SaveChangesAsync();

                foreach (var d in details)
                {
                    d.ReturnId = pr.ReturnId;
                    _context.PurchaseReturnDetails.Add(d);
                }
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Purchase Return created: " + pr.ReturnNo,
                    returnId = pr.ReturnId,
                    returnNo = pr.ReturnNo
                });
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
                if (!int.TryParse(f["ReturnId"], out int id) || id <= 0)
                    return Json(new { success = false, message = "Invalid ID." });

                var pr = await _context.PurchaseReturns
                    .Include(r => r.PurchaseReturnDetails)
                    .FirstOrDefaultAsync(r => r.ReturnId == id);
                if (pr == null) return Json(new { success = false, message = "Not found." });

                if (int.TryParse(f["PurchaseId"], out int pid)) pr.PurchaseId = pid;
                if (DateTime.TryParse(f["ReturnDate"], out var rd)) pr.ReturnDate = rd;
                pr.ReturnReason = f["ReturnReason"].ToString() ?? pr.ReturnReason;
                if (int.TryParse(f["GrnId"], out int gid) && gid > 0) pr.GrnId = gid;

                var st = f["Status"].ToString();
                if (!string.IsNullOrEmpty(st)) pr.Status = st;

                // Soft-delete old details
                if (pr.PurchaseReturnDetails != null)
                {
                    foreach (var d in pr.PurchaseReturnDetails)
                        d.IsDeleted = true;
                }

                // Add new details
                var newDetails = new List<PurchaseReturnDetail>();
                decimal total = 0;
                for (int i = 0; ; i++)
                {
                    var pidStr = f["ReturnDetails[" + i + "].ProductId"].ToString();
                    if (string.IsNullOrEmpty(pidStr)) break;
                    if (!int.TryParse(pidStr, out int rpid) || rpid <= 0) continue;

                    int.TryParse(f["ReturnDetails[" + i + "].PurchaseDetailId"].ToString(), out int pdid);
                    int.TryParse(f["ReturnDetails[" + i + "].Quantity"].ToString(), out int qty);
                    decimal.TryParse(f["ReturnDetails[" + i + "].UnitPrice"].ToString(), out decimal price);
                    if (qty <= 0) continue;

                    var batchNo = f["ReturnDetails[" + i + "].BatchNo"].ToString();
                    DateOnly? expDate = null;
                    if (DateOnly.TryParse(f["ReturnDetails[" + i + "].ExpiryDate"], out var ed)) expDate = ed;

                    var d = new PurchaseReturnDetail
                    {
                        ReturnId = pr.ReturnId,
                        PurchaseDetailId = pdid,
                        ProductId = rpid,
                        Quantity = qty,
                        UnitPrice = price,
                        TotalPrice = qty * price,
                        BatchNo = batchNo,
                        ExpiryDate = expDate,
                        CreatedBy = UserId.ToString(),
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    newDetails.Add(d);
                    total += d.TotalPrice;
                }

                pr.TotalAmount = total;
                var t = pr.GetType();
                if (t.GetProperty("UpdatedBy") != null) t.GetProperty("UpdatedBy").SetValue(pr, UserId);
                if (t.GetProperty("UpdatedDate") != null) t.GetProperty("UpdatedDate").SetValue(pr, DateTime.Now);

                if (newDetails.Count > 0)
                    _context.PurchaseReturnDetails.AddRange(newDetails);

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Purchase Return updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // ---------------------------------------------------
        // TOGGLE STATUS
        // ---------------------------------------------------
        

        // ---------------------------------------------------
        // DELETE
        // ---------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var pr = await _context.PurchaseReturns
                    .Include(r => r.PurchaseReturnDetails)
                    .FirstOrDefaultAsync(r => r.ReturnId == id);
                if (pr == null) return Json(new { success = false, message = "Not found." });

                if (pr.PurchaseReturnDetails != null)
                {
                    foreach (var d in pr.PurchaseReturnDetails)
                        d.IsDeleted = true;
                }

                pr.IsDeleted = true;
                var t = pr.GetType();
                if (t.GetProperty("DeletedBy") != null) t.GetProperty("DeletedBy").SetValue(pr, UserId);
                if (t.GetProperty("DeletedDate") != null) t.GetProperty("DeletedDate").SetValue(pr, DateTime.Now);

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Purchase Return deleted successfully." });
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
                var pr = await _context.PurchaseReturns
                    .Include(r => r.PurchaseReturnDetails)
                    .FirstOrDefaultAsync(r => r.ReturnId == id && r.IsDeleted != true);

                if (pr == null) return NotFound();

                var details = await _context.PurchaseReturnDetails
                    .Where(d => d.ReturnId == id && (d.IsDeleted == null || d.IsDeleted == false))
                    .ToListAsync();

                ViewBag.Details = details;

                var po = await _context.PurchaseMasters.FindAsync(pr.PurchaseId);
                ViewBag.PurchaseOrderNo = po != null ? po.PurchaseOrderNo : "-";

                if (pr.GrnId.HasValue && pr.GrnId.Value > 0)
                {
                    var grn = await _context.Grns.FindAsync(pr.GrnId.Value);
                    ViewBag.GrnNo = grn != null ? grn.GrnNo : "-";
                }
                else
                {
                    ViewBag.GrnNo = "-";
                }

                return View("Profile", pr);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Profile");
            }
        }
    

        // ---------------------------------------------------
        // GET Grn DETAILS � Returns items with batch numbers
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetGrndetails(int grnId)
        {
            try
            {
                var grn = await _context.Grns.FindAsync(grnId);
                if (grn == null) return Json(new List<object>());

                // Load Grn details
                var grnDetails = await _context.Grndetails
                    .Include(d => d.Product)
                    .Where(d => d.GrnId == grnId && (d.IsDeleted == null || d.IsDeleted == false))
                    .ToListAsync();

                // Get PurchaseDetails for this Purchase (to find correct PurchaseDetailId)
                var purchaseDetails = await _context.PurchaseDetails
                    .Where(pd => pd.PurchaseId == grn.PurchaseId)
                    .ToListAsync();

                var result = new List<object>();
                foreach (var d in grnDetails)
                {
                    // Find matching PurchaseDetail by ProductId
                    var pd = purchaseDetails.FirstOrDefault(x => x.ProductId == d.ProductId);

                    result.Add(new
                    {
                        grnDetailId = d.GrndetailId,
                        purchaseDetailId = pd != null ? pd.DetailId : 0,
                        productId = d.ProductId,
                        productName = d.Product != null ? d.Product.ProductName : ("Product #" + d.ProductId),
                        batchNo = d.BatchNo ?? "",
                        expiryDate = d.ExpiryDate,
                        mfgDate = d.MfgDate,
                        receivedQty = d.ReceivedQty,
                        acceptedQty = d.AcceptedQty,
                        binId = d.BinId
                    });
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    
        // --- Toggle Status � Safe with allowed values ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            try
            {
                var pr = await _context.PurchaseReturns.FindAsync(id);
                if (pr == null) return Json(new { success = false, message = "Not found." });

                string oldStatus = pr.Status ?? "Draft";

                // Allowed statuses from DB constraint
                var allowed = new[] { "Cancelled", "Rejected", "Processed", "Approved", "Submitted", "Draft" };

                // Determine next status � safe
                string newStatus = oldStatus;
                
                // If current is Pending/Draft ? Approved/Completed
                if (oldStatus == "Draft" || oldStatus == "Pending")
                {
                    if (allowed.Contains("Approved")) newStatus = "Approved";
                    else if (allowed.Contains("Completed")) newStatus = "Completed";
                }
                // If Approved/Completed ? back to Pending/Draft
                else if (oldStatus == "Approved" || oldStatus == "Completed")
                {
                    if (allowed.Contains("Pending")) newStatus = "Pending";
                    else if (allowed.Contains("Draft")) newStatus = "Draft";
                }
                // Any other ? Pending/Draft
                else
                {
                    if (allowed.Contains("Pending")) newStatus = "Pending";
                    else if (allowed.Contains("Draft")) newStatus = "Draft";
                }

                pr.Status = newStatus;
                
                var t = pr.GetType();
                if (t.GetProperty("UpdatedBy") != null) t.GetProperty("UpdatedBy").SetValue(pr, UserId);
                if (t.GetProperty("UpdatedDate") != null) t.GetProperty("UpdatedDate").SetValue(pr, DateTime.Now);

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + oldStatus + " ? " + newStatus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }
    }
}








