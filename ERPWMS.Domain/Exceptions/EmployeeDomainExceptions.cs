using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Exceptions
{
    public abstract class EmployeeDomainException : Exception
    {
        public string? ErrorCode { get; }
        protected EmployeeDomainException(string message) : base(message) { }
        protected EmployeeDomainException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }

    public class EmployeeNotFoundException : EmployeeDomainException
    {
        public int EmployeeId { get; }
        public EmployeeNotFoundException(int id) : base($"Employee with ID {id} not found.", "EMP-001")
        {
            EmployeeId = id;
        }
    }

    public class EmployeeCodeAlreadyExistsException : EmployeeDomainException
    {
        public string Code { get; }
        public EmployeeCodeAlreadyExistsException(string code) : base($"Employee code '{code}' already exists.", "EMP-002")
        {
            Code = code;
        }
    }

    public class EmployeeEmailAlreadyExistsException : EmployeeDomainException
    {
        public string Email { get; }
        public EmployeeEmailAlreadyExistsException(string email) : base($"Employee email '{email}' already exists.", "EMP-003")
        {
            Email = email;
        }
    }

    public class EmployeeAlreadyActiveException : EmployeeDomainException
    {
        public EmployeeAlreadyActiveException(int id) : base($"Employee {id} is already active.", "EMP-004") { }
    }

    public class EmployeeAlreadyInactiveException : EmployeeDomainException
    {
        public EmployeeAlreadyInactiveException(int id) : base($"Employee {id} is already inactive.", "EMP-005") { }
    }

    public class EmployeeAttendanceAlreadyMarkedException : EmployeeDomainException
    {
        public int EmployeeId { get; }
        public DateOnly Date { get; }
        public EmployeeAttendanceAlreadyMarkedException(int id, DateOnly date)
            : base($"Attendance already marked for employee {id} on {date}.", "EMP-006")
        {
            EmployeeId = id;
            Date = date;
        }
    }

    public class EmployeeSalaryStructureNotFoundException : EmployeeDomainException
    {
        public int EmployeeId { get; }
        public EmployeeSalaryStructureNotFoundException(int id) : base($"Salary structure not found for employee {id}.", "EMP-007")
        {
            EmployeeId = id;
        }
    }

    public class UnauthorizedEmployeeAccessException : EmployeeDomainException
    {
        public UnauthorizedEmployeeAccessException(int userId, int employeeId)
            : base($"User {userId} is not authorized to access employee {employeeId}.", "EMP-401") { }
    }

    public class EmployeeLeaveNotFoundException : EmployeeDomainException
    {
        public int LeaveId { get; }
        public EmployeeLeaveNotFoundException(int id) : base($"Leave request {id} not found.", "EMP-008")
        {
            LeaveId = id;
        }
    }

    public class EmployeeLeaveAlreadyApprovedException : EmployeeDomainException
    {
        public int LeaveId { get; }
        public EmployeeLeaveAlreadyApprovedException(int id) : base($"Leave request {id} is already approved.", "EMP-009")
        {
            LeaveId = id;
        }
    }

    public class EmployeeLeaveAlreadyRejectedException : EmployeeDomainException
    {
        public int LeaveId { get; }
        public EmployeeLeaveAlreadyRejectedException(int id) : base($"Leave request {id} is already rejected.", "EMP-010")
        {
            LeaveId = id;
        }
    }

    public class EmployeePayrollAlreadyExistsException : EmployeeDomainException
    {
        public int EmployeeId { get; }
        public DateOnly MonthYear { get; }
        public EmployeePayrollAlreadyExistsException(int id, DateOnly month)
            : base($"Payroll already exists for employee {id} for {month:MMM yyyy}.", "EMP-011")
        {
            EmployeeId = id;
            MonthYear = month;
        }
    }

    public class EmployeePayrollNotFoundException : EmployeeDomainException
    {
        public int PayrollId { get; }
        public EmployeePayrollNotFoundException(int id) : base($"Payroll {id} not found.", "EMP-012")
        {
            PayrollId = id;
        }
    }
}
