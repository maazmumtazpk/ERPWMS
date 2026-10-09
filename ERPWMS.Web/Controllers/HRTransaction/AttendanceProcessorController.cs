using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Interfaces;
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
    public class AttendanceProcessorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAttendanceProcessor _processor;

        public AttendanceProcessorController(AppDbContext c, IAttendanceProcessor processor)
        {
            _context = c;
            _processor = processor;
        }

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
        public async Task<IActionResult> GetEmployees()
        {
            // Employee + Shift info (StartTime, EndTime)
            var list = await (from e in _context.Employees
                              where e.IsDeleted != true && e.IsActive == true
                              join s in _context.Shifts on e.ShiftId equals s.ShiftId into shiftJoin
                              from s in shiftJoin.DefaultIfEmpty()
                              orderby e.FirstName, e.LastName
                              select new
                              {
                                  employeeId = e.EmployeeId,
                                  employeeCode = e.EmployeeCode,
                                  fullName = e.FirstName + " " + e.LastName,
                                  shiftId = e.ShiftId,
                                  shiftName = s != null ? s.ShiftName : null,
                                  startTime = s != null ? (TimeOnly?)s.StartTime : null,
                                  endTime = s != null ? (TimeOnly?)s.EndTime : null
                              }).ToListAsync();

            // Convert TimeOnly to HH:mm string for JSON
            var result = list.Select(e => new
            {
                employeeId = e.employeeId,
                employeeCode = e.employeeCode,
                fullName = e.fullName,
                shiftId = e.shiftId,
                shiftName = e.shiftName,
                startTime = e.startTime.HasValue ? e.startTime.Value.ToString("HH:mm") : null,
                endTime = e.endTime.HasValue ? e.endTime.Value.ToString("HH:mm") : null
            }).ToList();

            return Json(result);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Punch(
            int employeeId,
            string attendanceDate,
            string? checkIn,
            string? checkOut)
        {
            try
            {
                if (employeeId <= 0)
                    return Json(new { success = false, message = "Employee required." });

                if (!DateOnly.TryParse(attendanceDate, out var date))
                    return Json(new { success = false, message = "Invalid date." });

                TimeOnly? inT = null;
                if (!string.IsNullOrWhiteSpace(checkIn) && TimeOnly.TryParse(checkIn, out var ti))
                    inT = ti;

                TimeOnly? outT = null;
                if (!string.IsNullOrWhiteSpace(checkOut) && TimeOnly.TryParse(checkOut, out var to))
                    outT = to;

                var input = new AttendanceInput
                {
                    EmployeeId = employeeId,
                    Date = date,
                    CheckIn = inT,
                    CheckOut = outT,
                    CurrentUserId = Uid()
                };

                var result = await _processor.ProcessAsync(input);

                if (!result.Success)
                    return Json(new { success = false, message = result.Message });

                return Json(new
                {
                    success = true,
                    message = result.Message,
                    status = result.Status,
                    lateMinutes = result.LateMinutes,
                    earlyMinutes = result.EarlyMinutes,
                    otHours = result.OTHours,
                    workingMinutes = result.WorkingMinutes,
                    isHalfDay = result.IsHalfDay
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTodayAttendance()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var list = await _context.EmployeeAttendances
                .Include(a => a.Employee)
                .Where(a => a.AttendanceDate == today)
                .OrderByDescending(a => a.AttendanceId)
                .Select(a => new
                {
                    attendanceId = a.AttendanceId,
                    employeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                    employeeCode = a.Employee.EmployeeCode,
                    checkIn = a.CheckInTime.HasValue ? a.CheckInTime.Value.ToString("HH:mm") : "-",
                    checkOut = a.CheckOutTime.HasValue ? a.CheckOutTime.Value.ToString("HH:mm") : "-",
                    lateMinutes = a.LateMinutes ?? 0,
                    earlyMinutes = a.EarlyMinutes ?? 0,
                    otHours = a.OTHours ?? 0,
                    workingMinutes = a.WorkingMinutes ?? 0,
                    status = a.Status ?? "Unknown",
                    isHalfDay = a.IsHalfDay ?? false
                })
                .ToListAsync();
            return Json(list);
        }
    }
}
