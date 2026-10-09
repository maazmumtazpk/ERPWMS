using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.System
{
    public class UpdateSystemSettingRequest
    {
        public int SettingId { get; set; }

        [Required(ErrorMessage = "Setting key is required.")]
        [StringLength(100)]
        public string SettingKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Setting value is required.")]
        public string SettingValue { get; set; } = string.Empty;

        public string? DataType { get; set; }
        public bool? IsEncrypted { get; set; }
        public string? Description { get; set; }
        public int UpdatedBy { get; set; }
    }
}