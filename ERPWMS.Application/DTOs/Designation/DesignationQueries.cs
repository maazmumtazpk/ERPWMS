using MediatR;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Designation
{
    public class GetDesignationProfileQuery : IRequest<DesignationProfileResponse>
    {
        public int DesignationId { get; set; }
    }

    public class GetActiveDesignationsQuery : IRequest<IEnumerable<DesignationSummaryResponse>>
    {
        public int DepartmentId { get; set; }
    }

    public class GetDesignationsByDepartmentQuery : IRequest<IEnumerable<DesignationSummaryResponse>>
    {
        public int DepartmentId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class GetDesignationsByCompanyQuery : IRequest<IEnumerable<DesignationSummaryResponse>>
    {
        public int CompanyId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class SearchDesignationsQuery : IRequest<SearchDesignationsResponse>
    {
        public string? SearchTerm { get; set; }
        public int? DepartmentId { get; set; }
        public int? CompanyId { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class DesignationProfileResponse
    {
        public int DesignationId { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public int? RankLevel { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
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
      
    }

    public class DesignationSummaryResponse
    {
        public int DesignationId { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public int? RankLevel { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
       
    }

    public class SearchDesignationsResponse
    {
        public IEnumerable<DesignationSummaryResponse> Designations { get; set; } = new List<DesignationSummaryResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}


