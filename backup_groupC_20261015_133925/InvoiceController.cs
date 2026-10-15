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
    public class InvoiceController : Controller
    {
        private readonly AppDbContext _context;

        public InvoiceController(AppDbContext c)
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
                var list = await _context.Invoices.Where(i => i.IsDeleted != true).ToListAsync();
                var now = DateTime.Now;
                return Json(new
                {
                    total = list.Count,
                    active = list.Count(i => i.Status == "Paid" || i.Status == "Approved"),
                    inactive = list.Count(i => i.Status == "Pending" || i.Status == "Draft" || i.Status == "Unpaid"),
                    addedThisMonth = list.Count(i =>
                        i.CreatedDate.HasValue &&
                        i.CreatedDate.Value.Month == now.Month &&
                        i.CreatedDate.Value.Year == now.Year)
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
                var data = await (from i in _context.Invoices
                                  join s in _context.SalesMasters on i.SalesId equals s.SalesId into sj
                                  from s in sj.DefaultIfEmpty()
                                  join c in _context.Customers on s.CustomerId equals c.CustomerId into cj
                                  from c in cj.DefaultIfEmpty()
                                  where i.IsDeleted != true
                                  orderby i.InvoiceId
                                  select new
                                  {
                                      invoiceId = i.InvoiceId,
                                      invoiceNo = i.InvoiceNo,
                                      salesId = i.SalesId,
                                      salesOrderNo = s != null ? s.SalesOrderNo : "-",
                                      customerName = c != null ? c.CustomerName : "-",
                                      invoiceDate = i.InvoiceDate,
                                      dueDate = i.DueDate,
                                      netAmount = i.NetAmount,
                                      paidAmount = i.PaidAmount.ToString(),      // ✅ decimal?
                                      balanceDue = i.BalanceDue,                 // ✅ decimal?
                                      status = i.Status ?? "Pending"
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
        public async Task<IActionResult> GetSalesOrders()
        {
            try
            {
                var invoicedSOIds = await _context.Invoices
                    .Where(i => i.IsDeleted != true)
                    .Select(i => i.SalesId)
                    .Distinct()
                    .ToListAsync();

                var l = await _context.SalesMasters
                    .Where(s => s.IsDeleted != true && !invoicedSOIds.Contains(s.SalesId))
                    .OrderBy(s => s.SalesId)
                    .Select(s => new { salesId = s.SalesId, salesOrderNo = s.SalesOrderNo })
                    .ToListAsync();

                return Json(l);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
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
                        discountPercent = d.DiscountPercent,        // ✅ decimal?
                        batchNo = d.BatchNo ?? ""
                    })
                    .ToListAsync();
                return Json(details);
            }
            catch
            {
                return Json(new List<object>());
            }
        }

        // -------------------------------------------------------
        // AUTO-GENERATE INVOICE NO
        // -------------------------------------------------------
        private async Task<string> GenerateNextInvoiceNo()
        {
            try
            {
                var year = DateTime.Now.Year;
                var prefix = "INV-" + year + "-";
                var existing = await _context.Invoices
                    .Where(i => i.InvoiceNo != null && i.InvoiceNo.StartsWith(prefix))
                    .Select(i => i.InvoiceNo)
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
                return "INV-" + DateTime.Now.Year + "-" + DateTime.Now.ToString("HHmmss");
            }
        }

        // -------------------------------------------------------
        // ADD PARTIAL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> AddPartial()
        {
            ViewBag.NextInvoiceNo = await GenerateNextInvoiceNo();
            ViewBag.SalesOrders = await _context.SalesMasters
                .Where(s => s.IsDeleted != true && !_context.Invoices.Where(i => i.IsDeleted != true).Select(i => i.SalesId).Contains(s.SalesId))
                .OrderBy(s => s.SalesId)
                .Select(s => new { salesId = s.SalesId, salesOrderNo = s.SalesOrderNo })
                .ToListAsync();
            return PartialView("_AddForm");
        }

        // -------------------------------------------------------
        // EDIT PARTIAL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var inv = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == id && i.IsDeleted != true);
            if (inv == null) return NotFound();

            ViewBag.Invoice = inv;
            ViewBag.SalesOrders = await _context.SalesMasters
                .Where(s => s.IsDeleted != true && !_context.Invoices.Where(i => i.IsDeleted != true).Select(i => i.SalesId).Contains(s.SalesId))
                .OrderBy(s => s.SalesId)
                .Select(s => new { salesId = s.SalesId, salesOrderNo = s.SalesOrderNo })
                .ToListAsync();
            return PartialView("_EditForm", inv);
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

                if (!int.TryParse(f["SalesId"], out int salesId) || salesId <= 0)
                    return Json(new { success = false, message = "Please select Sales Order." });

                var invNo = (f["InvoiceNo"].ToString() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(invNo) || await _context.Invoices.AnyAsync(i => i.InvoiceNo == invNo))
                    invNo = await GenerateNextInvoiceNo();
                if (await _context.Invoices.AnyAsync(i => i.InvoiceNo == invNo))
                    invNo = "INV-" + DateTime.Now.Year + "-" + DateTime.Now.ToString("HHmmssfff");

                var invDate = DateTime.TryParse(f["InvoiceDate"], out var idt) ? idt : DateTime.Now;
                DateOnly dueDate = DateOnly.TryParse(f["DueDate"], out var dd) ? dd : DateOnly.FromDateTime(DateTime.Now.AddDays(30));

                decimal.TryParse(f["Discount"], out decimal disc);
                decimal.TryParse(f["TaxAmount"], out decimal tax);
                decimal.TryParse(f["PaidAmount"], out decimal paid);

                var details = new List<InvoiceDetail>();
                decimal subTotal = 0;

                for (int i = 0; ; i++)
                {
                    var pidStr = f["InvoiceDetails[" + i + "].ProductId"].ToString();
                    if (string.IsNullOrEmpty(pidStr)) break;
                    if (!int.TryParse(pidStr, out int pid) || pid <= 0) continue;

                    int.TryParse(f["InvoiceDetails[" + i + "].SalesDetailId"].ToString(), out int sdid);
                    int.TryParse(f["InvoiceDetails[" + i + "].Quantity"].ToString(), out int qty);
                    decimal.TryParse(f["InvoiceDetails[" + i + "].UnitPrice"].ToString(), out decimal price);
                    decimal.TryParse(f["InvoiceDetails[" + i + "].DiscountPercent"].ToString(), out decimal dp);
                    if (qty <= 0) continue;

                    decimal lineTotal = qty * price;
                    decimal netLine = lineTotal - (lineTotal * dp / 100m);

                    details.Add(new InvoiceDetail
                    {
                        SalesDetailId = sdid,
                        ProductId = pid,
                        Quantity = qty,
                        UnitPrice = price,
                        DiscountPercent = dp,
                        TotalPrice = netLine,
                        BatchNo = f["InvoiceDetails[" + i + "].BatchNo"].ToString(),
                        CreatedBy = UserId.ToString(),
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    });
                    subTotal += netLine;
                }

                if (details.Count == 0)
                    return Json(new { success = false, message = "At least one item required." });

                decimal netAmount = subTotal - disc + tax;
                decimal balance = netAmount - paid;

                var inv = new Invoice
                {
                    InvoiceNo = invNo,
                    SalesId = salesId,
                    InvoiceDate = invDate,
                    DueDate = dueDate,
                    TotalAmount = subTotal,
                    Discount = disc,
                    TaxAmount = tax,
                    NetAmount = netAmount,
                    PaidAmount = paid,
                    BalanceDue = balance < 0 ? 0 : balance,
                    Status = "Unpaid",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = UserId.ToString(),
                    CreatedDate = DateTime.Now
                };

                _context.Invoices.Add(inv);
                await _context.SaveChangesAsync();

                foreach (var d in details)
                {
                    d.InvoiceId = inv.InvoiceId;
                    _context.InvoiceDetails.Add(d);
                }
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Invoice created: " + inv.InvoiceNo, id = inv.InvoiceId });
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
                var inv = await _context.Invoices.FindAsync(id);
                if (inv == null)
                    return Json(new { success = false, message = "Not found." });

                string old = inv.Status ?? "Pending";
                if (old == "Pending" || old == "Draft" || old == "Unpaid")
                    inv.Status = "Paid";
                else
                    inv.Status = "Pending";

                inv.UpdatedBy = UserId.ToString();
                inv.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + old + " -> " + inv.Status });
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
                var inv = await _context.Invoices
                    .Include(i => i.InvoiceDetails)
                    .FirstOrDefaultAsync(i => i.InvoiceId == id);
                if (inv == null)
                    return Json(new { success = false, message = "Not found." });

                if (inv.InvoiceDetails != null)
                {
                    foreach (var d in inv.InvoiceDetails)
                        d.IsDeleted = true;
                }

                inv.IsDeleted = true;
                inv.DeletedBy = UserId.ToString();
                inv.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Invoice deleted." });
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
            var inv = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == id && i.IsDeleted != true);
            if (inv == null) return NotFound();

            var details = await _context.InvoiceDetails
                .Include(d => d.Product)
                .Where(d => d.InvoiceId == id && (d.IsDeleted == null || d.IsDeleted == false))
                .ToListAsync();
            ViewBag.Details = details;

            var so = await _context.SalesMasters.FindAsync(inv.SalesId);
            ViewBag.SalesOrderNo = so != null ? so.SalesOrderNo : "-";
            ViewBag.DeliveryDate = so != null ? so.DeliveryDate : null;

            if (so != null)
            {
                var cust = await _context.Customers.FindAsync(so.CustomerId);
                ViewBag.CustomerName = cust != null ? cust.CustomerName : "-";
                ViewBag.CustomerPhone = cust != null ? cust.Phone : "-";
                ViewBag.CustomerEmail = cust != null ? cust.Email : "-";
                ViewBag.CustomerAddress = cust != null ? cust.Address : "-";
            }

            return View("Profile", inv);
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
                if (!int.TryParse(f["InvoiceId"], out int id) || id <= 0)
                    return Json(new { success = false, message = "Invalid Invoice ID." });

                var inv = await _context.Invoices
                    .Include(i => i.InvoiceDetails)
                    .FirstOrDefaultAsync(i => i.InvoiceId == id);
                if (inv == null)
                    return Json(new { success = false, message = "Invoice not found." });

                if (DateTime.TryParse(f["InvoiceDate"], out var idate)) inv.InvoiceDate = idate;
                if (DateOnly.TryParse(f["DueDate"], out var dueDate)) inv.DueDate = dueDate;

                decimal discount = 0, tax = 0, paid = 0;
                decimal.TryParse(f["Discount"], out discount);
                decimal.TryParse(f["TaxAmount"], out tax);
                decimal.TryParse(f["PaidAmount"], out paid);

                inv.Discount = discount;
                inv.TaxAmount = tax;
                inv.PaidAmount = paid;

                var activeDetails = inv.InvoiceDetails
                    .Where(d => d.IsDeleted == null || d.IsDeleted == false)
                    .ToList();

                decimal totalAmount = activeDetails.Sum(d => d.TotalPrice);
                inv.TotalAmount = totalAmount;
                inv.NetAmount = totalAmount - discount + tax;

                decimal balance = inv.NetAmount - paid;
                inv.BalanceDue = balance < 0 ? 0 : balance;

                string status = (f["Status"].ToString() ?? "").Trim();
                if (string.IsNullOrEmpty(status)) status = "Pending";
                inv.Status = status;

                inv.UpdatedBy = UserId.ToString();
                inv.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Invoice updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }
    }
}