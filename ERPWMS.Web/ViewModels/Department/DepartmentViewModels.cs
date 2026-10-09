using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Web.ViewModels.Department
{
    public class RegisterDepartmentViewModel
    {
        public int? DepartmentId { get; set; }
        [Required]
        public string DepartmentName { get; set; } = string.Empty;
        public string? DepartmentCode { get; set; }
        [Required]
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public bool? IsActive { get; set; } = true;

        public string? Title { get; set; }
        public bool IsEdit { get; set; }
        public string? SubmitButtonText { get; set; }
        public string? CancelUrl { get; set; }
    }

    public class DepartmentSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public int? BranchId { get; set; }
        public int? CompanyId { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public List<DepartmentItem> Departments { get; set; } = new();
        public string? ResultsSummary { get; set; }
        public bool HasResults => Departments != null && Departments.Count > 0;
        public int TotalPages { get; set; }
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }

    public class DepartmentItem
    {
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? DepartmentCode { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? CompanyName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int DesignationCount { get; set; }
        public int EmployeeCount { get; set; }
    }
}
