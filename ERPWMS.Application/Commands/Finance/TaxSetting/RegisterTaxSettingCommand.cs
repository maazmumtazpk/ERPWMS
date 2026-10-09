
using MediatR;
using System;

namespace ERPWMS.Application.Commands.Finance.TaxSetting
{
    public class RegisterTaxSettingCommand : IRequest<RegisterTaxSettingResponse>
    {
        public int CompanyId { get; set; }
        public string TaxName { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public decimal TaxRate { get; set; }
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
        public bool? IsDefault { get; set; }
        public string CreatedBy { get; set; }
    }

    public class RegisterTaxSettingResponse
    {
        public bool IsSuccess { get; set; }
        public int TaxId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}
