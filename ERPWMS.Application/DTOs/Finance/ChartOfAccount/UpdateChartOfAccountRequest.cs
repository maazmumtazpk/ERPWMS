namespace ERPWMS.Application.DTOs.Finance.ChartOfAccount
{
    public class UpdateChartOfAccountRequest
    {
        public int AccountId { get; set; }

        // ✅ Will be auto-filled from session / hidden field
        public int CompanyId { get; set; }

        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public int? ParentAccountId { get; set; }
        public bool? IsActive { get; set; }

        // Audit fields - will be set by handler
        public int UpdatedBy { get; set; }
    }
}
