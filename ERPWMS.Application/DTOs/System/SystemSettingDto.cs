using System;

namespace ERPWMS.Application.DTOs.System
{
    public class SystemSettingDto
    {
        public int SettingId { get; set; }
        public string SettingKey { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
        public string? DataType { get; set; }
        public bool? IsEncrypted { get; set; }
        public string? Description { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string DisplayValue => IsEncrypted == true ? "********" : SettingValue;
    }
}