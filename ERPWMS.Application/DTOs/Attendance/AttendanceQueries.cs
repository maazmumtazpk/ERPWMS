using System;

namespace ERPWMS.Application.DTOs.Attendance
{
    public record GetAttendanceHistoryRequest(
        int EmployeeId,
        DateOnly StartDate,
        DateOnly EndDate
    );

    public record GetAttendanceReportRequest(
        int? DepartmentId,
        int? DesignationId,
        int? BranchId,
        DateOnly? StartDate,
        DateOnly? EndDate,
        string? Status
    );
}
