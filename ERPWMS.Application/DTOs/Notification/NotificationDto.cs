using System;

namespace ERPWMS.Application.DTOs.Notification
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int? TemplateId { get; set; }
        public string? TemplateName { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime? SentDate { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public string? Type { get; set; }
        public string? ReferenceNo { get; set; }
        public string? ReferenceModule { get; set; }
        public string Status => IsRead == true ? "Read" : "Unread";
    }
}