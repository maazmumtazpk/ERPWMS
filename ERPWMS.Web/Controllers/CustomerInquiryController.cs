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
    public class CustomerInquiryController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerInquiryController(AppDbContext c)
        {
            _context = c;
        }

        private int Uid() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 1;

        // ============================================================
        // INDEX
        // ============================================================
        [HttpGet]
        public IActionResult Index() => View();

        // ============================================================
        // STATISTICS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var l = await _context.CustomerInquiries.ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(i => i.Status == "Resolved"),
                inactive = l.Count(i => i.Status == "Open" || i.Status == "Pending"),
                addedThisMonth = l.Count(i =>
                    i.CreatedDate.HasValue &&
                    i.CreatedDate.Value.Month == DateTime.Now.Month &&
                    i.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // ============================================================
        // GET ALL
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.CustomerInquiries
                .Include(i => i.Customer)
                .OrderBy(i => i.InquiryId)
                .Select(i => new
                {
                    inquiryId = i.InquiryId,
                    customerName = i.Customer != null ? i.Customer.CustomerName : "N/A",
                    subject = i.Subject,
                    inquiryType = i.InquiryType ?? "General",
                    status = i.Status ?? "Open",
                    createdDate = i.CreatedDate
                })
                .ToListAsync();
            return Json(l);
        }

        // ============================================================
        // GET CUSTOMERS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var l = await _context.Customers
                .Where(c => c.IsDeleted != true)
                .Select(c => new { customerId = c.CustomerId, customerName = c.CustomerName })
                .ToListAsync();
            return Json(l);
        }

        // ============================================================
        // ADD / EDIT PARTIALS
        // ============================================================
        [HttpGet]
        public IActionResult AddInquiryPartial() => PartialView("_AddInquiryForm");

        [HttpGet]
        public async Task<IActionResult> EditInquiryPartial(int id)
        {
            var i = await _context.CustomerInquiries.FindAsync(id);
            if (i == null) return NotFound();
            return PartialView("_EditInquiryForm", i);
        }

        // ============================================================
        // REGISTER
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int customerId, string? inquiryType, string subject, string? message)
        {
            try
            {
                if (customerId <= 0)
                    return Json(new { success = false, message = "Please select customer." });
                if (string.IsNullOrWhiteSpace(subject))
                    return Json(new { success = false, message = "Subject required." });

                _context.CustomerInquiries.Add(new CustomerInquiry
                {
                    CustomerId = customerId,
                    InquiryType = inquiryType ?? "General",
                    Subject = subject.Trim(),
                    Message = message ?? "",
                    Status = "Open",
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Inquiry created!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // EDIT
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int inquiryId, string subject, string? message, string? status)
        {
            try
            {
                var i = await _context.CustomerInquiries.FindAsync(inquiryId);
                if (i == null)
                    return Json(new { success = false, message = "Not found" });

                i.Subject = subject;
                i.Message = message;
                i.Status = status;
                i.UpdatedBy = Uid().ToString();
                i.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // DELETE
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var i = await _context.CustomerInquiries.FindAsync(id);
                if (i == null)
                    return Json(new { success = false, message = "Not found" });

                _context.CustomerInquiries.Remove(i);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // TOGGLE STATUS
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var i = await _context.CustomerInquiries.FindAsync(id);
                if (i == null)
                    return Json(new { success = false, message = "Not found" });

                i.Status = i.Status == "Open" ? "Resolved" : "Open";
                i.UpdatedBy = Uid().ToString();
                i.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + i.Status });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}