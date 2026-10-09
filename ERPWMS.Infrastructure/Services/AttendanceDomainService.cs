using ERPWMS.Database.Models;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Domain.Services.DTOs;   // ✅ SAHI namespace (Domain wala)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    // ✅ C# 12 Primary Constructor
    public class AttendanceDomainService(IEmployeeRepository employeeRepository) : IAttendanceDomainService
    {
        // =========================================================
        // CHECK IN
        // =========================================================
        public async Task<AttendanceDetails> CheckInAsync(
            int employeeId,
            DateOnly attendanceDate,
            TimeOnly checkInTime,
            int? shiftId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            var employee = await employeeRepository.GetByIdAsync(employeeId);

            if (employee is null)
                throw new InvalidOperationException("Employee not found.");

            var existing = await employeeRepository
                .GetAttendanceByDateAsync(employeeId, attendanceDate);

            if (existing is not null)
                throw new InvalidOperationException("Attendance already exists for this date.");

            var attendance = new EmployeeAttendance
            {
                EmployeeId = employeeId,
                AttendanceDate = attendanceDate,
                CheckInTime = checkInTime,
                Status = "Present",
                ShiftId = shiftId,
                CreatedBy = userId.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            await employeeRepository.AddAttendanceAsync(attendance);
            await employeeRepository.SaveAsync();

            return Map(attendance);
        }

        // =========================================================
        // CHECK OUT
        // =========================================================
        public async Task<AttendanceDetails> CheckOutAsync(
            int employeeId,
            DateOnly attendanceDate,
            TimeOnly checkOutTime,
            int userId,
            CancellationToken cancellationToken = default)
        {
            var attendance = await employeeRepository
                .GetAttendanceByDateAsync(employeeId, attendanceDate);

            if (attendance is null)
                throw new InvalidOperationException("Attendance record not found.");

            if (!attendance.CheckInTime.HasValue)
                throw new InvalidOperationException("Employee has not checked in.");

            if (checkOutTime < attendance.CheckInTime.Value)
                throw new InvalidOperationException("Check-out time cannot be earlier than check-in time.");

            attendance.CheckOutTime = checkOutTime;

            var workingHours = (decimal)(checkOutTime - attendance.CheckInTime.Value).TotalHours;

            attendance.UpdatedBy = userId.ToString();
            attendance.UpdatedDate = DateTime.UtcNow;

            await employeeRepository.UpdateAttendanceAsync(attendance);
            await employeeRepository.SaveAsync();

            return Map(attendance, workingHours);
        }

        // =========================================================
        // MARK ABSENT
        // =========================================================
        public async Task<AttendanceDetails> MarkAbsentAsync(
            int employeeId,
            DateOnly attendanceDate,
            int userId,
            CancellationToken cancellationToken = default)
        {
            var employee = await employeeRepository.GetByIdAsync(employeeId);

            if (employee is null)
                throw new InvalidOperationException("Employee not found.");

            var existing = await employeeRepository
                .GetAttendanceByDateAsync(employeeId, attendanceDate);

            if (existing is not null)
                throw new InvalidOperationException("Attendance already exists for this date.");

            var attendance = new EmployeeAttendance
            {
                EmployeeId = employeeId,
                AttendanceDate = attendanceDate,
                Status = "Absent",
                CreatedBy = userId.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            await employeeRepository.AddAttendanceAsync(attendance);
            await employeeRepository.SaveAsync();

            return Map(attendance);
        }

        // =========================================================
        // MARK LATE
        // =========================================================
        public async Task<AttendanceDetails> MarkLateAsync(
            int employeeId,
            DateOnly attendanceDate,
            TimeOnly checkInTime,
            int? shiftId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            var employee = await employeeRepository.GetByIdAsync(employeeId);

            if (employee is null)
                throw new InvalidOperationException("Employee not found.");

            var existing = await employeeRepository
                .GetAttendanceByDateAsync(employeeId, attendanceDate);

            if (existing is not null)
                throw new InvalidOperationException("Attendance already exists for this date.");

            var attendance = new EmployeeAttendance
            {
                EmployeeId = employeeId,
                AttendanceDate = attendanceDate,
                CheckInTime = checkInTime,
                Status = "Late",
                ShiftId = shiftId,
                CreatedBy = userId.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            await employeeRepository.AddAttendanceAsync(attendance);
            await employeeRepository.SaveAsync();

            return Map(attendance);
        }

        // =========================================================
        // GET HISTORY
        // =========================================================
        public async Task<List<AttendanceDetails>> GetHistoryAsync(
            int employeeId,
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken = default)
        {
            var records = await employeeRepository
                .GetAttendanceByDateRangeAsync(employeeId, startDate, endDate);

            return records.Select(x => Map(x)).ToList();
        }

        // =========================================================
        // GET REPORT
        // =========================================================
        public async Task<List<AttendanceDetails>> GetReportAsync(
            int? departmentId,
            int? designationId,
            int? branchId,
            DateOnly? startDate,
            DateOnly? endDate,
            string? status,
            CancellationToken cancellationToken = default)
        {
            var records = await employeeRepository
                .GetAttendanceReportAsync(
                    departmentId, designationId, branchId,
                    startDate, endDate, status);

            return records.Select(x => Map(x)).ToList();
        }

        // =========================================================
        // MAP  (✅ FINAL FIX: Object Initializer)
        // =========================================================
        private static AttendanceDetails Map(
            EmployeeAttendance attendance,
            decimal? workingHours = null)
        {
            decimal? calculatedHours = workingHours;

            if (!calculatedHours.HasValue &&
                attendance.CheckInTime.HasValue &&
                attendance.CheckOutTime.HasValue)
            {
                calculatedHours = (decimal)(
                    attendance.CheckOutTime.Value -
                    attendance.CheckInTime.Value).TotalHours;
            }

            // ✅ Object Initializer use kiya (kyunke Domain wale DTO mein constructor nahi hai)
            return new AttendanceDetails
            {
                AttendanceId = attendance.AttendanceId,
                EmployeeId = attendance.EmployeeId,
                AttendanceDate = attendance.AttendanceDate,
                CheckInTime = attendance.CheckInTime,
                CheckOutTime = attendance.CheckOutTime,
                Status = attendance.Status,
                WorkingHours = calculatedHours,
                OvertimeHours = attendance.OvertimeHours,
                ShiftId = attendance.ShiftId
            };
        }
    }
}