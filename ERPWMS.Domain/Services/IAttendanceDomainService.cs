using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Domain.Services.DTOs;

namespace ERPWMS.Domain.Services
{
    public interface IAttendanceDomainService
    {
        Task<AttendanceDetails> CheckInAsync(
            int employeeId,
            DateOnly attendanceDate,
            TimeOnly checkInTime,
            int? shiftId,
            int userId,
            CancellationToken cancellationToken = default);

        Task<AttendanceDetails> CheckOutAsync(
            int employeeId,
            DateOnly attendanceDate,
            TimeOnly checkOutTime,
            int userId,
            CancellationToken cancellationToken = default);

        Task<AttendanceDetails> MarkAbsentAsync(
            int employeeId,
            DateOnly attendanceDate,
            int userId,
            CancellationToken cancellationToken = default);

        Task<AttendanceDetails> MarkLateAsync(
            int employeeId,
            DateOnly attendanceDate,
            TimeOnly checkInTime,
            int? shiftId,
            int userId,
            CancellationToken cancellationToken = default);

        Task<List<AttendanceDetails>> GetHistoryAsync(
            int employeeId,
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken = default);

        Task<List<AttendanceDetails>> GetReportAsync(
            int? departmentId,
            int? designationId,
            int? branchId,
            DateOnly? startDate,
            DateOnly? endDate,
            string? status,
            CancellationToken cancellationToken = default);
    }
}