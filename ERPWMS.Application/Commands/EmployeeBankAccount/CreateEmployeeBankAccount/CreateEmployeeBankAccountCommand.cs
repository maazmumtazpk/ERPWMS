using MediatR;
using ERPWMS.Application.DTOs.EmployeeBankAccount;

namespace ERPWMS.Application.Commands.EmployeeBankAccount.CreateEmployeeBankAccount
{
    public class CreateEmployeeBankAccountCommand
        : IRequest<CreateEmployeeBankAccountResponse>
    {
        public CreateEmployeeBankAccountRequest Request { get; set; } = new();

        public int CurrentUserId { get; set; }
    }

    public class CreateEmployeeBankAccountResponse
    {
        public int BankAccountId { get; set; }

        public string Message { get; set; } =
            "Employee bank account created successfully.";
    }
}