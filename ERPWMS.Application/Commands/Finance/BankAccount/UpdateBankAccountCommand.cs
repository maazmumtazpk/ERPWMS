using MediatR;
namespace ERPWMS.Application.Commands.Finance.BankAccount
{
    public class UpdateBankAccountCommand : IRequest<UpdateBankAccountResponse>
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

    public class UpdateBankAccountResponse
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}

