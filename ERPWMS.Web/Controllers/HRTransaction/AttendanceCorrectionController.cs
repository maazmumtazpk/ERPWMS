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
    public class AttendanceCorrectionController : Controller
    {
        private readonly AppDbContext _context;
        public AttendanceCorrectionController(AppDbContext c) { _context = c; }

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
            var list = await _context.AttendanceCorrectionRequests
                .Include(r => r.Employee)
                .Where(r => r.IsDeleted != true)
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => new {
                    correctionId = r.CorrectionId,
                    employeeName = r.Employee.FirstName + " " + r.Employee.LastName,
                    employeeCode = r.Employee.EmployeeCode,
                    attendanceDate = r.AttendanceDate.ToString("yyyy-MM-dd"),
                    originalIn = r.OriginalInTime.HasValue ? r.OriginalInTime.Value.ToString("HH:mm") : null,
                    originalOut = r.OriginalOutTime.HasValue ? r.OriginalOutTime.Value.ToString("HH:mm") : null,
                    requestedIn = r.RequestedInTime.HasValue ? r.RequestedInTime.Value.ToString("HH:mm") : null,
                    requestedOut = r.RequestedOutTime.HasValue ? r.RequestedOutTime.Value.ToString("HH:mm") : null,
                    reason = r.Reason,
                    status = r.Status
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
        public async Task<IActionResult> Submit(int employeeId, string attendanceDate, string? newIn, string? newOut, string reason)
        {
            try
            {
                if (employeeId <= 0 || string.IsNullOrWhiteSpace(reason))
                    return Json(new { success = false, message = "Employee and Reason required." });

                if (!DateOnly.TryParse(attendanceDate, out var date))
                    return Json(new { success = false, message = "Invalid date." });

                var att = await _context.EmployeeAttendances
                    .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.AttendanceDate == date);

                TimeOnly? newInT = null, newOutT = null;
                if (!string.IsNullOrWhiteSpace(newIn) && TimeOnly.TryParse(newIn, out var ti)) newInT = ti;
                if (!string.IsNullOrWhiteSpace(newOut) && TimeOnly.TryParse(newOut, out var to)) newOutT = to;

                _context.AttendanceCorrectionRequests.Add(new AttendanceCorrectionRequest {
                    EmployeeId = employeeId,
                    AttendanceDate = date,
                    OriginalInTime = att?.CheckInTime,
                    OriginalOutTime = att?.CheckOutTime,
                    RequestedInTime = newInT,
                    RequestedOutTime = newOutT,
                    Reason = reason,
                    Status = "Pending",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Correction request submitted!" });
            }
            catch (Exception ex) {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string? remarks)
        {
            var r = await _context.AttendanceCorrectionRequests.FindAsync(id);
            if (r == null) return Json(new { success = false, message = "Not found" });

            r.Status = "Approved";
            r.ApprovalRemarks = remarks;
            r.ApprovedBy = Uid().ToString();
            r.ApprovedDate = DateTime.Now;

            // Apply correction to actual attendance
            var att = await _context.EmployeeAttendances
                .FirstOrDefaultAsync(a => a.EmployeeId == r.EmployeeId && a.AttendanceDate == r.AttendanceDate);
            if (att != null) {
                if (r.RequestedInTime.HasValue) att.CheckInTime = r.RequestedInTime;
                if (r.RequestedOutTime.HasValue) att.CheckOutTime = r.RequestedOutTime;
                att.UpdatedBy = Uid().ToString();
                att.UpdatedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Approved and applied!" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? remarks)
        {
            var r = await _context.AttendanceCorrectionRequests.FindAsync(id);
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
