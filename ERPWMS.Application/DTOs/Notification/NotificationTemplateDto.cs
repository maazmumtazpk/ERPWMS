using System;

namespace ERPWMS.Application.DTOs.Notification
{
    public class NotificationTemplateDto
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string TemplateCode { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? Type { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}