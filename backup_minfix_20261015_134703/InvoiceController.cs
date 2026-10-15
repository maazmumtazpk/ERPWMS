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
    [Authorize(Policy = "Invoice.Access")]
    public class InvoiceController : Controller
    {
        private readonly AppDbContext _context;
        public InvoiceController(AppDbContext c) { _context = c; }

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
            var q = _context.Invoices.Where(i => i.IsDeleted != true);
            return Json(new
            {
                total = await q.CountAsync(),
                active = await q.CountAsync(i => i.Status == "Paid" || i.Status == "Approved"),
                inactive = await q.CountAsync(i => i.Status == "Pending" || i.Status == "Draft" || i.Status == "Unpaid"),
                addedThisMonth = await q.CountAsync(i =>
                    i.CreatedDate.HasValue &&
                    i.CreatedDate.Value.Month == now.Month &&
                    i.CreatedDate.Value.Year == now.Year)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await (from i in _context.Invoices
                              join s in _context.SalesMasters on i.SalesId equals s.SalesId into sj
                              from s in sj.DefaultIfEmpty()
                              join c in _context.Customers on s.CustomerId equals c.CustomerId into cj
                              from c in cj.DefaultIfEmpty()
                              where i.IsDeleted != true
                              orderby i.InvoiceId descending
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
                                  paidAmount = i.PaidAmount,
                                  balanceDue = i.BalanceDue,
                                  status = i.Status ?? "Pending"
                              }).ToListAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesOrders()
        {
            var invoicedIds = _context.Invoices.Where(i => i.IsDeleted != true).Select(i => i.SalesId);
            var l = await _context.SalesMasters
                .Where(s => s.IsDeleted != true && !invoicedIds.Contains(s.SalesId))
                .OrderBy(s => s.SalesId)
                .Select(s => new { salesId = s.SalesId, salesOrderNo = s.SalesOrderNo })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesDetails(int salesId)
        {
            if (salesId <= 0) return Json(new List<object>());
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
                    discountPercent = d.DiscountPercent,
                    batchNo = d.BatchNo ?? ""
                }).ToListAsync();
            return Json(details);
        }

        private async Task<string> GenerateNextInvoiceNo()
        {
            var year = DateTime.Now.Year;
            var prefix = "INV-" + year + "-";
            var existing = await _context.Invoices
                .Where(i => i.InvoiceNo != null && i.InvoiceNo.StartsWith(prefix))
                .Select(i => i.InvoiceNo).ToListAsync();

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
            ViewBag.NextInvoiceNo = await GenerateNextInvoiceNo();
            var invoicedIds = _context.Invoices.Where(i => i.IsDeleted != true).Select(i => i.SalesId);
            ViewBag.SalesOrders = await _context.SalesMasters
                .Where(s => s.IsDeleted != true && !invoicedIds.Contains(s.SalesId))
                .OrderBy(s => s.SalesId)
                .Select(s => new { salesId = s.SalesId, salesOrderNo = s.SalesOrderNo })
                .ToListAsync();
            return PartialView("_AddForm");
        }

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var inv = await _context.Invoices.Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == id && i.IsDeleted != true);
            if (inv == null) return NotFound();

            ViewBag.Invoice = inv;
            var invoicedIds = _context.Invoices.Where(i => i.IsDeleted != true && i.InvoiceId != id).Select(i => i.SalesId);
            ViewBag.SalesOrders = await _context.SalesMasters
                .Where(s => s.IsDeleted != true && !invoicedIds.Contains(s.SalesId))
                .OrderBy(s => s.SalesId)
                .Select(s => new { salesId = s.SalesId, salesOrderNo = s.SalesOrderNo })
                .ToListAsync();
            return PartialView("_EditForm", inv);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register()
        {
            var f = Request.Form;

            if (!int.TryParse(f["SalesId"], out int salesId) || salesId <= 0)
                return Json(new { success = false, message = "Please select Sales Order." });

            var invNo = (f["InvoiceNo"].ToString() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(invNo)) invNo = await GenerateNextInvoiceNo();

            var invDate = DateTime.TryParse(f["InvoiceDate"], out var idt) ? idt : DateTime.Now;
            DateOnly dueDate = DateOnly.TryParse(f["DueDate"], out var dd)
                ? dd : DateOnly.FromDateTime(DateTime.Now.AddDays(30));

            if (!decimal.TryParse(f["Discount"], out decimal disc) || disc < 0)
                return Json(new { success = false, message = "Invalid discount." });
            if (!decimal.TryParse(f["TaxAmount"], out decimal tax) || tax < 0)
                return Json(new { success = false, message = "Invalid tax." });
            if (!decimal.TryParse(f["PaidAmount"], out decimal paid) || paid < 0)
                return Json(new { success = false, message = "Invalid paid amount." });

            var details = new List<InvoiceDetail>();
            decimal subTotal = 0;

            for (int i = 0; i < MaxLineItems; i++)
            {
                var pidStr = f["InvoiceDetails[" + i + "].ProductId"].ToString();
                if (string.IsNullOrEmpty(pidStr)) break;
                if (!int.TryParse(pidStr, out int pid) || pid <= 0) continue;

                int.TryParse(f["InvoiceDetails[" + i + "].SalesDetailId"].ToString(), out int sdid);
                if (!int.TryParse(f["InvoiceDetails[" + i + "].Quantity"].ToString(), out int qty) || qty <= 0) continue;
                if (!decimal.TryParse(f["InvoiceDetails[" + i + "].UnitPrice"].ToString(), out decimal price) || price < 0) continue;

                decimal.TryParse(f["InvoiceDetails[" + i + "].DiscountPercent"].ToString(), out decimal dp);
                if (dp < 0 || dp > 100) dp = 0;

                decimal line = qty * price;
                decimal net = line - (line * dp / 100m);

                details.Add(new InvoiceDetail
                {
                    SalesDetailId = sdid,
                    ProductId = pid,
                    Quantity = qty,
                    UnitPrice = price,
                    DiscountPercent = dp,
                    TotalPrice = net,
                    BatchNo = f["InvoiceDetails[" + i + "].BatchNo"].ToString(),
                    CreatedBy = UserId.ToString(),
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                });
                subTotal += net;
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

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Invoices.Add(inv);
                await _context.SaveChangesAsync();

                foreach (var d in details) { d.InvoiceId = inv.InvoiceId; }
                _context.InvoiceDetails.AddRange(details);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                return Json(new { success = false, message = "Could not create invoice. Please try again." });
            }

            return Json(new { success = true, message = "Invoice created: " + inv.InvoiceNo, id = inv.InvoiceId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Invoice.Edit")]
        public async Task<IActionResult> Toggle(int id)
        {
            var inv = await _context.Invoices.FindAsync(id);
            if (inv == null || inv.IsDeleted == true)
                return Json(new { success = false, message = "Not found." });

            string old = inv.Status ?? "Pending";
            inv.Status = (old == "Pending" || old == "Draft" || old == "Unpaid") ? "Paid" : "Pending";
            inv.UpdatedBy = UserId.ToString();
            inv.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Status: " + old + " -> " + inv.Status });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Invoice.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var inv = await _context.Invoices.Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == id && i.IsDeleted != true);
            if (inv == null) return Json(new { success = false, message = "Not found." });

            if (inv.InvoiceDetails != null)
                foreach (var d in inv.InvoiceDetails) d.IsDeleted = true;

            inv.IsDeleted = true;
            inv.DeletedBy = UserId.ToString();
            inv.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Invoice deleted." });
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var inv = await _context.Invoices.Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == id && i.IsDeleted != true);
            if (inv == null) return NotFound();

            ViewBag.Details = await _context.InvoiceDetails
                .Include(d => d.Product)
                .Where(d => d.InvoiceId == id && (d.IsDeleted == null || d.IsDeleted == false))
                .ToListAsync();

            var so = await _context.SalesMasters.FindAsync(inv.SalesId);
            ViewBag.SalesOrderNo = so != null ? so.SalesOrderNo : "-";
            ViewBag.DeliveryDate = so != null ? so.DeliveryDate : null;

            if (so != null)
            {
                var cust = await _context.Customers.FindAsync(so.CustomerId);
                ViewBag.CustomerName = cust?.CustomerName ?? "-";
                ViewBag.CustomerPhone = cust?.Phone ?? "-";
                ViewBag.CustomerEmail = cust?.Email ?? "-";
                ViewBag.CustomerAddress = cust?.Address ?? "-";
            }
            return View("Profile", inv);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Invoice.Edit")]
        public async Task<IActionResult> Edit()
        {
            var f = Request.Form;
            if (!int.TryParse(f["InvoiceId"], out int id) || id <= 0)
                return Json(new { success = false, message = "Invalid Invoice ID." });

            var inv = await _context.Invoices.Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == id && i.IsDeleted != true);
            if (inv == null) return Json(new { success = false, message = "Invoice not found." });

            if (DateTime.TryParse(f["InvoiceDate"], out var idate)) inv.InvoiceDate = idate;
            if (DateOnly.TryParse(f["DueDate"], out var dueDate)) inv.DueDate = dueDate;

            decimal discount = 0, tax = 0, paid = 0;
            if (!decimal.TryParse(f["Discount"], out discount) || discount < 0)
                return Json(new { success = false, message = "Invalid discount." });
            if (!decimal.TryParse(f["TaxAmount"], out tax) || tax < 0)
                return Json(new { success = false, message = "Invalid tax." });
            if (!decimal.TryParse(f["PaidAmount"], out paid) || paid < 0)
                return Json(new { success = false, message = "Invalid paid amount." });

            inv.Discount = discount;
            inv.TaxAmount = tax;
            inv.PaidAmount = paid;

            var active = inv.InvoiceDetails.Where(d => d.IsDeleted == null || d.IsDeleted == false).ToList();
            decimal total = active.Sum(d => d.TotalPrice);
            inv.TotalAmount = total;
            inv.NetAmount = total - discount + tax;
            decimal balance = inv.NetAmount - paid;
            inv.BalanceDue = balance < 0 ? 0 : balance;

            string status = (f["Status"].ToString() ?? "").Trim();
            inv.Status = string.IsNullOrEmpty(status) ? "Pending" : status;

            inv.UpdatedBy = UserId.ToString();
            inv.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Invoice updated successfully." });
        }
    }
}
