using System;
using System.Collections.Generic;
using ERPWMS.Application.DTOs.EmployeeBankAccount;
using ERPWMS.Application.DTOs.EmployeeSalary;

namespace ERPWMS.Application.DTOs.Employee
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public int BranchId { get; set; }
        public int? WarehouseId { get; set; }
        public int? UserId { get; set; }
        public int? ShiftId { get; set; }
        public string EmploymentType { get; set; } = "Permanent";
        public string WeeklyOffDay { get; set; } = "Sunday";
        public bool OvertimeEligible { get; set; } = true;
        public int? SalaryStructureId { get; set; }
        public int? LeavePolicyId { get; set; }

        public string EmployeeCode { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? FatherName { get; set; }
        public string? Cnic { get; set; }
        public DateOnly? DOB { get; set; }
        public string? Gender { get; set; }
        public DateOnly HireDate { get; set; }
        public DateOnly? TerminationDate { get; set; }
        public decimal BaseSalary { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String?UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }
        public string? BranchName { get; set; }
        public string? WarehouseName { get; set; }
        public string? ShiftName { get; set; }
        public string? FullName => $"{FirstName} {LastName}";

        public List<EmployeeBankAccountDto> BankAccounts { get; set; } = new();
        public List<EmployeeSalaryDto> Salaries { get; set; } = new();
    }
}

