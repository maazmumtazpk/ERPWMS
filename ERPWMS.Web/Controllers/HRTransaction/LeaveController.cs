using ERPWMS.Database;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPWMS.Web.Controllers.HRTransaction
{
    [Authorize]
    public class LeaveController : Controller
    {
        private readonly AppDbContext _context;
        public LeaveController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(cl, out var id) || id <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return id;
        }

        public IActionResult Index() => View();

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
                    addedThisMonth = l.Count(x => x.CreatedDate.HasValue &&
                        x.CreatedDate.Value.Month == DateTime.Now.Month &&
                        x.CreatedDate.Value.Year == DateTime.Now.Year)
                });
            }
            catch { return Json(new { total = 0, approved = 0, pending = 0, addedThisMonth = 0 }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _context.EmployeeLeaves.Include(x => x.Employee)
                    .Where(x => x.IsDeleted != true)
                    .OrderByDescending(x => x.LeaveId)
                    .ToListAsync();

                var l = data.Select(x => new {
                    leaveId = x.LeaveId,
                    employeeName = x.Employee != null
                        ? (x.Employee.FirstName + " " + (x.Employee.LastName ?? "")).Trim() : "N/A",
                    employeeCode = x.Employee?.EmployeeCode ?? "-",
                    leaveType = x.LeaveType ?? "Casual",
                    fromDate = x.FromDate.ToString("dd MMM yyyy"),
                    toDate = x.ToDate.ToString("dd MMM yyyy"),
                    totalDays = x.TotalDays,
                    reason = x.Reason ?? "-",
                    status = x.Status ?? "Pending"
                }).ToList();

                return Json(l);
            }
            catch { return Json(new System.Collections.Generic.List<object>()); }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees() =>
            Json(await _context.Employees.Where(e => e.IsActive == true && e.IsDeleted != true)
                .Select(e => new {
                    employeeId = e.EmployeeId,
                    employeeName = (e.FirstName + " " + (e.LastName ?? "")).Trim()
                }).ToListAsync());

        [HttpGet]
        public IActionResult AddLeavePartial() => PartialView("_AddLeaveForm");

        [HttpGet]
        public async Task<IActionResult> EditLeavePartial(int id)
        {
            var l = await _context.EmployeeLeaves.Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.LeaveId == id);
            if (l == null) return NotFound();
            return PartialView("_EditLeaveForm", l);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int employeeId, string? leaveType,
            DateOnly fromDate, DateOnly toDate, string? reason)
        {
            try
            {
                if (employeeId <= 0)
                    return Json(new { success = false, message = "Please select employee." });

                if (toDate < fromDate)
                    return Json(new { success = false, message = "To date must be >= From date." });

                int serverTotalDays = toDate.DayNumber - fromDate.DayNumber + 1;
                if (serverTotalDays <= 0 || serverTotalDays > 90)
                    return Json(new { success = false, message = "Invalid duration (1-90 days)." });

                var today = DateOnly.FromDateTime(DateTime.Today);
                if (fromDate < today.AddDays(-30))
                    return Json(new { success = false, message = "Cannot apply for leave older than 30 days." });

                var overlap = await _context.EmployeeLeaves
                    .AnyAsync(x => x.EmployeeId == employeeId
                        && x.IsDeleted != true
                        && x.Status != "Rejected"
                        && (x.FromDate <= toDate && x.ToDate >= fromDate));
                if (overlap)
                    return Json(new { success = false, message = "Overlapping leave exists." });

                _context.EmployeeLeaves.Add(new EmployeeLeave
                {
                    EmployeeId = employeeId,
                    LeaveType = leaveType ?? "Casual",
                    FromDate = fromDate,
                    ToDate = toDate,
                    TotalDays = serverTotalDays,
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
                System.Diagnostics.Debug.WriteLine(ex);
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int leaveId, string? leaveType,
            DateOnly fromDate, DateOnly toDate, string? reason)
        {
            try
            {
                var l = await _context.EmployeeLeaves.FindAsync(leaveId);
                if (l == null)
                    return Json(new { success = false, message = "Not found" });

                if (l.Status != "Pending" && l.Status != null)
                    return Json(new { success = false, message = "Only pending leaves can be edited." });

                if (toDate < fromDate)
                    return Json(new { success = false, message = "To date must be >= From date." });

                l.LeaveType = leaveType;
                l.FromDate = fromDate;
                l.ToDate = toDate;
                l.TotalDays = toDate.DayNumber - fromDate.DayNumber + 1;
                l.Reason = reason;
                l.UpdatedBy = Uid().ToString();
                l.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Leave updated!" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var l = await _context.EmployeeLeaves.FindAsync(id);
                if (l == null)
                    return Json(new { success = false, message = "Not found" });

                if (l.Status == "Approved")
                    return Json(new { success = false, message = "Approved leaves cannot be deleted." });

                l.IsDeleted = true;
                l.DeletedBy = Uid().ToString();
                l.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Approve")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var l = await _context.EmployeeLeaves.FindAsync(id);
                if (l == null)
                    return Json(new { success = false, message = "Not found" });

                var currentUid = Uid().ToString();

                if (l.Status == "Pending" || l.Status == null)
                {
                    if (l.CreatedBy == currentUid)
                        return Json(new { success = false, message = "You cannot approve your own leave request." });

                    l.Status = "Approved";
                    l.ApprovedBy = currentUid;
                    l.ApprovedDate = DateTime.Now;
                }
                else if (l.Status == "Approved")
                {
                    if (l.ApprovedBy == currentUid)
                        return Json(new { success = false, message = "You cannot reject your own approval." });

                    l.Status = "Rejected";
                    l.RejectionReason = "Rejected by admin";
                }
                else
                {
                    l.Status = "Pending";
                    l.ApprovedBy = null;
                    l.ApprovedDate = null;
                }

                l.UpdatedBy = currentUid;
                l.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + l.Status });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var l = await _context.EmployeeLeaves.Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.LeaveId == id);
            if (l == null) return NotFound();

            if (int.TryParse(l.CreatedBy, out int cid))
            {
                var c = await _context.Users.FindAsync(cid);

            }


            if (int.TryParse(l.ApprovedBy, out int aid))
            {
                var a = await _context.Users.FindAsync(aid);
                ViewBag.ApprovedByName = a?.UserName ?? "-";
            }
            else ViewBag.ApprovedByName = "-";

            if (int.TryParse(l.UpdatedBy, out int uid))
            {
                var u = await _context.Users.FindAsync(uid);

            }


            return View("Profile", l);
        }
    }
}

