using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Exceptions.HRMaster
{
    // ============================================================
    // BASE EMPLOYEE DOMAIN EXCEPTION
    // ============================================================

    public class EmployeeDomainException : HRMasterDomainException
    {
        public EmployeeDomainException(
            string message,
            string errorCode)
            : base(message, errorCode)
        {
        }

        public EmployeeDomainException(
            string message,
            string errorCode,
            Exception innerException)
            : base(message, errorCode, innerException)
        {
        }
    }


    // ============================================================
    // EMPLOYEE VALIDATION EXCEPTION
    // ============================================================

    public class EmployeeValidationException : EmployeeDomainException
    {
        public string? PropertyName { get; }

        public EmployeeValidationException(
            string message)
            : base(message, "EMP-000")
        {
        }

        public EmployeeValidationException(
            string message,
            string? propertyName)
            : base(message, "EMP-000")
        {
            PropertyName = propertyName;
        }

        public EmployeeValidationException(
            string message,
            string? propertyName,
            Exception innerException)
            : base(message, "EMP-000", innerException)
        {
            PropertyName = propertyName;
        }
    }


    // ============================================================
    // EMPLOYEE NOT FOUND
    // ============================================================

    public class EmployeeNotFoundException : EmployeeDomainException
    {
        public int? EmployeeId { get; }
        public string? EmployeeCode { get; }

        public EmployeeNotFoundException(int employeeId)
            : base(
                $"Employee with ID {employeeId} was not found",
                "EMP-001")
        {
            EmployeeId = employeeId;
        }

        public EmployeeNotFoundException(string employeeCode)
            : base(
                $"Employee with Code {employeeCode} was not found",
                "EMP-001")
        {
            EmployeeCode = employeeCode;
        }

        public EmployeeNotFoundException(
            string message,
            string errorCode)
            : base(message, errorCode)
        {
        }
    }


    // ============================================================
    // EMPLOYEE CODE ALREADY EXISTS
    // ============================================================

    public class EmployeeCodeAlreadyExistsException
        : EmployeeDomainException
    {
        public string EmployeeCode { get; }

        public EmployeeCodeAlreadyExistsException(
            string employeeCode)
            : base(
                $"Employee with code '{employeeCode}' already exists",
                "EMP-002")
        {
            EmployeeCode = employeeCode;
        }

        public EmployeeCodeAlreadyExistsException(
            string employeeCode,
            string message)
            : base(message, "EMP-002")
        {
            EmployeeCode = employeeCode;
        }
    }


    // ============================================================
    // EMPLOYEE INACTIVE
    // ============================================================

    public class EmployeeInactiveException
        : EmployeeDomainException
    {
        public int EmployeeId { get; }

        public EmployeeInactiveException(
            int employeeId)
            : base(
                $"Employee with ID {employeeId} is inactive",
                "EMP-003")
        {
            EmployeeId = employeeId;
        }

        public EmployeeInactiveException(
            int employeeId,
            string message)
            : base(message, "EMP-003")
        {
            EmployeeId = employeeId;
        }
    }


    // ============================================================
    // EMPLOYEE DELETED
    // ============================================================

    public class EmployeeDeletedException
        : EmployeeDomainException
    {
        public int EmployeeId { get; }

        public EmployeeDeletedException(
            int employeeId)
            : base(
                $"Employee with ID {employeeId} has been deleted and cannot be modified",
                "EMP-004")
        {
            EmployeeId = employeeId;
        }

        public EmployeeDeletedException(
            int employeeId,
            string message)
            : base(message, "EMP-004")
        {
            EmployeeId = employeeId;
        }
    }


    // ============================================================
    // EMPLOYEE DEPARTMENT MISMATCH
    // ============================================================

    public class EmployeeDepartmentMismatchException
        : EmployeeDomainException
    {
        public int EmployeeId { get; }
        public int DepartmentId { get; }

        public EmployeeDepartmentMismatchException(
            int employeeId,
            int departmentId)
            : base(
                $"Employee {employeeId} is not assigned to department {departmentId}",
                "EMP-005")
        {
            EmployeeId = employeeId;
            DepartmentId = departmentId;
        }
    }
}