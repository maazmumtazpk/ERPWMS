using System;

namespace ERPWMS.Application.DTOs.Attendance
{
    public record AttendanceDetails(
        int AttendanceId,
        int EmployeeId,
        DateOnly AttendanceDate,
        TimeOnly? CheckInTime,
        TimeOnly? CheckOutTime,
        string? Status,
        decimal? WorkingHours,
        decimal? OvertimeHours,
        int? ShiftId
    );
}