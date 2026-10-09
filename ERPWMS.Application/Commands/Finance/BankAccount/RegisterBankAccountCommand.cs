using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Finance.BankAccount
{
    public class RegisterBankAccountCommand : IRequest<RegisterBankAccountResponse>
    {
        public int CompanyId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string AccountTitle { get; set; } = string.Empty;
        public string AccountNo { get; set; } = string.Empty;
        public string? Iban { get; set; }
        public string? BranchCode { get; set; }
        public string? AccountType { get; set; }
        public decimal OpeningBalance { get; set; }
        public string CreatedBy { get; set; }
    }

    public class RegisterBankAccountResponse
    {
        public bool IsSuccess { get; set; }
        public int BankAccountId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}
