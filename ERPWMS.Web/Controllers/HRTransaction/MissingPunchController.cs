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
    public class MissingPunchController : Controller
    {
        private readonly AppDbContext _context;
        public MissingPunchController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.MissingPunchRequests
                .Include(r => r.Employee)
                .Where(r => r.IsDeleted != true)
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => new {
                    requestId = r.RequestId,
                    employeeName = r.Employee.FirstName + " " + r.Employee.LastName,
                    employeeCode = r.Employee.EmployeeCode,
                    attendanceDate = r.AttendanceDate.ToString("yyyy-MM-dd"),
                    punchType = r.PunchType,
                    requestedIn = r.RequestedInTime.HasValue ? r.RequestedInTime.Value.ToString("HH:mm") : null,
                    requestedOut = r.RequestedOutTime.HasValue ? r.RequestedOutTime.Value.ToString("HH:mm") : null,
                    reason = r.Reason,
                    status = r.Status,
                    createdDate = r.CreatedDate
                }).ToListAsync();
            return Json(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var list = await _context.Employees
                .Where(e => e.IsDeleted != true && e.IsActive == true)
                .Select(e => new { employeeId = e.EmployeeId, employeeCode = e.EmployeeCode, fullName = e.FirstName + " " + e.LastName })
                .OrderBy(e => e.fullName).ToListAsync();
            return Json(list);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int employeeId, string attendanceDate, string punchType, string? inTime, string? outTime, string reason)
        {
            try
            {
                if (employeeId <= 0 || string.IsNullOrWhiteSpace(reason))
                    return Json(new { success = false, message = "Employee and Reason required." });

                if (!DateOnly.TryParse(attendanceDate, out var date))
                    return Json(new { success = false, message = "Invalid date." });

                TimeOnly? inT = null, outT = null;
                if (!string.IsNullOrWhiteSpace(inTime) && TimeOnly.TryParse(inTime, out var ti)) inT = ti;
                if (!string.IsNullOrWhiteSpace(outTime) && TimeOnly.TryParse(outTime, out var to)) outT = to;

                _context.MissingPunchRequests.Add(new MissingPunchRequest {
                    EmployeeId = employeeId,
                    AttendanceDate = date,
                    PunchType = punchType ?? "In",
                    RequestedInTime = inT,
                    RequestedOutTime = outT,
                    Reason = reason,
                    Status = "Pending",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Request submitted!" });
            }
            catch (Exception ex) {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string? remarks)
        {
            var r = await _context.MissingPunchRequests.FindAsync(id);
            if (r == null) return Json(new { success = false, message = "Not found" });
            r.Status = "Approved";
            r.ApprovalRemarks = remarks;
            r.ApprovedBy = Uid().ToString();
            r.ApprovedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Approved!" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? remarks)
        {
            var r = await _context.MissingPunchRequests.FindAsync(id);
            if (r == null) return Json(new { success = false, message = "Not found" });
            r.Status = "Rejected";
            r.ApprovalRemarks = remarks;
            r.ApprovedBy = Uid().ToString();
            r.ApprovedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Rejected!" });
        }
    }
}
