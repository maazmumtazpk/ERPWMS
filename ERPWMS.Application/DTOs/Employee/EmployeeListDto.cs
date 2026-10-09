using System;

namespace ERPWMS.Application.DTOs.Employee
{
    public class EmployeeListDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? FullName => $"{FirstName} {LastName}";
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }
        public string? BranchName { get; set; }
        public decimal BaseSalary { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateOnly HireDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int BankAccountCount { get; set; }
    }
}
