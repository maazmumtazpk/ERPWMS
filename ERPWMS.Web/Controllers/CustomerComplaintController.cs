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
    public class CustomerComplaintController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerComplaintController(AppDbContext c)
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
            var l = await _context.CustomerComplaints.ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(c => c.Status == "Resolved"),
                inactive = l.Count(c => c.Status == "Open" || c.Status == "Pending"),
                addedThisMonth = l.Count(c =>
                    c.CreatedDate.HasValue &&
                    c.CreatedDate.Value.Month == DateTime.Now.Month &&
                    c.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // ============================================================
        // GET ALL
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.CustomerComplaints
                .Include(c => c.Customer)
                .OrderBy(c => c.ComplaintId)
                .Select(c => new
                {
                    complaintId = c.ComplaintId,
                    customerName = c.Customer != null ? c.Customer.CustomerName : "N/A",
                    complaintType = c.ComplaintType ?? "General",
                    description = c.Description,
                    status = c.Status ?? "Open",
                    createdDate = c.CreatedDate
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
        public IActionResult AddComplaintPartial() => PartialView("_AddComplaintForm");

        [HttpGet]
        public async Task<IActionResult> EditComplaintPartial(int id)
        {
            var c = await _context.CustomerComplaints.FindAsync(id);
            if (c == null) return NotFound();
            return PartialView("_EditComplaintForm", c);
        }

        // ============================================================
        // REGISTER
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int customerId, string? complaintType, string? description)
        {
            try
            {
                if (customerId <= 0)
                    return Json(new { success = false, message = "Please select customer." });

                _context.CustomerComplaints.Add(new CustomerComplaint
                {
                    CustomerId = customerId,
                    ComplaintType = complaintType ?? "General",
                    Description = description ?? "",
                    Status = "Open",
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Complaint registered!" });
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
        public async Task<IActionResult> Edit(int complaintId, string? description, string? status)
        {
            try
            {
                var c = await _context.CustomerComplaints.FindAsync(complaintId);
                if (c == null)
                    return Json(new { success = false, message = "Not found" });

                c.Description = description;
                c.Status = status;
                c.UpdatedBy = Uid().ToString();
                c.UpdatedDate = DateTime.Now;
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
                var c = await _context.CustomerComplaints.FindAsync(id);
                if (c == null)
                    return Json(new { success = false, message = "Not found" });

                _context.CustomerComplaints.Remove(c);
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
                var c = await _context.CustomerComplaints.FindAsync(id);
                if (c == null)
                    return Json(new { success = false, message = "Not found" });

                c.Status = c.Status == "Open" ? "Resolved" : "Open";
                c.UpdatedBy = Uid().ToString();
                c.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + c.Status });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}