using MediatR;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Department
{
    /// <summary>
    /// Query to get department profile
    /// </summary>
    public class GetDepartmentProfileQuery : IRequest<DepartmentProfileResponse>
    {
        public int DepartmentId { get; set; }
    }

    /// <summary>
    /// Query to get all active departments for a branch
    /// </summary>
    public class GetActiveDepartmentsQuery : IRequest<IEnumerable<DepartmentSummaryResponse>>
    {
        public int BranchId { get; set; }
    }

    /// <summary>
    /// Query to get departments by branch
    /// </summary>
    public class GetDepartmentsByBranchQuery : IRequest<IEnumerable<DepartmentSummaryResponse>>
    {
        public int BranchId { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// Query to get departments by company
    /// </summary>
    public class GetDepartmentsByCompanyQuery : IRequest<IEnumerable<DepartmentSummaryResponse>>
    {
        public int CompanyId { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// Query to search departments
    /// </summary>
    public class SearchDepartmentsQuery : IRequest<SearchDepartmentsResponse>
    {
        public string? SearchTerm { get; set; }
        public int? BranchId { get; set; }
        public int? CompanyId { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Query to get department by code
    /// </summary>
    public class GetDepartmentByCodeQuery : IRequest<DepartmentProfileResponse>
    {
        public string DepartmentCode { get; set; } = string.Empty;
        public int BranchId { get; set; }
    }

    /// <summary>
    /// Response for department profile
    /// </summary>
    public class DepartmentProfileResponse
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string? DepartmentCode { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy{ get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }
        public int DesignationCount { get; set; }
        public int EmployeeCount { get; set; }
    }

    /// <summary>
    /// Response for department summary
    /// </summary>
    public class DepartmentSummaryResponse
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string? DepartmentCode { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int DesignationCount { get; set; }
        public int EmployeeCount { get; set; }
    }

    /// <summary>
    /// Response for department search
    /// </summary>
    public class SearchDepartmentsResponse
    {
        public IEnumerable<DepartmentSummaryResponse> Departments { get; set; } = new List<DepartmentSummaryResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}


