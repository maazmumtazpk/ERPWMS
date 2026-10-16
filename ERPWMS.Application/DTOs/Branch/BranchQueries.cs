using MediatR;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Branch
{
    /// <summary>
    /// Query to get branch profile
    /// </summary>
    public class GetBranchProfileQuery : IRequest<BranchProfileResponse>
    {
        public int BranchId { get; set; }
    }

    /// <summary>
    /// Query to get all active branches for a company
    /// </summary>
    public class GetActiveBranchesQuery : IRequest<IEnumerable<BranchSummaryResponse>>
    {
        public int CompanyId { get; set; }
    }

    /// <summary>
    /// Query to search branches
    /// </summary>
    public class SearchBranchesQuery : IRequest<SearchBranchesResponse>
    {
        public string? SearchTerm { get; set; }
        public int? CompanyId { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Query to get branch by code
    /// </summary>
    public class GetBranchByCodeQuery : IRequest<BranchProfileResponse>
    {
        public string BranchCode { get; set; } = string.Empty;
        public int CompanyId { get; set; }
    }

    /// <summary>
    /// Query to get branches by company
    /// </summary>
    public class GetBranchesByCompanyQuery : IRequest<IEnumerable<BranchSummaryResponse>>
    {
        public int CompanyId { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// Response for branch profile
    /// </summary>
    public class BranchProfileResponse
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string? BranchCode { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
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
        public int EmployeeCount { get; set; }
        public int WarehouseCount { get; set; }
    }

    /// <summary>
    /// Response for branch summary
    /// </summary>
    public class BranchSummaryResponse
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string? BranchCode { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int EmployeeCount { get; set; }
        public int WarehouseCount { get; set; }
    }

    /// <summary>
    /// Response for branch search
    /// </summary>
    public class SearchBranchesResponse
    {
        public IEnumerable<BranchSummaryResponse> Branches { get; set; } = new List<BranchSummaryResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
