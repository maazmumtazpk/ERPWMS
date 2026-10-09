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
    public class AttendanceController : Controller
    {
        private readonly AppDbContext _context;

        public AttendanceController(AppDbContext c)
        {
            _context = c;
        }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
        }

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
            var l = await _context.EmployeeAttendances.Where(a => a.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(a => a.Status == "Present"),
                inactive = l.Count(a => a.Status == "Absent" || a.Status == "Leave"),
                addedThisMonth = l.Count(a =>
                    a.CreatedDate.HasValue &&
                    a.CreatedDate.Value.Month == DateTime.Now.Month &&
                    a.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.EmployeeAttendances
                .Include(a => a.Employee)
                .Where(a => a.IsDeleted != true)
                .OrderBy(a => a.AttendanceId)
                .Select(a => new
                {
                    attendanceId = a.AttendanceId,
                    employeeName = a.Employee != null
                        ? a.Employee.FirstName + " " + (a.Employee.LastName ?? "")
                        : "N/A",
                    attendanceDate = a.AttendanceDate,
                    checkInTime = a.CheckInTime,
                    checkOutTime = a.CheckOutTime,
                    status = a.Status ?? "Present",
                    overtimeHours = a.OvertimeHours    // ✅ decimal? as-is
                })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // GET EMPLOYEES
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var l = await _context.Employees
                .Where(e => e.IsActive == true && e.IsDeleted != true)
                .Select(e => new
                {
                    employeeId = e.EmployeeId,
                    employeeName = e.FirstName + " " + (e.LastName ?? "")
                })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // GET SHIFTS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetShifts()
        {
            try
            {
                var l = await _context.Shifts
                    .Where(s => s.IsDeleted != true)
                    .Select(s => new { shiftId = s.ShiftId, shiftName = s.ShiftName })
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
        public IActionResult AddAttendancePartial() => PartialView("_AddAttendanceForm");

        [HttpGet]
        public async Task<IActionResult> EditAttendancePartial(int id)
        {
            try
            {
                var a = await _context.EmployeeAttendances
                    .Include(x => x.Employee)
                    .FirstOrDefaultAsync(x => x.AttendanceId == id);
                if (a == null) return NotFound();
                return PartialView("_EditAttendanceForm", a);
            }
            catch
            {
                return NotFound();
            }
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int employeeId,
            DateOnly attendanceDate,
            string status,
            string? checkInTime,
            string? checkOutTime,
            decimal? overtimeHours,
            int? shiftId)
        {
            try
            {
                if (employeeId <= 0)
                    return Json(new { success = false, message = "Please select employee." });

                var existing = await _context.EmployeeAttendances
                    .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.AttendanceDate == attendanceDate && x.IsDeleted != true);
                if (existing != null)
                    return Json(new { success = false, message = "Attendance already marked for this employee on this date." });

                TimeOnly? ci = null, co = null;
                if (!string.IsNullOrEmpty(checkInTime) && TimeOnly.TryParse(checkInTime, out var t1)) ci = t1;
                if (!string.IsNullOrEmpty(checkOutTime) && TimeOnly.TryParse(checkOutTime, out var t2)) co = t2;

                _context.EmployeeAttendances.Add(new EmployeeAttendance
                {
                    EmployeeId = employeeId,
                    AttendanceDate = attendanceDate,
                    CheckInTime = ci,
                    CheckOutTime = co,
                    Status = status ?? "Present",
                    OvertimeHours = overtimeHours ?? 0,     // ✅ decimal?
                    ShiftId = shiftId,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Attendance marked!" });
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
            int attendanceId,
            string? status,
            string? checkInTime,
            string? checkOutTime,
            decimal? overtimeHours,
            int? shiftId)
        {
            try
            {
                var a = await _context.EmployeeAttendances.FindAsync(attendanceId);
                if (a == null)
                    return Json(new { success = false, message = "Not found" });

                TimeOnly? ci = null, co = null;
                if (!string.IsNullOrEmpty(checkInTime) && TimeOnly.TryParse(checkInTime, out var t1)) ci = t1;
                if (!string.IsNullOrEmpty(checkOutTime) && TimeOnly.TryParse(checkOutTime, out var t2)) co = t2;

                a.Status = status ?? "Present";
                a.CheckInTime = ci;
                a.CheckOutTime = co;
                a.OvertimeHours = overtimeHours ?? 0;   // ✅ decimal?
                a.ShiftId = shiftId;
                a.UpdatedBy = Uid().ToString();
                a.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Attendance updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
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
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------------------------------------------------------
        // TOGGLE (Present <-> Absent)
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var a = await _context.EmployeeAttendances.FindAsync(id);
                if (a == null)
                    return Json(new { success = false, message = "Not found" });

                a.Status = a.Status == "Present" ? "Absent" : "Present";
                a.UpdatedBy = Uid().ToString();
                a.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + a.Status });
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
                var a = await _context.EmployeeAttendances
                    .Include(x => x.Employee)
                    .FirstOrDefaultAsync(x => x.AttendanceId == id);

                if (a == null) return NotFound();

                // ✅ CreatedBy string hai
                var creator = await _context.Users.FirstOrDefaultAsync(u => u.UserName == a.CreatedBy);
                ViewBag.CreatedByName = creator != null ? creator.UserName : "N/A";

                // ✅ UpdatedBy string hai
                if (!string.IsNullOrWhiteSpace(a.UpdatedBy))
                {
                    var upd = await _context.Users.FirstOrDefaultAsync(u => u.UserName == a.UpdatedBy);
                    ViewBag.UpdatedByName = upd != null ? upd.UserName : "N/A";
                }
                else
                {
                    ViewBag.UpdatedByName = "-";
                }

                return View("Profile", a);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
                return RedirectToAction("Index");
            }
        }
    }
}