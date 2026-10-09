using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Exceptions.HRMaster
{
    // ============================================================
    // BASE EMPLOYEE SALARY DOMAIN EXCEPTION
    // ============================================================

    public class EmployeeSalaryDomainException : HRMasterDomainException
    {
        public EmployeeSalaryDomainException(
            string message,
            string errorCode)
            : base(message, errorCode)
        {
        }

        public EmployeeSalaryDomainException(
            string message,
            string errorCode,
            Exception innerException)
            : base(message, errorCode, innerException)
        {
        }
    }


    // ============================================================
    // EMPLOYEE SALARY NOT FOUND
    // ============================================================

    public class EmployeeSalaryNotFoundException : EmployeeSalaryDomainException
    {
        public int SalaryId { get; }
        public int? EmployeeId { get; }

        public EmployeeSalaryNotFoundException(int salaryId)
            : base(
                $"Salary record with ID {salaryId} was not found",
                "ESL-001")
        {
            SalaryId = salaryId;
        }

        public EmployeeSalaryNotFoundException(
            int salaryId,
            int employeeId)
            : base(
                $"Salary record with ID {salaryId} was not found for employee {employeeId}",
                "ESL-001")
        {
            SalaryId = salaryId;
            EmployeeId = employeeId;
        }
    }


    // ============================================================
    // EMPLOYEE SALARY ALREADY EXISTS
    // ============================================================

    public class EmployeeSalaryAlreadyExistsException : EmployeeSalaryDomainException
    {
        public int EmployeeId { get; }
        public DateOnly EffectiveDate { get; }

        public EmployeeSalaryAlreadyExistsException(
            int employeeId,
            DateOnly effectiveDate)
            : base(
                $"Salary record for employee {employeeId} with effective date {effectiveDate} already exists",
                "ESL-002")
        {
            EmployeeId = employeeId;
            EffectiveDate = effectiveDate;
        }
    }


    // ============================================================
    // EMPLOYEE SALARY INACTIVE
    // ============================================================

    public class EmployeeSalaryInactiveException : EmployeeSalaryDomainException
    {
        public int SalaryId { get; }

        public EmployeeSalaryInactiveException(int salaryId)
            : base(
                $"Salary record with ID {salaryId} is inactive",
                "ESL-003")
        {
            SalaryId = salaryId;
        }
    }


    // ============================================================
    // EMPLOYEE SALARY FUTURE DATE
    // ============================================================

    public class EmployeeSalaryFutureDateException : EmployeeSalaryDomainException
    {
        public DateOnly EffectiveDate { get; }

        public EmployeeSalaryFutureDateException(
            DateOnly effectiveDate)
            : base(
                $"Effective date {effectiveDate} cannot be in the future",
                "ESL-004")
        {
            EffectiveDate = effectiveDate;
        }
    }
}