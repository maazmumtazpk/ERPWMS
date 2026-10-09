using System;

namespace ERPWMS.Application.DTOs.Finance.TaxSetting
{
    public class TaxSettingResponse
    {
        public int TaxId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string TaxName { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public decimal TaxRate { get; set; }
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
