using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Constants;
using ERPWMS.Domain.Interfaces;

namespace ERPWMS.Infrastructure.Services
{
    public class AttendanceProcessor : IAttendanceProcessor
    {
        private readonly AppDbContext _context;

        public AttendanceProcessor(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AttendanceResult> ProcessAsync(AttendanceInput input)
        {
            var result = new AttendanceResult { Success = false };

            // 1. Employee find
            var emp = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == input.EmployeeId);

            if (emp == null)
            {
                result.Message = "Employee not found.";
                return result;
            }

            // 2. Settings load
            var settings = await _context.PayrollSettings
                .OrderByDescending(p => p.SettingId)
                .FirstOrDefaultAsync();

            int grace = settings?.GracePeriodMinutes ?? 15;
            int lateThreshold = settings?.LateThresholdMinutes ?? 30;
            int earlyThreshold = settings?.EarlyThresholdMinutes ?? 30;
            int workingDays = settings?.WorkingDays ?? 26;

            // 3. Holiday check
            var isHoliday = await _context.Holidays
                .AnyAsync(h => h.HolidayDate == input.Date
                    && h.IsDeleted != true
                    && h.IsActive == true
                    && (h.BranchId == null || h.BranchId == emp.BranchId));

            // 4. Weekly off check
            var weeklyOff = await _context.WeeklyOffs
                .Where(w => w.EmployeeId == input.EmployeeId
                    && w.IsActive == true
                    && w.IsDeleted != true
                    && w.EffectiveFrom <= input.Date
                    && (w.EffectiveTo == null || w.EffectiveTo >= input.Date))
                .OrderByDescending(w => w.EffectiveFrom)
                .FirstOrDefaultAsync();

            var dayName = input.Date.DayOfWeek.ToString();
            var isWeeklyOff = weeklyOff != null
                && weeklyOff.DayOfWeek != "None"
                && string.Equals(weeklyOff.DayOfWeek, dayName, StringComparison.OrdinalIgnoreCase);

            // 5. Shift load
            Shift? shift = null;
            if (input.ShiftId.HasValue)
            {
                shift = await _context.Shifts
                    .FirstOrDefaultAsync(s => s.ShiftId == input.ShiftId.Value);
            }
            else if (emp.ShiftId.HasValue)
            {
                shift = await _context.Shifts
                    .FirstOrDefaultAsync(s => s.ShiftId == emp.ShiftId.Value);
            }

            // 6. Punch missing?
            if (!input.CheckIn.HasValue)
            {
                result.Status = AttendanceStatuses.Absent;
                result.Message = "No Check-In recorded — marked Absent.";
                result.IsHolidayWork = false;
                result.IsWeeklyOffWork = false;
                await SaveOrUpdateAttendance(input, result, isHoliday, isWeeklyOff);
                result.Success = true;
                return result;
            }

            var inTime = input.CheckIn.Value;

            // 7. Working minutes
            int workingMinutes = 0;
            if (input.CheckOut.HasValue)
            {
                var outTime = input.CheckOut.Value;
                var span = outTime.ToTimeSpan() - inTime.ToTimeSpan();
                if (span.TotalMinutes < 0) span = span.Add(TimeSpan.FromHours(24));
                workingMinutes = (int)span.TotalMinutes;
            }
            result.WorkingMinutes = workingMinutes;

            // 8. Late calculation
            int lateMinutes = 0;
            if (shift != null)
            {
                var scheduledStart = shift.StartTime.ToTimeSpan();
                var actualStart = inTime.ToTimeSpan();
                var diff = actualStart - scheduledStart;
                if (diff.TotalMinutes > grace)
                {
                    lateMinutes = (int)diff.TotalMinutes;
                }
            }
            result.LateMinutes = lateMinutes;

            // 9. Early departure
            int earlyMinutes = 0;
            if (shift != null && input.CheckOut.HasValue)
            {
                var scheduledEnd = shift.EndTime.ToTimeSpan();
                var actualEnd = input.CheckOut.Value.ToTimeSpan();
                var diff = scheduledEnd - actualEnd;
                if (diff.TotalMinutes > earlyThreshold)
                {
                    earlyMinutes = (int)diff.TotalMinutes;
                }
            }
            result.EarlyMinutes = earlyMinutes;

            // 10. OT calculation
            decimal otHours = 0;
            if (shift != null && input.CheckOut.HasValue && emp.OvertimeEligible)
            {
                var scheduledEnd = shift.EndTime.ToTimeSpan();
                var actualEnd = input.CheckOut.Value.ToTimeSpan();
                var otSpan = actualEnd - scheduledEnd;
                if (otSpan.TotalHours > 0.5)
                {
                    otHours = (decimal)Math.Round(otSpan.TotalHours, 2);
                }
            }
            result.OTHours = otHours;

            // 11. Half Day detection
            bool isHalfDay = false;
            if (workingMinutes > 0 && workingMinutes < 240) // < 4 hours
            {
                isHalfDay = true;
            }
            else if (lateMinutes > 120) // > 2 hours late = half day
            {
                isHalfDay = true;
            }
            result.IsHalfDay = isHalfDay;

            // 12. Status determination
            string status;
            if (isHoliday)
            {
                status = AttendanceStatuses.HolidayWork;
                result.IsHolidayWork = true;
            }
            else if (isWeeklyOff)
            {
                status = AttendanceStatuses.WeeklyOffWork;
                result.IsWeeklyOffWork = true;
            }
            else if (isHalfDay)
            {
                status = AttendanceStatuses.HalfDay;
            }
            else if (lateMinutes > 0)
            {
                status = AttendanceStatuses.Late;
            }
            else if (earlyMinutes > 0)
            {
                status = AttendanceStatuses.EarlyDeparture;
            }
            else
            {
                status = AttendanceStatuses.Present;
            }

            result.Status = status;
            result.Message = $"Attendance processed: {status}";
            result.Success = true;

            await SaveOrUpdateAttendance(input, result, isHoliday, isWeeklyOff);
            return result;
        }

        private async Task SaveOrUpdateAttendance(AttendanceInput input, AttendanceResult result, bool isHoliday, bool isWeeklyOff)
        {
            var existing = await _context.EmployeeAttendances
                .FirstOrDefaultAsync(a =>
                    a.EmployeeId == input.EmployeeId &&
                    a.AttendanceDate == input.Date);

            if (existing == null)
            {
                existing = new EmployeeAttendance
                {
                    EmployeeId = input.EmployeeId,
                    AttendanceDate = input.Date,
                    CreatedBy = input.CurrentUserId.ToString(),
                    CreatedDate = DateTime.Now
                };
                _context.EmployeeAttendances.Add(existing);
            }
            else
            {
                existing.UpdatedBy = input.CurrentUserId.ToString();
                existing.UpdatedDate = DateTime.Now;
            }

            existing.CheckInTime = input.CheckIn;
            existing.CheckOutTime = input.CheckOut;
            existing.LateMinutes = result.LateMinutes;
            existing.EarlyMinutes = result.EarlyMinutes;
            existing.OTHours = result.OTHours;
            existing.WorkingMinutes = result.WorkingMinutes;
            existing.IsHalfDay = result.IsHalfDay;
            existing.IsHolidayWork = isHoliday;
            existing.IsWeeklyOffWork = isWeeklyOff;
            existing.Status = result.Status;
            existing.OvertimeHours = result.OTHours;

            await _context.SaveChangesAsync();
        }
    }
}
