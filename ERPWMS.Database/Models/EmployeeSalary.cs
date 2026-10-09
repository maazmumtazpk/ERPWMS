using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class EmployeeSalary
{
    public int SalaryId { get; set; }

    public int EmployeeId { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public decimal BasicSalary { get; set; }

    public decimal? HouseRent { get; set; }

    public decimal? UtilityAllowance { get; set; }

    public decimal? MedicalAllowance { get; set; }

    public decimal? OtherAllowances { get; set; }

    public decimal? TotalAllowances { get; set; }

    public decimal? IncomeTax { get; set; }

    public decimal? ProfessionalTax { get; set; }

    public decimal? OtherDeductions { get; set; }

    public decimal? TotalDeductions { get; set; }

    public decimal? NetSalary { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
