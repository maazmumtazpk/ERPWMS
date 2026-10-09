using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Infrastructure.Services;
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
    public class LeaveRequestController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILeaveBalanceService _balance;

        public LeaveRequestController(AppDbContext c, ILeaveBalanceService balance)
        {
            _context = c; _balance = balance;
        }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.LeaveRequests
                .Include(r => r.Employee).Include(r => r.LeaveType)
                .Where(r => r.IsDeleted != true)
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => new {
                    leaveRequestId = r.LeaveRequestId,
                    employeeName = r.Employee.FirstName + " " + r.Employee.LastName,
                    employeeCode = r.Employee.EmployeeCode,
                    leaveType = r.LeaveType.LeaveName,
                    fromDate = r.FromDate.ToString("yyyy-MM-dd"),
                    toDate = r.ToDate.ToString("yyyy-MM-dd"),
                    totalDays = r.TotalDays,
                    reason = r.Reason,
                    status = r.Status
                }).ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetDropdowns()
        {
            var emps = await _context.Employees.Where(e => e.IsDeleted != true && e.IsActive == true)
                .Select(e => new { id = e.EmployeeId, name = e.EmployeeCode + " - " + e.FirstName + " " + e.LastName })
                .OrderBy(e => e.name).ToListAsync();

            var types = await _context.LeaveTypes.Where(t => t.IsDeleted != true && t.IsActive == true)
                .Select(t => new { id = t.LeaveTypeId, name = t.LeaveName + " (" + t.AnnualEntitlement + " days)" })
                .OrderBy(t => t.name).ToListAsync();

            return Json(new { employees = emps, leaveTypes = types });
        }

        [HttpGet]
        public async Task<IActionResult> GetBalances(int employeeId)
        {
            var year = DateTime.Now.Year;
            var l = await _context.LeaveBalances
                .Include(b => b.LeaveType)
                .Where(b => b.EmployeeId == employeeId && b.Year == year && b.IsDeleted != true)
                .Select(b => new {
                    leaveType = b.LeaveType.LeaveName,
                    allocated = b.Allocated,
                    used = b.Used,
                    pending = b.Pending,
                    remaining = b.Remaining
                }).ToListAsync();
            return Json(l);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int employeeId, int leaveTypeId, string fromDate, string toDate, string reason)
        {
            try
            {
                if (employeeId <= 0 || leaveTypeId <= 0) return Json(new { success = false, message = "Employee and Type required." });
                if (string.IsNullOrWhiteSpace(reason)) return Json(new { success = false, message = "Reason required." });
                if (!DateOnly.TryParse(fromDate, out var f)) return Json(new { success = false, message = "Invalid From date." });
                if (!DateOnly.TryParse(toDate, out var t)) return Json(new { success = false, message = "Invalid To date." });
                if (t < f) return Json(new { success = false, message = "To date must be >= From date." });

                int days = t.DayNumber - f.DayNumber + 1;

                var req = new LeaveRequest {
                    EmployeeId = employeeId, LeaveTypeId = leaveTypeId,
                    FromDate = f, ToDate = t, TotalDays = days,
                    Reason = reason, Status = "Pending", IsActive = true, IsDeleted = false,
                    CreatedBy = Uid().ToString(), CreatedDate = DateTime.Now
                };
                _context.LeaveRequests.Add(req);
                await _context.SaveChangesAsync();

                // Reserve balance
                await _balance.DeductAsync(employeeId, leaveTypeId, days, Uid());

                return Json(new { success = true, message = "Leave applied!" });
            } catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string? remarks)
        {
            var r = await _context.LeaveRequests.FindAsync(id);
            if (r == null) return Json(new { success = false, message = "Not found" });
            r.Status = "Approved"; r.ApprovalRemarks = remarks;
            r.ApprovedBy = Uid().ToString(); r.ApprovedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            // Confirm usage
            await _balance.ConfirmUsageAsync(r.EmployeeId, r.LeaveTypeId, r.TotalDays);
            return Json(new { success = true, message = "Approved!" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? remarks)
        {
            var r = await _context.LeaveRequests.FindAsync(id);
            if (r == null) return Json(new { success = false, message = "Not found" });
            r.Status = "Rejected"; r.ApprovalRemarks = remarks;
            r.ApprovedBy = Uid().ToString(); r.ApprovedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            // Release balance
            await _balance.ReleaseAsync(r.EmployeeId, r.LeaveTypeId, r.TotalDays);
            return Json(new { success = true, message = "Rejected!" });
        }
    }
}
