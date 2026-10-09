using ERPWMS.Application.DTOs.System;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Services.System
{
    public class DashboardService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IUserSessionService _sessionService;

        public DashboardService(
            AppDbContext context,
            INotificationService notificationService,
            IUserSessionService sessionService)
        {
            _context = context;
            _notificationService = notificationService;
            _sessionService = sessionService;
        }

        public async Task<SystemAdminDashboardDto> GetDashboardDataAsync()
        {
            // ✅ Users - sahi hai
            var totalUsers = await _context.Users.CountAsync(u => u.IsDeleted != true);
            var activeUsers = await _context.Users.CountAsync(u => u.IsActive == true && u.IsDeleted != true);

            // ✅ Fixed: Using AspNetRoles instead of Roles
            var totalRoles = await _context.AspNetRoles.CountAsync();

            // ✅ Modules - sahi hai
            var totalModules = await _context.Modules.CountAsync(m => m.IsActive == true);

            // ✅ Notifications - sahi hai
            var unreadNotifications = await _context.Notifications.CountAsync(n => n.IsRead != true);

            // ✅ Sessions - sahi hai
            var activeSessions = await _sessionService.GetActiveSessionCountAsync();

            // ✅ AuditLogs - sahi hai
            var recentAuditEvents = await _context.AuditLogs.CountAsync();

            // ✅ Backup logs - sahi hai
            var totalBackups = await _context.DataBackupLogs.CountAsync();
            var successfulBackups = await _context.DataBackupLogs.CountAsync(b => b.Status == "Success");
            var failedBackups = await _context.DataBackupLogs.CountAsync(b => b.Status == "Failed");

            return new SystemAdminDashboardDto
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                TotalRoles = totalRoles,
                TotalModules = totalModules,
                UnreadNotifications = unreadNotifications,
                ActiveSessions = activeSessions,
                RecentAuditEvents = recentAuditEvents,
                TotalBackups = totalBackups,
                SuccessfulBackups = successfulBackups,
                FailedBackups = failedBackups
            };
        }
    }
}