namespace ERPWMS.Application.DTOs.System
{
    public class SystemAdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalRoles { get; set; }
        public int TotalModules { get; set; }
        public int UnreadNotifications { get; set; }
        public int ActiveSessions { get; set; }
        public int RecentAuditEvents { get; set; }
        public int TotalBackups { get; set; }
        public int SuccessfulBackups { get; set; }
        public int FailedBackups { get; set; }
    }
}