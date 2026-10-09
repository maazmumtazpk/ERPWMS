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

namespace ERPWMS.Web.Controllers.HRTransaction
{
    [Authorize]
    public class LeaveController : Controller
    {
        private readonly AppDbContext _context;

        public LeaveController(AppDbContext c)
        {
            _context = c;
        }

        private int Uid() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 1;

        // -------------------------------------------------------
        // INDEX
        // -------------------------------------------------------
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
                var l = await _context.EmployeeLeaves.Where(x => x.IsDeleted != true).ToListAsync();
                return Json(new
                {
                    total = l.Count,
                    approved = l.Count(x => x.Status == "Approved"),
                    pending = l.Count(x => x.Status == "Pending" || x.Status == null),
                    addedThisMonth = l.Count(x =>
                        x.CreatedDate.HasValue &&
                        x.CreatedDate.Value.Month == DateTime.Now.Month &&
                        x.CreatedDate.Value.Year == DateTime.Now.Year)
                });
            }
            catch
            {
                return Json(new { total = 0, approved = 0, pending = 0, addedThisMonth = 0 });
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
                var data = await _context.EmployeeLeaves
                    .Include(x => x.Employee)
                    .Where(x => x.IsDeleted != true)
                    .OrderBy(x => x.LeaveId)
                    .ToListAsync();

                var l = data.Select(x => new
                {
                    leaveId = x.LeaveId,
                    employeeName = x.Employee != null
                        ? (x.Employee.FirstName + " " + (x.Employee.LastName ?? "")).Trim()
                        : "N/A",
                    employeeCode = x.Employee != null ? x.Employee.EmployeeCode : "-",
                    leaveType = x.LeaveType ?? "Casual",
                    fromDate = x.FromDate.ToString("dd MMM yyyy"),
                    toDate = x.ToDate.ToString("dd MMM yyyy"),
                    totalDays = x.TotalDays,
                    reason = x.Reason ?? "-",
                    status = x.Status ?? "Pending"
                }).ToList();

                return Json(l);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // -------------------------------------------------------
        // GET EMPLOYEES
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var l = await _context.Employees
                    .Where(e => e.IsActive == true && e.IsDeleted != true)
                    .Select(e => new
                    {
                        employeeId = e.EmployeeId,
                        employeeName = (e.FirstName + " " + (e.LastName ?? "")).Trim()
                    })
                    .ToListAsync();
                return Json(l);
            }
            catch
            {
                return Json(new List<object>());
            }
        }

        // -------------------------------------------------------
        // ADD / EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult AddLeavePartial() => PartialView("_AddLeaveForm");

        [HttpGet]
        public async Task<IActionResult> EditLeavePartial(int id)
        {
            var l = await _context.EmployeeLeaves
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.LeaveId == id);
            if (l == null) return NotFound();
            return PartialView("_EditLeaveForm", l);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int employeeId,
            string? leaveType,
            DateOnly fromDate,
            DateOnly toDate,
            int totalDays,
            string? reason)
        {
            try
            {
                if (employeeId <= 0)
                    return Json(new { success = false, message = "Please select employee." });
                if (totalDays <= 0)
                    return Json(new { success = false, message = "Total days must be greater than 0." });

                var duplicate = await _context.EmployeeLeaves
                    .AnyAsync(x => x.EmployeeId == employeeId
                        && x.FromDate == fromDate
                        && x.ToDate == toDate
                        && x.IsDeleted != true);
                if (duplicate)
                    return Json(new { success = false, message = "Leave already exists for this employee with same dates." });

                _context.EmployeeLeaves.Add(new EmployeeLeave
                {
                    EmployeeId = employeeId,
                    LeaveType = leaveType ?? "Casual",
                    FromDate = fromDate,
                    ToDate = toDate,
                    TotalDays = totalDays,
                    Reason = reason ?? "",
                    Status = "Pending",
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Leave request submitted!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // -------------------------------------------------------
        // EDIT
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int leaveId,
            string? leaveType,
            DateOnly fromDate,
            DateOnly toDate,
            int totalDays,
            string? reason,
            string? status)
        {
            try
            {
                var l = await _context.EmployeeLeaves.FindAsync(leaveId);
                if (l == null)
                    return Json(new { success = false, message = "Not found" });

                l.LeaveType = leaveType;
                l.FromDate = fromDate;
                l.ToDate = toDate;
                l.TotalDays = totalDays;
                l.Reason = reason;
                l.Status = status;
                l.UpdatedBy = Uid().ToString();
                l.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Leave updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------------------------------------------------------
        // DELETE
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var l = await _context.EmployeeLeaves.FindAsync(id);
                if (l == null)
                    return Json(new { success = false, message = "Not found" });

                l.IsDeleted = true;
                l.DeletedBy = Uid().ToString();
                l.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------------------------------------------------------
        // TOGGLE (Pending -> Approved -> Rejected -> Pending)
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var l = await _context.EmployeeLeaves.FindAsync(id);
                if (l == null)
                    return Json(new { success = false, message = "Not found" });

                if (l.Status == "Pending" || l.Status == null)
                {
                    l.Status = "Approved";
                    l.ApprovedBy = Uid().ToString();
                    l.ApprovedDate = DateTime.Now;
                }
                else if (l.Status == "Approved")
                {
                    l.Status = "Rejected";
                    l.RejectionReason = "Rejected by admin";
                }
                else
                {
                    l.Status = "Pending";
                    l.ApprovedBy = null;
                    l.ApprovedDate = null;
                }

                l.UpdatedBy = Uid().ToString();
                l.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + l.Status });
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
            try
            {
                var l = await _context.EmployeeLeaves
                    .Include(x => x.Employee)
                    .FirstOrDefaultAsync(x => x.LeaveId == id);
                if (l == null) return NotFound();

                var c = await _context.Users.FindAsync(l.CreatedBy);
                ViewBag.CreatedByName = c != null ? c.UserName : "N/A";

                // ✅ ApprovedBy string hai
                if (!string.IsNullOrWhiteSpace(l.ApprovedBy))
                {
                    var a = await _context.Users.FindAsync(l.ApprovedBy);
                    ViewBag.ApprovedByName = a != null ? a.UserName : "N/A";
                }
                else
                {
                    ViewBag.ApprovedByName = "-";
                }

                // ✅ UpdatedBy string hai
                if (!string.IsNullOrWhiteSpace(l.UpdatedBy))
                {
                    var u = await _context.Users.FindAsync(l.UpdatedBy);
                    ViewBag.UpdatedByName = u != null ? u.UserName : "N/A";
                }
                else
                {
                    ViewBag.UpdatedByName = "-";
                }

                return View("Profile", l);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
                return RedirectToAction("Index");
            }
        }
    }
}