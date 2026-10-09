using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int DepartmentId { get; set; }

    public int DesignationId { get; set; }

    public int BranchId { get; set; }

    public int? WarehouseId { get; set; }

    public int? UserId { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? FatherName { get; set; }

    public string? Cnic { get; set; }


    public string? Gender { get; set; }

    public DateOnly HireDate { get; set; }

    public DateOnly? TerminationDate { get; set; }

    public decimal BaseSalary { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? ShiftId { get; set; }

    public string? DeletedByName { get; set; }

    public string? CreatedByName { get; set; }

    public string? UpdatedByName { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual Department Department { get; set; } = null!;

    public virtual Designation Designation { get; set; } = null!;

    public virtual ICollection<EmployeeBankAccount> EmployeeBankAccounts { get; set; } = new List<EmployeeBankAccount>();

    public virtual ICollection<EmployeeLeaf> EmployeeLeaves { get; set; } = new List<EmployeeLeaf>();

    public virtual ICollection<EmployeeLeave> EmployeeLeavesNavigation { get; set; } = new List<EmployeeLeave>();

    public virtual ICollection<EmployeePayrollDetail> EmployeePayrollDetails { get; set; } = new List<EmployeePayrollDetail>();

    public virtual ICollection<EmployeePayroll> EmployeePayrolls { get; set; } = new List<EmployeePayroll>();

    public virtual ICollection<EmployeeSalary> EmployeeSalaries { get; set; } = new List<EmployeeSalary>();

    public virtual ICollection<EmployeeSalary1> EmployeeSalary1s { get; set; } = new List<EmployeeSalary1>();

    public virtual Shift? Shift { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
    public DateOnly? DOB { get; set; }

    // =========================================================
    // STEP 2 ADDITIONS — Payroll-related fields
    // =========================================================

    public string EmploymentType { get; set; } = "Permanent";

    public string WeeklyOffDay { get; set; } = "Sunday";

    public bool OvertimeEligible { get; set; } = true;

    public int? SalaryStructureId { get; set; }

    public int? LeavePolicyId { get; set; }
}

