using System;

namespace ERPWMS.Application.DTOs.Audit
{
    public class LoginHistoryDto
    {
        public int LoginId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string? IPAddress { get; set; }
        public string? DeviceInfo { get; set; }
        public string? Status { get; set; }
        public string Duration
        {
            get
            {
                if (!LoginTime.HasValue) return "-";
                var end = LogoutTime ?? DateTime.UtcNow;
                var diff = end - LoginTime.Value;
                return $"{diff.Hours}h {diff.Minutes}m";
            }
        }
        public string StatusDisplay => Status == "Success" ? "✅ Success" : "❌ Failed";
    }
}