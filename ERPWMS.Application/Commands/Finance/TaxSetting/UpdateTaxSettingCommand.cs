using MediatR;
using System;

namespace ERPWMS.Application.Commands.Finance.TaxSetting
{
    public class UpdateTaxSettingCommand : IRequest<UpdateTaxSettingResponse>
    {
        public int TaxId { get; set; }
        public int CompanyId { get; set; }
        public string TaxName { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public decimal TaxRate { get; set; }
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsActive { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class UpdateTaxSettingResponse
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}
