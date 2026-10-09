using MediatR;
using System;

namespace ERPWMS.Application.DTOs.Employee
{
    // ===== EMPLOYEE COMMANDS =====

    public class RegisterEmployeeCommand : IRequest<int>
    {
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public int BranchId { get; set; }
        public int? WarehouseId { get; set; }

        public int? ShiftId { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? FatherName { get; set; }
        public string? CNIC { get; set; }
        public DateOnly? DOB { get; set; }
        public string? Gender { get; set; }
        public DateOnly HireDate { get; set; }
        public decimal BaseSalary { get; set; }

        public string? EmployeeCode { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public class UpdateEmployeeCommand : IRequest<bool>
    {
        public int EmployeeId { get; set; }

        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public int BranchId { get; set; }
        public int? WarehouseId { get; set; }

        public int? ShiftId { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? FatherName { get; set; }
        public string? CNIC { get; set; }
        public DateOnly? DOB { get; set; }
        public string? Gender { get; set; }
        public DateOnly HireDate { get; set; }
        public DateOnly? TerminationDate { get; set; }
        public decimal BaseSalary { get; set; }

        public string? EmployeeCode { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public class ActivateEmployeeCommand : IRequest<bool>
    {
        public int EmployeeId { get; set; }
    }

    public class DeactivateEmployeeCommand : IRequest<bool>
    {
        public int EmployeeId { get; set; }
    }

    public class DeleteEmployeeCommand : IRequest<bool>
    {
        public int EmployeeId { get; set; }
    }

    // ===== SHIFT =====

    public class AssignShiftCommand : IRequest<bool>
    {
        public int EmployeeId { get; set; }
        public int ShiftId { get; set; }
    }

    // ===== ATTENDANCE =====

    public class MarkAttendanceCommand : IRequest<int>
    {
        public int EmployeeId { get; set; }
        public DateOnly AttendanceDate { get; set; }
        public TimeOnly CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public decimal? OvertimeHours { get; set; }
    }

    // ===== LEAVE =====

    public class SubmitLeaveCommand : IRequest<int>
    {
        public int EmployeeId { get; set; }
        public string LeaveType { get; set; } = null!;
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public string Reason { get; set; } = null!;
    }

    public class ApproveLeaveCommand : IRequest<bool>
    {
        public int LeaveId { get; set; }
    }

    public class RejectLeaveCommand : IRequest<bool>
    {
        public int LeaveId { get; set; }
        public string RejectionReason { get; set; } = null!;
    }

    // ===== PAYROLL =====

    public class GeneratePayrollCommand : IRequest<int>
    {
        public int EmployeeId { get; set; }
        public DateOnly MonthYear { get; set; }
    }

    public class ApprovePayrollCommand : IRequest<bool>
    {
        public int PayrollId { get; set; }
    }

    public class RecordPaymentCommand : IRequest<bool>
    {
        public int PayrollId { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}