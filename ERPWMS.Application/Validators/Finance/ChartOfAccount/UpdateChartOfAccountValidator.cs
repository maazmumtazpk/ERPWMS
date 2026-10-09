using ERPWMS.Application.DTOs.Finance.ChartOfAccount;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Finance.ChartOfAccount
{
    public class UpdateChartOfAccountValidator : AbstractValidator<UpdateChartOfAccountRequest>
    {
        private readonly IChartOfAccountRepository _repository;

        public UpdateChartOfAccountValidator(IChartOfAccountRepository repository)
        {
            _repository = repository;

            // ✅ 1. AccountId required
            RuleFor(x => x.AccountId)
                .NotEmpty().WithMessage("Account ID is required.")
                .NotEmpty().WithMessage("Invalid account.");

            // ✅ 2. CompanyId required
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("Company is required.")
                .NotEmpty().WithMessage("Invalid company selected.");

            // 3. Account Code - Required + Unique per company (excluding self)
            RuleFor(x => x.AccountCode)
                .NotEmpty().WithMessage("Account Code is required.")
                .MaximumLength(20).WithMessage("Account Code cannot exceed 20 characters.")
                .MustAsync(async (request, code, cancellation) =>
                    !await _repository.IsCodeExistsAsync(request.CompanyId, code, request.AccountId))
                .WithMessage("Account Code already exists in this company.");

            // 4. Account Name
            RuleFor(x => x.AccountName)
                .NotEmpty().WithMessage("Account Name is required.")
                .MaximumLength(100).WithMessage("Account Name cannot exceed 100 characters.");

            // 5. Account Type
            RuleFor(x => x.AccountType)
                .NotEmpty().WithMessage("Account Type is required.")
                .Must(BeValidAccountType).WithMessage("Invalid Account Type. Allowed: Asset, Liability, Equity, Income, Expense");

            // 6. Parent Account Validation (cannot be self)
            RuleFor(x => x.ParentAccountId)
                .Must((request, parentId) => parentId != request.AccountId)
                .WithMessage("Account cannot be its own parent.")
                .MustAsync(async (request, parentId, cancellation) =>
                    await IsParentValidAsync(request.CompanyId, parentId, cancellation))
                .WithMessage("Invalid parent account selected.")
                .When(x => x.ParentAccountId.HasValue);
        }

        private bool BeValidAccountType(string accountType)
        {
            var validTypes = new[] { "Asset", "Liability", "Equity", "Income", "Expense" };
            return !string.IsNullOrEmpty(accountType) && validTypes.Contains(accountType);
        }

        private async Task<bool> IsParentValidAsync(int companyId, int? parentId, CancellationToken cancellationToken)
        {
            if (!parentId.HasValue) return true;

            var parent = await _repository.GetByIdAsync(parentId.Value, companyId);
            if (parent == null) return false;

            return parent.IsActive == true && parent.IsDeleted != true;
        }
    }
}

