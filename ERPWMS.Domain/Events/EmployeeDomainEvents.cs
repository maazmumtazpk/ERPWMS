using System;
using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Events
{
    public abstract class EmployeeDomainEvent : INotification
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public int EmployeeId { get; protected set; }
        public int? UserId { get; protected set; }
    }

    // ===== EMPLOYEE EVENTS =====
    public class EmployeeRegisteredEvent : EmployeeDomainEvent
    {
        public string EmployeeName { get; }
        public int DepartmentId { get; }
        public int DesignationId { get; }
        public int BranchId { get; }
        public int? ShiftId { get; }
        public decimal BaseSalary { get; }

        public EmployeeRegisteredEvent(int id, string name, int departmentId, int designationId, int branchId, int? shiftId, decimal baseSalary, int userId)
        {
            EmployeeId = id;
            EmployeeName = name;
            DepartmentId = departmentId;
            DesignationId = designationId;
            BranchId = branchId;
            ShiftId = shiftId;
            BaseSalary = baseSalary;
            UserId = userId;
        }
    }

    public class EmployeeActivatedEvent : EmployeeDomainEvent
    {
        public EmployeeActivatedEvent(int id, int userId)
        {
            EmployeeId = id;
            UserId = userId;
        }
    }

    public class EmployeeDeactivatedEvent : EmployeeDomainEvent
    {
        public EmployeeDeactivatedEvent(int id, int userId)
        {
            EmployeeId = id;
            UserId = userId;
        }
    }

    public class EmployeeModifiedEvent : EmployeeDomainEvent
    {
        public string OldName { get; }
        public string NewName { get; }
        public EmployeeModifiedEvent(int id, string oldName, string newName, int userId)
        {
            EmployeeId = id;
            OldName = oldName;
            NewName = newName;
            UserId = userId;
        }
    }

    public class EmployeeDeletedEvent : EmployeeDomainEvent
    {
        public EmployeeDeletedEvent(int id, int userId)
        {
            EmployeeId = id;
            UserId = userId;
        }
    }

    public class EmployeeShiftAssignedEvent : EmployeeDomainEvent
    {
        public int ShiftId { get; }
        public string ShiftName { get; }
        public EmployeeShiftAssignedEvent(int employeeId, int shiftId, string shiftName, int userId)
        {
            EmployeeId = employeeId;
            ShiftId = shiftId;
            ShiftName = shiftName;
            UserId = userId;
        }
    }

    // ===== ATTENDANCE EVENTS =====
    public class AttendanceMarkedEvent : EmployeeDomainEvent
    {
        public DateOnly Date { get; }
        public TimeOnly CheckIn { get; }
        public TimeOnly? CheckOut { get; }
        public string Status { get; }

        public AttendanceMarkedEvent(int employeeId, DateOnly date, TimeOnly checkIn, TimeOnly? checkOut, string status, int userId)
        {
            EmployeeId = employeeId;
            Date = date;
            CheckIn = checkIn;
            CheckOut = checkOut;
            Status = status;
            UserId = userId;
        }
    }

    public class AttendanceUpdatedEvent : EmployeeDomainEvent
    {
        public DateOnly Date { get; }
        public string OldStatus { get; }
        public string NewStatus { get; }

        public AttendanceUpdatedEvent(int employeeId, DateOnly date, string oldStatus, string newStatus, int userId)
        {
            EmployeeId = employeeId;
            Date = date;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            UserId = userId;
        }
    }

    // ===== LEAVE EVENTS =====
    public class LeaveSubmittedEvent : EmployeeDomainEvent
    {
        public int LeaveId { get; }
        public string LeaveType { get; }
        public DateOnly FromDate { get; }
        public DateOnly ToDate { get; }
        public int TotalDays { get; }

        public LeaveSubmittedEvent(int employeeId, int leaveId, string leaveType, DateOnly fromDate, DateOnly toDate, int totalDays, int userId)
        {
            EmployeeId = employeeId;
            LeaveId = leaveId;
            LeaveType = leaveType;
            FromDate = fromDate;
            ToDate = toDate;
            TotalDays = totalDays;
            UserId = userId;
        }
    }

    public class LeaveApprovedEvent : EmployeeDomainEvent
    {
        public int LeaveId { get; }
        public LeaveApprovedEvent(int employeeId, int leaveId, int userId)
        {
            EmployeeId = employeeId;
            LeaveId = leaveId;
            UserId = userId;
        }
    }

    public class LeaveRejectedEvent : EmployeeDomainEvent
    {
        public int LeaveId { get; }
        public string Reason { get; }
        public LeaveRejectedEvent(int employeeId, int leaveId, string reason, int userId)
        {
            EmployeeId = employeeId;
            LeaveId = leaveId;
            Reason = reason;
            UserId = userId;
        }
    }

    // ===== PAYROLL EVENTS =====
    public class PayrollGeneratedEvent : EmployeeDomainEvent
    {
        public int PayrollId { get; }
        public DateOnly MonthYear { get; }
        public decimal GrossPay { get; }
        public decimal NetPay { get; }

        public PayrollGeneratedEvent(int employeeId, int payrollId, DateOnly monthYear, decimal grossPay, decimal netPay, int userId)
        {
            EmployeeId = employeeId;
            PayrollId = payrollId;
            MonthYear = monthYear;
            GrossPay = grossPay;
            NetPay = netPay;
            UserId = userId;
        }
    }

    public class PayrollApprovedEvent : EmployeeDomainEvent
    {
        public int PayrollId { get; }
        public PayrollApprovedEvent(int employeeId, int payrollId, int userId)
        {
            EmployeeId = employeeId;
            PayrollId = payrollId;
            UserId = userId;
        }
    }

    public class PayrollPaidEvent : EmployeeDomainEvent
    {
        public int PayrollId { get; }
        public DateTime PaymentDate { get; }
        public PayrollPaidEvent(int employeeId, int payrollId, DateTime paymentDate, int userId)
        {
            EmployeeId = employeeId;
            PayrollId = payrollId;
            PaymentDate = paymentDate;
            UserId = userId;
        }
    }
}
