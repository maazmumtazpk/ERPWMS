using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using System;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Infrastructure
{
    public class TaxSettingBusinessRules : ITaxSettingBusinessRules
    {
        private readonly ITaxSettingRepository _repository;

        public TaxSettingBusinessRules(ITaxSettingRepository repository)
        {
            _repository = repository;
        }

        public async Task ValidateCodeUniquenessAsync(int companyId, string taxCode, int? excludeId = null)
        {
            var exists = await _repository.IsCodeExistsAsync(companyId, taxCode, excludeId);
            if (exists)
                throw new InvalidOperationException("Tax Code already exists in this company.");
        }

        public Task ValidateTaxRateAsync(decimal taxRate)
        {
            if (taxRate <= 0)
                throw new InvalidOperationException("Tax Rate must be greater than 0.");
            if (taxRate >= 100)
                throw new InvalidOperationException("Tax Rate must be less than 100.");
            return Task.CompletedTask;
        }

        public Task ValidateEffectiveDatesAsync(DateOnly effectiveFrom, DateOnly? effectiveTo)
        {
            if (effectiveTo.HasValue && effectiveFrom > effectiveTo)
                throw new InvalidOperationException("Effective From date must be before or equal to Effective To date.");
            return Task.CompletedTask;
        }

        public async Task ResetDefaultTaxAsync(int companyId, int? excludeId = null)
        {
            var currentDefault = await _repository.GetDefaultTaxAsync(companyId);
            if (currentDefault != null && (excludeId == null || currentDefault.TaxId != excludeId))
            {
                currentDefault.IsDefault = false;
                await _repository.UpdateAsync(currentDefault);
            }
        }

        public async Task<bool> IsDefaultTaxAllowedAsync(int companyId, int? excludeId = null)
        {
            return !await _repository.HasDefaultTaxAsync(companyId, excludeId);
        }
    }
}



