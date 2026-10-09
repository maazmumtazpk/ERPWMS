namespace ERPWMS.Application.DTOs.Finance.BankAccount
{
    public class UpdateBankAccountRequest
    {
        public int BankAccountId { get; set; }
        public int CompanyId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string AccountTitle { get; set; } = string.Empty;
        public string AccountNo { get; set; } = string.Empty;
        public string? Iban { get; set; }
        public string? BranchCode { get; set; }
        public string? AccountType { get; set; }
        public decimal OpeningBalance { get; set; }
        public bool? IsActive { get; set; }
        public int UpdatedBy { get; set; }
    }
}
