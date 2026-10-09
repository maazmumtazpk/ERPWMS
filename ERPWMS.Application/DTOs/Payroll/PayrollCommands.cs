using System;

namespace ERPWMS.Application.DTOs.Payroll
{
    public record GeneratePayrollRequest(
        int EmployeeId,
        DateOnly MonthYear
    );

    public record ApprovePayrollRequest(
        int PayrollId
    );

    public record MarkPayrollPaidRequest(
        int PayrollId,
        DateTime PaymentDate
    );
}