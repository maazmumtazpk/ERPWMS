using System;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface ITaxSettingBusinessRules
    {
        Task ValidateCodeUniquenessAsync(int companyId, string taxCode, int? excludeId = null);
        Task ValidateTaxRateAsync(decimal taxRate);
        Task ValidateEffectiveDatesAsync(DateOnly effectiveFrom, DateOnly? effectiveTo);
        Task ResetDefaultTaxAsync(int companyId, int? excludeId = null);
        Task<bool> IsDefaultTaxAllowedAsync(int companyId, int? excludeId = null);
    }
}
