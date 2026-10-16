using MediatR;
namespace ERPWMS.Application.Commands.Finance.BankAccount
{
    public class DeleteBankAccountCommand : IRequest<DeleteBankAccountResponse>
    {
        public int BankAccountId { get; set; }
        public int CompanyId { get; set; }
        public int DeletedBy { get; set; }
    }

    public class DeleteBankAccountResponse
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}

