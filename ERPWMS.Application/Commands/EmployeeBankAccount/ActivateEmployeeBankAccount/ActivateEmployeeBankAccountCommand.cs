using MediatR;

namespace ERPWMS.Application.Commands.EmployeeBankAccount.ActivateEmployeeBankAccount
{
    public class ActivateEmployeeBankAccountCommand : IRequest<bool>
    {
        public int BankAccountId { get; set; }

        public int EmployeeId { get; set; }

        public int CurrentUserId { get; set; }
    }
}