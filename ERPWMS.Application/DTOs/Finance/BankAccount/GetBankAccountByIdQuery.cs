using MediatR;

namespace ERPWMS.Application.DTOs.Finance.BankAccount
{
    public class GetBankAccountByIdQuery : IRequest<BankAccountResponse?>
    {
        public int BankAccountId { get; set; }
        public int CompanyId { get; set; }
    }
}
