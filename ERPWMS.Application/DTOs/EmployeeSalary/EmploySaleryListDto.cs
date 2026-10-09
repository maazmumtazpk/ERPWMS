using System;

namespace ERPWMS.Application.DTOs.EmployeeSalary
{
    /// <summary>
    /// DTO for listing employee salaries (lightweight version)
    /// </summary>
    public class EmployeeSalaryListDto
    {
        // =========================================================
        // IDENTIFICATION
        // =========================================================

        public int SalaryId { get; set; }
        public int EmployeeId { get; set; }

        // =========================================================
        // SALARY INFORMATION
        // =========================================================

        public DateOnly EffectiveDate { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal? TotalAllowances { get; set; }
        public decimal? TotalDeductions { get; set; }
        public decimal? NetSalary { get; set; }

        // =========================================================
        // STATUS
        // =========================================================

        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        // =========================================================
        // NAVIGATION PROPERTIES (for display)
        // =========================================================

        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }
    }
}