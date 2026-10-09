using System;

namespace ERPWMS.Application.DTOs.Finance.TaxSetting
{
    public class UpdateTaxSettingRequest
    {
        public int TaxId { get; set; }

        public int CompanyId { get; set; }

        public string TaxName { get; set; } = string.Empty;

        public string TaxCode { get; set; } = string.Empty;

        public decimal TaxRate { get; set; }

        public DateOnly EffectiveFrom { get; set; }

        public DateOnly? EffectiveTo { get; set; }

        // CHANGE: bool? → bool
        public bool IsDefault { get; set; }

        // CHANGE: bool? → bool
        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }
    }
}