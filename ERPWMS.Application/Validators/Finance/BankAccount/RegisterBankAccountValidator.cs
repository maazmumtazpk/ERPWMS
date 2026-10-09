
using ERPWMS.Application.DTOs.Finance.BankAccount;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Finance.BankAccount
{
    public class RegisterBankAccountValidator
        : AbstractValidator<RegisterBankAccountRequest>
    {
        private readonly IBankAccountRepository _repository;

        public RegisterBankAccountValidator(
            IBankAccountRepository repository)
        {
            _repository = repository;

            // =====================================================
            // COMPANY
            // =====================================================

            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company is required.")

                .NotEmpty()
                .WithMessage("Invalid company.");


            // =====================================================
            // BANK NAME
            // =====================================================

            RuleFor(x => x.BankName)
                .NotEmpty()
                .WithMessage("Bank Name is required.")

                .MaximumLength(100)
                .WithMessage(
                    "Bank Name cannot exceed 100 characters.");


            // =====================================================
            // ACCOUNT TITLE
            // =====================================================

            RuleFor(x => x.AccountTitle)
                .NotEmpty()
                .WithMessage("Account Title is required.")

                .MaximumLength(100)
                .WithMessage(
                    "Account Title cannot exceed 100 characters.");


            // =====================================================
            // ACCOUNT NUMBER
            // =====================================================

            RuleFor(x => x.AccountNo)
                .NotEmpty()
                .WithMessage("Account Number is required.")

                .MaximumLength(50)
                .WithMessage(
                    "Account Number cannot exceed 50 characters.")

                .MustAsync(
                    async (request, accountNo, cancellationToken) =>
                    {
                        if (string.IsNullOrWhiteSpace(accountNo))
                        {
                            return true;
                        }

                        return !await _repository
                            .IsAccountNoExistsAsync(
                                request.CompanyId,
                                accountNo,
                                null);
                    })
                .WithMessage(
                    "Account Number already exists in this company.");


            // =====================================================
            // IBAN
            // =====================================================

            RuleFor(x => x.Iban)
                .MaximumLength(50)
                .WithMessage(
                    "IBAN cannot exceed 50 characters.")

                .Must(BeValidIban)
                .WithMessage(
                    "Invalid IBAN format.")

                .When(x =>
                    !string.IsNullOrWhiteSpace(x.Iban));


            // =====================================================
            // ACCOUNT TYPE
            // =====================================================

            RuleFor(x => x.AccountType)
                .Must(IsValidAccountType)
                .WithMessage(
                    "Invalid Account Type.");
        }


        // =========================================================
        // IBAN VALIDATION
        // =========================================================

        private static bool BeValidIban(string? iban)
        {
            if (string.IsNullOrWhiteSpace(iban))
            {
                return true;
            }

            iban = iban
                .Replace(" ", string.Empty)
                .Trim()
                .ToUpperInvariant();

            return Regex.IsMatch(
                iban,
                @"^[A-Z]{2}[0-9]{2}[A-Z0-9]{4,}$");
        }


        // =========================================================
        // ACCOUNT TYPE VALIDATION
        // =========================================================

        private static bool IsValidAccountType(string? type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return true;
            }

            return new[]
            {
                "Current",
                "Saving",
                "Fixed",
                "Others"
            }
            .Contains(
                type.Trim(),
                StringComparer.OrdinalIgnoreCase);
        }
    }
}


