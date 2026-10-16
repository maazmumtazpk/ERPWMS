using MediatR;
using ERPWMS.Application.DTOs.EmployeeBankAccount;

namespace ERPWMS.Application.Queries.EmployeeBankAccount.GetEmployeeBankAccountDetails
{
    public class GetEmployeeBankAccountDetailsQuery : IRequest<EmployeeBankAccountDto>
    {
        public int BankAccountId { get; set; }
        public int EmployeeId { get; set; }
    }
}
