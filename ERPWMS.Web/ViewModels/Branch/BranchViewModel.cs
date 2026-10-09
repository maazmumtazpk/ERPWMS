using System;

namespace ERPWMS.Web.ViewModels.Branch
{
    public class BranchViewModel
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public int EmployeeCount { get; set; }
        public int WarehouseCount { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}
