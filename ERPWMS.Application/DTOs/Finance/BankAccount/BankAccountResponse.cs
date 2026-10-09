using System;

namespace ERPWMS.Application.DTOs.Finance.BankAccount
{
    public class BankAccountResponse
    {
        public int BankAccountId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string AccountTitle { get; set; } = string.Empty;
        public string AccountNo { get; set; } = string.Empty;
        public string? Iban { get; set; }
        public string? BranchCode { get; set; }
        public string? AccountType { get; set; }
        public decimal OpeningBalance { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
