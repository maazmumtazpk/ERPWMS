using System.Collections.Generic;

namespace ERPWMS.Web.ViewModels.Branch
{
    public class BranchSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public int? CompanyId { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public List<BranchViewModel> Branches { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
