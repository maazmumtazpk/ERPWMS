using System;
using System.Collections.Generic;

namespace ERPWMS.Web.ViewModels.Employee
{
    public class EmployeeDashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }
        public int NewThisMonth { get; set; }
        public int OnLeaveToday { get; set; }
        public int PresentToday { get; set; }
        public decimal AverageSalary { get; set; }
        public int DepartmentsCount { get; set; }
        public int DesignationsCount { get; set; }
        public int BranchesCount { get; set; }
        public dynamic? Summary { get; set; }
    }

    public class EmployeeDetailsViewModel
    {
        public int EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? FatherName { get; set; }
        public string? Cnic { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public DateTime? JoiningDate { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? WarehouseName { get; set; }
        public decimal? BasicSalary { get; set; }
        public decimal? BaseSalary { get; set; }
        public decimal? CurrentNetSalary { get; set; }
        public bool? IsActive { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public dynamic? Employee { get; set; }
        public dynamic? PrimaryBankAccount { get; set; }
        public dynamic? Salaries { get; set; }
        public dynamic? Payrolls { get; set; }
        public dynamic? Leaves { get; set; }
        public dynamic? Attendances { get; set; }
        public dynamic? BankAccounts { get; set; }
    }
}