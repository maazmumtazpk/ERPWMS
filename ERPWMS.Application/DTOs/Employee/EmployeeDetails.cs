using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Employee
{
    /// <summary>
    /// Complete Employee Details DTO for ViewEmployee page
    /// </summary>
    public class EmployeeDetails
    {
        // =========================================================
        // PERSONAL INFORMATION
        // =========================================================

        public int EmployeeId { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}".Trim();

        public string? FatherName { get; set; }

        // Support both naming styles
        public string? Cnic { get; set; }

        public string? CNIC
        {
            get => Cnic;
            set => Cnic = value;
        }

        public DateOnly? Dob { get; set; }

        public DateOnly? DOB
        {
            get => Dob;
            set => Dob = value;
        }

        public string? Gender { get; set; }

        // =========================================================
        // CONTACT INFORMATION
        // =========================================================

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Mobile { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        // =========================================================
        // EMPLOYMENT INFORMATION
        // =========================================================

        public int DepartmentId { get; set; }

        public int DesignationId { get; set; }

        public int BranchId { get; set; }

        public int? WarehouseId { get; set; }

        public int? ShiftId { get; set; }

        public DateOnly HireDate { get; set; }

        public DateOnly? TerminationDate { get; set; }

        // =========================================================
        // EMPLOYMENT DISPLAY NAMES
        // =========================================================

        public string DepartmentName { get; set; } = string.Empty;

        public string DesignationName { get; set; } = string.Empty;

        public string BranchName { get; set; } = string.Empty;

        public string WarehouseName { get; set; } = string.Empty;

        public string ShiftName { get; set; } = string.Empty;

        // =========================================================
        // STATUS
        // =========================================================

        public bool? IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        // =========================================================
        // FINANCIAL INFORMATION
        // =========================================================

        public decimal BaseSalary { get; set; }

        public decimal? CurrentNetSalary { get; set; }

        // =========================================================
        // AUDIT INFORMATION
        // =========================================================

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public String?UpdatedBy{ get; set; }

        public DateTime? UpdatedDate { get; set; }

        // =========================================================
        // ATTENDANCE
        // =========================================================

        public decimal AttendancePercentage { get; set; }

        public List<EmployeeAttendanceDto> Attendances { get; set; }
            = new List<EmployeeAttendanceDto>();

        // =========================================================
        // PAYROLL HISTORY
        // =========================================================

        public List<EmployeePayrollDto> Payrolls { get; set; }
            = new List<EmployeePayrollDto>();

        public EmployeePayrollDto? LatestPayroll { get; set; }

        // =========================================================
        // LEAVE HISTORY
        // =========================================================

        public List<EmployeeLeaveDto> Leaves { get; set; }
            = new List<EmployeeLeaveDto>();

        // =========================================================
        // SALARY HISTORY
        // =========================================================

        public List<EmployeeSalaryDto> Salaries { get; set; }
            = new List<EmployeeSalaryDto>();

        // =========================================================
        // BANK ACCOUNTS
        // =========================================================

        public List<EmployeeBankAccountInfo> BankAccounts { get; set; }
            = new List<EmployeeBankAccountInfo>();

        public EmployeeBankAccountInfo? PrimaryBankAccount { get; set; }
    }

    // =========================================================
    // NESTED DTO: Bank Account Info
    // =========================================================

    public class EmployeeBankAccountInfo
    {
        public int BankAccountId { get; set; }

        public string BankName { get; set; } = string.Empty;

        public string AccountNumber { get; set; } = string.Empty;

        public string? Iban { get; set; }

        public string? AccountType { get; set; }

        public bool? IsPrimary { get; set; }

        public bool? IsActive { get; set; }
    }
}