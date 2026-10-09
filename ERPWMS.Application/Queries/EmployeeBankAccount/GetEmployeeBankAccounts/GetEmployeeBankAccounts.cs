using MediatR;
using ERPWMS.Application.DTOs.EmployeeBankAccount;

namespace ERPWMS.Application.Queries.EmployeeBankAccount.GetEmployeeBankAccounts
{
    public class GetEmployeeBankAccountsQuery
        : IRequest<IEnumerable<EmployeeBankAccountDto>>
    {
        public int EmployeeId { get; set; }
    }
}