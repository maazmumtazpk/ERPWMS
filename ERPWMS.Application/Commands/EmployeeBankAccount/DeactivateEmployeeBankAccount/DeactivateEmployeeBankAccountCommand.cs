using MediatR;

namespace ERPWMS.Application.Commands.EmployeeBankAccount.DeactivateEmployeeBankAccount
{
    public class DeactivateEmployeeBankAccountCommand : IRequest<bool>
    {
        public int BankAccountId { get; set; }

        public int EmployeeId { get; set; }

        public int CurrentUserId { get; set; }
    }
}