using MediatR;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Company
{
    
    public class GetCompanyProfileQuery : IRequest<CompanyProfileResponse>
    {
        public int CompanyId { get; set; }
    }

    
    public class GetActiveCompaniesQuery : IRequest<IEnumerable<CompanySummaryResponse>>
    {
    }

   
    public class SearchCompaniesQuery : IRequest<SearchCompaniesResponse>
    {
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

   
    public class GetCompanyByNameQuery : IRequest<CompanyProfileResponse>
    {
        public string CompanyName { get; set; } = string.Empty;
    }

    
    public class CompanyProfileResponse
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? RegistrationNumber { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy{ get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }
        public int BranchCount { get; set; }
        public int UserCount { get; set; }
    }

  
    public class CompanySummaryResponse
    {

        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? RegistrationNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int WarehouseCount { get; set; }
        public int BranchCount { get; set; }
        public int UserCount { get; set; }
    }

  
    public class SearchCompaniesResponse
    {
        public IEnumerable<CompanySummaryResponse> Companies { get; set; } = new List<CompanySummaryResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
