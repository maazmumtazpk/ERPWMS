using ERPWMS.Application.DTOs.Finance.TaxSetting;
using ERPWMS.Domain.Interfaces;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Validators.Finance.TaxSetting
{
    public class UpdateTaxSettingValidator : AbstractValidator<UpdateTaxSettingRequest>
    {
        private readonly ITaxSettingRepository _repository;

        public UpdateTaxSettingValidator(ITaxSettingRepository repository)
        {
            _repository = repository;

            // ✅ TaxId required
            RuleFor(x => x.TaxId)
                .NotEmpty().WithMessage("Tax ID is required.")
                .NotEmpty().WithMessage("Invalid tax setting.");

            // ✅ CompanyId required
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("Company is required.")
                .NotEmpty().WithMessage("Invalid company.");

            // Tax Name
            RuleFor(x => x.TaxName)
                .NotEmpty().WithMessage("Tax Name is required.")
                .MaximumLength(50).WithMessage("Tax Name cannot exceed 50 characters.");

            // Tax Code - Unique per company (excluding self)
            RuleFor(x => x.TaxCode)
                .NotEmpty().WithMessage("Tax Code is required.")
                .MaximumLength(20).WithMessage("Tax Code cannot exceed 20 characters.")
                .MustAsync(async (request, code, cancellation) =>
                    !await _repository.IsCodeExistsAsync(request.CompanyId, code, request.TaxId))
                .WithMessage("Tax Code already exists in this company.");

            // Tax Rate
            RuleFor(x => x.TaxRate)
                .NotEmpty().WithMessage("Tax Rate is required.")
                .NotEmpty().WithMessage("Tax Rate must be greater than 0.")
                .LessThan(100).WithMessage("Tax Rate must be less than 100.");

            // Effective Dates
            RuleFor(x => x.EffectiveFrom)
                .NotEmpty().WithMessage("Effective From date is required.");

            RuleFor(x => x)
                .Must(x => !x.EffectiveTo.HasValue || x.EffectiveFrom <= x.EffectiveTo.Value)
                .WithMessage("Effective From date must be before or equal to Effective To date.");

            // If IsDefault is true, check no other default exists (excluding self)
            RuleFor(x => x)
                .MustAsync(async (request, cancellation) =>
                {
                    if (request.IsDefault != true) return true;
                    return !await _repository.HasDefaultTaxAsync(request.CompanyId, request.TaxId);
                })
                .WithMessage("A default tax setting already exists for this company.");
        }
    }
}

