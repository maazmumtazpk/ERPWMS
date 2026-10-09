using MediatR;
using ERPWMS.Application.DTOs.EmployeeBankAccount;

namespace ERPWMS.Application.Commands.EmployeeBankAccount.ChangeEmployeeBankAccount
{
    public class ChangeEmployeeBankAccountCommand
        : IRequest<ChangeEmployeeBankAccountResponse>
    {
        public int BankAccountId { get; set; }

        public int EmployeeId { get; set; }

        public CreateEmployeeBankAccountRequest Request { get; set; } = new();

        public int CurrentUserId { get; set; }
    }

    public class ChangeEmployeeBankAccountResponse
    {
        public int BankAccountId { get; set; }

        public string Message { get; set; } =
            "Employee bank account updated successfully.";
    }
}