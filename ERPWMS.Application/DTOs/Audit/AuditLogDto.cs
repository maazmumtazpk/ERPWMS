using System;

namespace ERPWMS.Application.DTOs.Audit
{
    public class AuditLogDto
    {
        public int AuditId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int? ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public string Action { get; set; } = string.Empty;
        public int RecordId { get; set; }
        public string RecordType { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime? Timestamp { get; set; }
        public string DisplayAction => Action switch
        {
            "CREATE" => "➕ Created",
            "UPDATE" => "✏️ Updated",
            "DELETE" => "🗑️ Deleted",
            "ACTIVATE" => "✅ Activated",
            "DEACTIVATE" => "⏸️ Deactivated",
            "LOGIN" => "🔐 Login",
            "LOGOUT" => "🚪 Logout",
            "APPROVE" => "✅ Approved",
            "PASSWORD_CHANGE" => "🔑 Password Changed",
            "SETTINGS_UPDATE" => "⚙️ Settings Updated",
            _ => Action
        };
    }
}