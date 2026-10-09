using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Web.ViewModels.Designation
{
    public class RegisterDesignationViewModel
    {
        public int? DesignationId { get; set; }
        [Required]
        public string DesignationName { get; set; } = string.Empty;
        public int? RankLevel { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public bool? IsActive { get; set; } = true;

        public string? Title { get; set; }
        public bool IsEdit { get; set; }
        public string? SubmitButtonText { get; set; }
        public string? CancelUrl { get; set; }
    }

    public class DesignationSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public int? DepartmentId { get; set; }
        public int? CompanyId { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public List<DesignationItem> Designations { get; set; } = new();
        public string? ResultsSummary { get; set; }
        public bool HasResults => Designations != null && Designations.Count > 0;
        public int TotalPages { get; set; }
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }

    public class DesignationItem
    {
        public int DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public int? RankLevel { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? CompanyName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
