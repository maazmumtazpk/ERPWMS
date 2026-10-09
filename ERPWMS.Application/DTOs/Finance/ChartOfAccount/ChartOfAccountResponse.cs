using System;

namespace ERPWMS.Application.DTOs.Finance.ChartOfAccount
{
    public class ChartOfAccountResponse
    {
        public int AccountId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public int? ParentAccountId { get; set; }
        public string? ParentAccountName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
