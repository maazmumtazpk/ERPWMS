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
    public class CustomerFeedbackController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerFeedbackController(AppDbContext c)
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
            var l = await _context.CustomerFeedbacks.ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(f => f.Rating >= 4),
                inactive = l.Count(f => f.Rating < 4),
                addedThisMonth = l.Count(f =>
                    f.CreatedDate.HasValue &&
                    f.CreatedDate.Value.Month == DateTime.Now.Month &&
                    f.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // ============================================================
        // GET ALL
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.CustomerFeedbacks
                .Include(f => f.Customer)
                .OrderBy(f => f.FeedbackId)
                .Select(f => new
                {
                    feedbackId = f.FeedbackId,
                    customerName = f.Customer != null ? f.Customer.CustomerName : "N/A",
                    rating = f.Rating,                          // ✅ int? direct
                    comments = f.Comments ?? "",
                    category = f.Category ?? "General",
                    feedbackDate = f.FeedbackDate,
                    createdDate = f.CreatedDate
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
        public IActionResult AddFeedbackPartial() => PartialView("_AddFeedbackForm");

        [HttpGet]
        public async Task<IActionResult> EditFeedbackPartial(int id)
        {
            var f = await _context.CustomerFeedbacks.FindAsync(id);
            if (f == null) return NotFound();
            return PartialView("_EditFeedbackForm", f);
        }

        // ============================================================
        // REGISTER
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int customerId, int rating, string? comments, string? category)
        {
            try
            {
                if (customerId <= 0)
                    return Json(new { success = false, message = "Please select customer." });

                _context.CustomerFeedbacks.Add(new CustomerFeedback
                {
                    CustomerId = customerId,
                    Rating = rating,
                    Comments = comments ?? "",
                    Category = category ?? "General",
                    FeedbackDate = DateTime.Now,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Feedback added!" });
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
        public async Task<IActionResult> Edit(int feedbackId, int rating, string? comments, string? category)
        {
            try
            {
                var f = await _context.CustomerFeedbacks.FindAsync(feedbackId);
                if (f == null)
                    return Json(new { success = false, message = "Not found" });

                f.Rating = rating;
                f.Comments = comments;
                f.Category = category;
                f.CreatedDate = DateTime.Now;
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
                var f = await _context.CustomerFeedbacks.FindAsync(id);
                if (f == null)
                    return Json(new { success = false, message = "Not found" });

                _context.CustomerFeedbacks.Remove(f);
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
                var f = await _context.CustomerFeedbacks.FindAsync(id);
                if (f == null)
                    return Json(new { success = false, message = "Not found" });

                // ✅ FIX: Rating int? hai ya int — safe check
                int current = f.Rating ?? 0;
                f.Rating = current >= 4 ? 3 : 5;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Rating: " + f.Rating });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}