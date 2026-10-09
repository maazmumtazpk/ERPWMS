using System;

namespace ERPWMS.Application.DTOs.Attendance
{
    public record CheckInAttendanceRequest(
        int EmployeeId,
        DateOnly AttendanceDate,
        TimeOnly CheckInTime,
        int? ShiftId
    );

    public record CheckOutAttendanceRequest(
        int EmployeeId,
        DateOnly AttendanceDate,
        TimeOnly CheckOutTime
    );

    public record MarkAbsentRequest(
        int EmployeeId,
        DateOnly AttendanceDate
    );

    public record MarkLateRequest(
        int EmployeeId,
        DateOnly AttendanceDate,
        TimeOnly CheckInTime,
        int? ShiftId
    );
}
