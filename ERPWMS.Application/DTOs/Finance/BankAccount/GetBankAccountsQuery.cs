using MediatR;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Finance.BankAccount
{
    public class GetBankAccountsQuery : IRequest<List<BankAccountResponse>>
    {
        public int CompanyId { get; set; }
        public string? SearchTerm { get; set; }
        public string? AccountType { get; set; }
        public bool? IsActive { get; set; }
        public bool IncludeDeleted { get; set; } = false;
    }
}
