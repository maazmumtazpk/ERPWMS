using System;

namespace ERPWMS.Application.DTOs.Payroll
{
    public record GetPayrollDetailRequest(
        int PayrollId
    );

    public record GetEmployeePayrollHistoryRequest(
        int EmployeeId
    );

    public record PayrollReportRequest(
        int? DepartmentId,
        int? EmployeeId,
        DateOnly? PeriodStart,
        DateOnly? PeriodEnd,
        string? Status
    );
}
