using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Notification
{
    public class CreateNotificationTemplateRequest
    {
        [Required(ErrorMessage = "Template name is required.")]
        [StringLength(100)]
        public string TemplateName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Template code is required.")]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z0-9_]+$", ErrorMessage = "Template code can only contain letters, numbers, and underscores.")]
        public string TemplateCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Body is required.")]
        public string Body { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Type { get; set; }

        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; }
    }
}