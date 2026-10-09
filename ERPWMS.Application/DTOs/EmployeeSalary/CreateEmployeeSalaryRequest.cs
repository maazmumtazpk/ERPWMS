using System;

namespace ERPWMS.Application.DTOs.EmployeeSalary
{
    public class CreateEmployeeSalaryRequest
    {
        public int EmployeeId { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal? HouseRent { get; set; }
        public decimal? UtilityAllowance { get; set; }
        public decimal? MedicalAllowance { get; set; }
        public decimal? OtherAllowances { get; set; }
        public decimal? IncomeTax { get; set; }
        public decimal? ProfessionalTax { get; set; }
        public decimal? OtherDeductions { get; set; }
    }
}
