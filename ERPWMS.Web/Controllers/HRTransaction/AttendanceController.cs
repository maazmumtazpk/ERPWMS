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
    public class AttendanceController : Controller
    {
        private readonly AppDbContext _context;
        public AttendanceController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }

        private const int MaxDaysBack = 30;
        private const decimal MaxOTHours = 16m;
        private const decimal StandardHours = 8m;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var l = await _context.EmployeeAttendances.Where(a => a.IsDeleted != true).ToListAsync();
            var now = DateTime.Now;
            return Json(new
            {
                total = l.Count,
                active = l.Count(a => a.Status == "Present"),
                inactive = l.Count(a => a.Status == "Absent" || a.Status == "Leave"),
                addedThisMonth = l.Count(a => a.CreatedDate.HasValue &&
                    a.CreatedDate.Value.Month == now.Month &&
                    a.CreatedDate.Value.Year == now.Year)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.EmployeeAttendances
                .Include(a => a.Employee)
                .Where(a => a.IsDeleted != true)
                .OrderByDescending(a => a.AttendanceDate)
                .Select(a => new {
                    attendanceId = a.AttendanceId,
                    employeeName = a.Employee != null
                        ? a.Employee.FirstName + " " + (a.Employee.LastName ?? "")
                        : "N/A",
                    attendanceDate = a.AttendanceDate,
                    checkInTime = a.CheckInTime,
                    checkOutTime = a.CheckOutTime,
                    status = a.Status ?? "Present",
                    overtimeHours = a.OvertimeHours
                })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees() =>
            Json(await _context.Employees
                .Where(e => e.IsActive == true && e.IsDeleted != true)
                .Select(e => new {
                    employeeId = e.EmployeeId,
                    employeeName = e.FirstName + " " + (e.LastName ?? "")
                }).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> GetShifts()
        {
            try
            {
                return Json(await _context.Shifts
                    .Where(s => s.IsDeleted != true)
                    .Select(s => new { shiftId = s.ShiftId, shiftName = s.ShiftName })
                    .ToListAsync());
            }
            catch { return Json(new System.Collections.Generic.List<object>()); }
        }

        [HttpGet]
        public IActionResult AddAttendancePartial() => PartialView("_AddAttendanceForm");

        [HttpGet]
        public async Task<IActionResult> EditAttendancePartial(int id)
        {
            try
            {
                var a = await _context.EmployeeAttendances.Include(x => x.Employee)
                    .FirstOrDefaultAsync(x => x.AttendanceId == id);
                if (a == null) return NotFound();
                return PartialView("_EditAttendanceForm", a);
            }
            catch { return NotFound(); }
        }

        // Helper: calculate OT from check-in/check-out
        private static decimal CalculateOT(TimeOnly? ci, TimeOnly? co, string status)
        {
            if (status != "Present" || !ci.HasValue || !co.HasValue) return 0;
            var duration = co.Value.ToTimeSpan() - ci.Value.ToTimeSpan();
            var hours = duration.TotalHours;
            if (hours < 0) hours += 24;
            if (hours <= (double)StandardHours) return 0;
            var ot = Math.Round((decimal)(hours - (double)StandardHours), 2);
            if (ot > MaxOTHours) ot = MaxOTHours;
            if (ot < 0) ot = 0;
            return ot;
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Edit")]
        public async Task<IActionResult> Register(
            int employeeId, DateOnly attendanceDate, string status,
            string? checkInTime, string? checkOutTime, int? shiftId)
        {
            try
            {
                if (employeeId <= 0)
                    return Json(new { success = false, message = "Please select employee." });

                var today = DateOnly.FromDateTime(DateTime.Today);
                if (attendanceDate > today)
                    return Json(new { success = false, message = "Future date not allowed." });
                if (attendanceDate < today.AddDays(-MaxDaysBack))
                    return Json(new { success = false, message = $"Cannot mark attendance older than {MaxDaysBack} days." });

                var allowedStatuses = new[] { "Present", "Absent", "Leave", "HalfDay", "Holiday" };
                if (string.IsNullOrWhiteSpace(status) || !allowedStatuses.Contains(status))
                    return Json(new { success = false, message = "Invalid status." });

                var existing = await _context.EmployeeAttendances
                    .FirstOrDefaultAsync(x => x.EmployeeId == employeeId
                        && x.AttendanceDate == attendanceDate && x.IsDeleted != true);
                if (existing != null)
                    return Json(new { success = false, message = "Attendance already marked." });

                TimeOnly? ci = null, co = null;
                if (!string.IsNullOrEmpty(checkInTime) && TimeOnly.TryParse(checkInTime, out var t1)) ci = t1;
                if (!string.IsNullOrEmpty(checkOutTime) && TimeOnly.TryParse(checkOutTime, out var t2)) co = t2;

                decimal serverOT = CalculateOT(ci, co, status);

                _context.EmployeeAttendances.Add(new EmployeeAttendance
                {
                    EmployeeId = employeeId,
                    AttendanceDate = attendanceDate,
                    CheckInTime = ci,
                    CheckOutTime = co,
                    Status = status,
                    OvertimeHours = serverOT,
                    ShiftId = shiftId,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"Attendance marked! OT: {serverOT}h" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Edit")]
        public async Task<IActionResult> Edit(
            int attendanceId, string? status,
            string? checkInTime, string? checkOutTime, int? shiftId)
        {
            try
            {
                var a = await _context.EmployeeAttendances.FindAsync(attendanceId);
                if (a == null)
                    return Json(new { success = false, message = "Not found" });

                var today = DateOnly.FromDateTime(DateTime.Today);
                if (a.AttendanceDate < today.AddDays(-MaxDaysBack))
                    return Json(new { success = false, message = "Attendance locked (older than 30 days)." });

                TimeOnly? ci = null, co = null;
                if (!string.IsNullOrEmpty(checkInTime) && TimeOnly.TryParse(checkInTime, out var t1)) ci = t1;
                if (!string.IsNullOrEmpty(checkOutTime) && TimeOnly.TryParse(checkOutTime, out var t2)) co = t2;

                a.Status = status ?? "Present";
                a.CheckInTime = ci;
                a.CheckOutTime = co;
                a.ShiftId = shiftId;
                a.OvertimeHours = CalculateOT(ci, co, a.Status);
                a.UpdatedBy = Uid().ToString();
                a.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Attendance updated!" });
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
                var a = await _context.EmployeeAttendances.FindAsync(id);
                if (a == null)
                    return Json(new { success = false, message = "Not found" });

                a.IsDeleted = true;
                a.UpdatedBy = Uid().ToString();
                a.UpdatedDate = DateTime.Now;
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
                var a = await _context.EmployeeAttendances.FindAsync(id);
                if (a == null)
                    return Json(new { success = false, message = "Not found" });

                var today = DateOnly.FromDateTime(DateTime.Today);
                if (a.AttendanceDate < today.AddDays(-MaxDaysBack))
                    return Json(new { success = false, message = "Attendance locked." });

                a.Status = a.Status == "Present" ? "Absent" : "Present";
                a.OvertimeHours = CalculateOT(a.CheckInTime, a.CheckOutTime, a.Status);
                a.UpdatedBy = Uid().ToString();
                a.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + a.Status });
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
            var a = await _context.EmployeeAttendances.Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.AttendanceId == id);
            if (a == null) return NotFound();

            if (int.TryParse(a.CreatedBy, out int cid))
            {
                var c = await _context.Users.FindAsync(cid);
                ViewBag.CreatedByName = c?.UserName ?? "-";
            }
            else ViewBag.CreatedByName = "-";

            if (int.TryParse(a.UpdatedBy, out int uid))
            {
                var u = await _context.Users.FindAsync(uid);
                ViewBag.UpdatedByName = u?.UserName ?? "-";
            }
            else ViewBag.UpdatedByName = "-";

            return View("Profile", a);
        }
    }
}
