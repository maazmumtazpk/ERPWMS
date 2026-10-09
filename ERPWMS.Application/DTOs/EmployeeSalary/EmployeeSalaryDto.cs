using System;

namespace ERPWMS.Application.DTOs.EmployeeSalary
{
    public class EmployeeSalaryDto
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
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String?UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
    }
}


