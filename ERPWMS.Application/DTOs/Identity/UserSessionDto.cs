using System;

namespace ERPWMS.Application.DTOs.Identity
{
    public class UserSessionDto
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string SessionToken { get; set; } = string.Empty;
        public DateTime? LoginTime { get; set; }
        public DateTime? LastActivity { get; set; }
        public string? IPAddress { get; set; }
        public string? DeviceInfo { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string Status => IsActive == true ? "Active" : "Terminated";
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
    }
}