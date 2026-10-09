using ERPWMS.Application.DTOs.Sales;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICurrentUserService = ERPWMS.Application.Common.ICurrentUserService;
using ERPWMS.Web.Filters;
using ERPWMS.Database;
using ERPWMS.Application.Common;

namespace ERPWMS.Web.Controllers.Sales
{
    [Authorize]
    public class SalesReturnController : Controller
    {
        private readonly ISalesReturnService _returnService;
        private readonly ICurrentUserService _currentUserService;
        private readonly AppDbContext _context;

        public SalesReturnController(
            ISalesReturnService returnService,
            ICurrentUserService currentUserService,
            AppDbContext context)
        {
            _returnService = returnService;
            _currentUserService = currentUserService;
            _context = context;
        }

        // ✅ FIX: UserId string? hai — TryParse sahi hai
        private int GetUserId()
        {
            if (int.TryParse(_currentUserService.UserId, out var id)) return id;
            return 0;
        }

        // ✅ FIX: CompanyId int? hai — direct use karo
        private int GetCompanyId()
        {
            return _currentUserService.CompanyId ?? 0;
        }

        // ============================================================
        // INDEX
        // ============================================================
        public async Task<IActionResult> Index(string? searchTerm, string? status)
        {
            try
            {
                var returns = await _context.SalesReturns
                    .Include(r => r.Sales)
                    .Include(r => r.Invoice)
                    .Where(r => r.IsDeleted != true)
                    .OrderBy(r => r.ReturnId)
                    .ToListAsync();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.Trim();
                    returns = returns.Where(r =>
                        (!string.IsNullOrEmpty(r.ReturnNo) && r.ReturnNo.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (r.Sales != null && !string.IsNullOrEmpty(r.Sales.SalesOrderNo) && r.Sales.SalesOrderNo.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                if (!string.IsNullOrWhiteSpace(status))
                    returns = returns.Where(r => r.Status == status).ToList();

                ViewBag.CurrentStatus = status;
                ViewBag.SearchTerm = searchTerm;
                ViewBag.Statuses = new[] { "Draft", "Submitted", "Approved", "Processed", "Rejected", "Cancelled" };

                return View(returns);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading returns: " + ex.Message;
                return View(new List<SalesReturn>());
            }
        }

        // ============================================================
        // CREATE GET
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Create(int? salesId)
        {
            _ = salesId;

            try
            {
                ViewBag.Invoices = await _context.Invoices
                    .Include(i => i.Sales)
                        .ThenInclude(s => s.Customer)
                    .Where(i => i.IsDeleted != true && (i.Status == "Unpaid" || i.Status == "Partial" || i.Status == "Paid" || i.Status == "Draft"))
                    .OrderBy(i => i.InvoiceId)
                    .ToListAsync();

                var model = new CreateSalesReturnRequest
                {
                    ReturnDate = DateTime.Now,
                    ReturnNo = "SR-" + DateTime.Now.ToString("yyyyMMdd-HHmmss")
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading form: " + ex.Message;
                return View(new CreateSalesReturnRequest());
            }
        }

        // ============================================================
        // CREATE POST
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSalesReturnRequest request)
        {
            try
            {
                if (request.InvoiceId > 0)
                {
                    var existsReturn = await _context.SalesReturns
                        .AnyAsync(r => r.InvoiceId == request.InvoiceId && r.IsDeleted != true);
                    if (existsReturn)
                    {
                        ModelState.AddModelError("", "A Sales Return already exists for this Invoice.");
                        ViewBag.Invoices = await _context.Invoices.Include(i => i.Sales).ToListAsync();
                        return View(request);
                    }
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.Invoices = await _context.Invoices
                        .Include(i => i.Sales).ThenInclude(s => s.Customer)
                        .Where(i => i.IsDeleted != true)
                        .OrderBy(i => i.InvoiceId)
                        .ToListAsync();
                    return View(request);
                }

                if (request.SalesReturnDetails == null || request.SalesReturnDetails.Count == 0)
                {
                    ModelState.AddModelError("", "At least one item is required.");
                    ViewBag.Invoices = await _context.Invoices.Include(i => i.Sales).ToListAsync();
                    return View(request);
                }

                var userId = GetUserId();

                var salesReturn = new SalesReturn
                {
                    SalesId = request.SalesId,
                    InvoiceId = request.InvoiceId > 0 ? request.InvoiceId : null,
                    ReturnDate = request.ReturnDate ?? DateTime.Now,
                    ReturnNo = request.ReturnNo ?? ("SR-" + DateTime.Now.ToString("yyyyMMdd-HHmmss")),
                    ReturnReason = request.ReturnReason ?? "Not specified",
                    Status = "Draft",
                    CreatedBy = userId.ToString(),
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };

                _context.SalesReturns.Add(salesReturn);
                await _context.SaveChangesAsync();

                decimal total = 0;
                foreach (var d in request.SalesReturnDetails)
                {
                    if (d.Quantity <= 0 || d.UnitPrice <= 0) continue;

                    var detail = new SalesReturnDetail
                    {
                        ReturnId = salesReturn.ReturnId,
                        InvoiceDetailId = d.InvoiceDetailId,
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        TotalPrice = d.Quantity * d.UnitPrice,
                        BatchNo = d.BatchNo,
                        CreatedBy = userId.ToString(),
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };

                    _context.SalesReturnDetails.Add(detail);
                    total += detail.TotalPrice;
                }

                salesReturn.TotalAmount = total;
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Sales Return '{salesReturn.ReturnNo}' created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                ViewBag.Invoices = await _context.Invoices.Include(i => i.Sales).ToListAsync();
                return View(request);
            }
        }

        // ============================================================
        // DETAILS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var r = await _context.SalesReturns
                .Include(x => x.Sales)
                .Include(x => x.Invoice)
                .FirstOrDefaultAsync(x => x.ReturnId == id && x.IsDeleted != true);

            if (r == null) return NotFound();

            var details = await _context.SalesReturnDetails
                .Include(d => d.Product)
                .Where(d => d.ReturnId == id && d.IsDeleted != true)
                .ToListAsync();

            var dto = new SalesReturnDto
            {
                ReturnId = r.ReturnId,
                SalesId = r.SalesId,
                SalesOrderNo = r.Sales != null ? r.Sales.SalesOrderNo : "-",
                InvoiceId = r.InvoiceId,
                InvoiceNo = r.Invoice != null ? r.Invoice.InvoiceNo : "-",
                ReturnDate = r.ReturnDate,
                ReturnNo = r.ReturnNo,
                ReturnReason = r.ReturnReason,
                TotalAmount = r.TotalAmount,
                Status = r.Status,
                ApprovedBy = r.ApprovedBy,
                ApprovedByName = r.ApprovedByNavigation != null ? r.ApprovedByNavigation.UserName : null,
                ApprovedDate = r.ApprovedDate,
                CreatedBy = r.CreatedBy,
                CreatedByName = r.CreatedByNavigation != null ? r.CreatedByNavigation.UserName : "User #" + r.CreatedBy,
                CreatedDate = r.CreatedDate,
                SalesReturnDetails = details.Select(d => new SalesReturnDetailDto
                {
                    ReturnDetailId = d.ReturnDetailId,
                    ReturnId = d.ReturnId,
                    InvoiceDetailId = d.InvoiceDetailId,
                    ProductId = d.ProductId,
                    ProductName = d.Product != null ? d.Product.ProductName : "N/A",
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    TotalPrice = d.TotalPrice,
                    BatchNo = d.BatchNo
                }).ToList()
            };

            return View(dto);
        }

        // ============================================================
        // GET INVOICE DETAILS (AJAX)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetInvoiceDetails(int invoiceId)
        {
            try
            {
                if (invoiceId <= 0) return Json(new List<object>());

                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId && i.IsDeleted != true);

                if (invoice == null)
                    return Json(new { success = false, message = "Invoice not found." });

                var items = await _context.InvoiceDetails
                    .Include(d => d.Product)
                    .Where(d => d.InvoiceId == invoiceId && d.IsDeleted != true)
                    .Select(d => new
                    {
                        invoiceDetailId = d.InvoiceDetailId,
                        productId = d.ProductId,
                        productName = d.Product != null ? d.Product.ProductName : "N/A",
                        quantity = d.Quantity,
                        unitPrice = d.UnitPrice,
                        batchNo = d.BatchNo ?? ""
                    })
                    .ToListAsync();

                return Json(new { success = true, salesId = invoice.SalesId, items = items });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // SUBMIT / APPROVE / REJECT / DELETE
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int id)
        {
            try
            {
                var r = await _context.SalesReturns.FindAsync(id);
                if (r == null) return NotFound();
                if (r.Status != "Draft") throw new Exception("Only Draft returns can be submitted.");
                r.Status = "Submitted";
                r.UpdatedBy = GetUserId().ToString();
                r.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Return submitted for approval.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message?.ToString(); }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSelfApproval(TransactionType = "SalesReturn")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var r = await _context.SalesReturns.FindAsync(id);
                if (r == null) return NotFound();
                if (r.Status != "Submitted") throw new Exception("Only Submitted returns can be approved.");
                r.Status = "Approved";
                r.ApprovedBy = GetUserId().ToString();
                r.ApprovedDate = DateTime.Now;
                r.UpdatedBy = GetUserId().ToString();
                r.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Return approved successfully.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message?.ToString(); }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var r = await _context.SalesReturns.FindAsync(id);
                if (r == null) return NotFound();
                if (r.Status != "Submitted") throw new Exception("Only Submitted returns can be rejected.");
                r.Status = "Rejected";
                r.UpdatedBy = GetUserId().ToString();
                r.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Return rejected.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message?.ToString(); }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var r = await _context.SalesReturns.FindAsync(id);
                if (r == null) return NotFound();
                if (r.Status != "Draft") throw new Exception("Only Draft returns can be deleted.");
                r.IsDeleted = true;
                r.DeletedBy = GetUserId().ToString();
                r.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Return deleted.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message?.ToString(); }
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // UI ENDPOINTS (Modal + JSON + Profile)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAllReturnsJson()
        {
            try
            {
                var list = await _context.SalesReturns
                    .Include(r => r.Sales).ThenInclude(s => s.Customer)
                    .Include(r => r.Invoice)
                    .Where(r => r.IsDeleted != true)
                    .OrderBy(r => r.ReturnId)
                    .Select(r => new
                    {
                        returnId = r.ReturnId,
                        returnNo = r.ReturnNo,
                        salesOrderNo = r.Sales != null ? r.Sales.SalesOrderNo : "",
                        customerName = r.Sales != null && r.Sales.Customer != null ? r.Sales.Customer.CustomerName : "",
                        invoiceNo = r.Invoice != null ? r.Invoice.InvoiceNo : "",
                        returnDate = r.ReturnDate,
                        returnReason = r.ReturnReason,
                        totalAmount = r.TotalAmount,
                        status = r.Status ?? "Draft",
                        createdByName = r.CreatedByNavigation != null ? r.CreatedByNavigation.UserName : "",
                        createdDate = r.CreatedDate
                    }).ToListAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var r = await _context.SalesReturns
                .Include(x => x.Sales).ThenInclude(s => s.Customer)
                .Include(x => x.Invoice)
                .FirstOrDefaultAsync(x => x.ReturnId == id && x.IsDeleted != true);
            if (r == null) return NotFound();

            var details = await _context.SalesReturnDetails
                .Include(d => d.Product)
                .Where(d => d.ReturnId == id && d.IsDeleted != true)
                .ToListAsync();

            var model = new SalesReturnDto
            {
                ReturnId = r.ReturnId,
                SalesId = r.SalesId,
                SalesOrderNo = r.Sales != null ? r.Sales.SalesOrderNo : "",
                InvoiceId = r.InvoiceId,
                InvoiceNo = r.Invoice != null ? r.Invoice.InvoiceNo : "",
                ReturnDate = r.ReturnDate,
                ReturnNo = r.ReturnNo,
                ReturnReason = r.ReturnReason,
                TotalAmount = r.TotalAmount,
                Status = r.Status,
                ApprovedBy = r.ApprovedBy,
                ApprovedByName = r.ApprovedByNavigation != null ? r.ApprovedByNavigation.UserName : "",
                ApprovedDate = r.ApprovedDate,
                CreatedBy = r.CreatedBy,
                CreatedByName = r.CreatedByNavigation != null ? r.CreatedByNavigation.UserName : "",
                CreatedDate = r.CreatedDate,
                IsDeleted = r.IsDeleted ?? false,
                SalesReturnDetails = details.Select(d => new SalesReturnDetailDto
                {
                    ReturnDetailId = d.ReturnDetailId,
                    ReturnId = d.ReturnId,
                    InvoiceDetailId = d.InvoiceDetailId,
                    ProductId = d.ProductId,
                    ProductName = d.Product != null ? d.Product.ProductName : "",
                    ProductCode = d.Product != null ? (d.Product.Sku ?? "") : "",
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    TotalPrice = d.TotalPrice,
                    BatchNo = d.BatchNo
                }).ToList()
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult GetCreateModalPartial(int? invoiceId = null)
        {
            var returnedIds = _context.SalesReturns
                .Where(r => r.IsDeleted != true && r.InvoiceId != null)
                .Select(r => r.InvoiceId.Value)
                .Distinct()
                .ToList();

            var invoices = _context.Invoices
                .Include(i => i.Sales).ThenInclude(s => s.Customer)
                .Where(i => i.IsDeleted != true && !returnedIds.Contains(i.InvoiceId))
                .OrderBy(i => i.InvoiceId)
                .ToList();

            ViewBag.Invoices = invoices;
            ViewBag.PreselectedInvoiceId = invoiceId;

            return PartialView("_CreateReturnForm");
        }

        [HttpGet]
        public async Task<IActionResult> GetCreateDropdowns()
        {
            try
            {
                var returnedIds = await _context.SalesReturns
                    .Where(r => r.IsDeleted != true && r.InvoiceId != null)
                    .Select(r => r.InvoiceId.Value)
                    .Distinct()
                    .ToListAsync();

                var invoices = await _context.Invoices
                    .Include(i => i.Sales).ThenInclude(s => s.Customer)
                    .Where(i => i.IsDeleted != true && !returnedIds.Contains(i.InvoiceId))
                    .OrderBy(i => i.InvoiceId)
                    .Select(i => new
                    {
                        invoiceId = i.InvoiceId,
                        invoiceNo = i.InvoiceNo,
                        salesId = i.SalesId,
                        salesOrderNo = i.Sales != null ? i.Sales.SalesOrderNo : "",
                        customerName = i.Sales != null && i.Sales.Customer != null ? i.Sales.Customer.CustomerName : ""
                    })
                    .ToListAsync();

                return Ok(new { invoices });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}