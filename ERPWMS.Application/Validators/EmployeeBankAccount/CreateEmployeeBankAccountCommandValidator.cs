using FluentValidation;
using ERPWMS.Application.Commands.EmployeeBankAccount.CreateEmployeeBankAccount;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.EmployeeBankAccount
{
    public class CreateEmployeeBankAccountCommandValidator : AbstractValidator<CreateEmployeeBankAccountCommand>
    {
        public CreateEmployeeBankAccountCommandValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .NotEmpty().WithMessage("Employee is required");

            RuleFor(x => x.Request.BankName)
                .NotEmpty().WithMessage("Bank name is required")
                .MaximumLength(100).WithMessage("Bank name cannot exceed 100 characters");

            RuleFor(x => x.Request.AccountNumber)
                .NotEmpty().WithMessage("Account number is required")
                .MaximumLength(50).WithMessage("Account number cannot exceed 50 characters");

            RuleFor(x => x.Request.Iban)
                .MaximumLength(50).WithMessage("IBAN cannot exceed 50 characters");

            RuleFor(x => x.Request.AccountType)
                .MaximumLength(50).WithMessage("Account type cannot exceed 50 characters");
        }
    }
}
