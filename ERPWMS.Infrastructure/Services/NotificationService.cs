
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Services
{
    public class NotificationService(
        AppDbContext context,
        INotificationTemplateService templateService) : INotificationService
    {
        private readonly AppDbContext _context = context;
        private readonly INotificationTemplateService _templateService = templateService;

        // ============================================================
        // GET USER NOTIFICATIONS
        // ============================================================

        public async Task<IEnumerable<ERPWMS.Database.Models.Notification>>
            GetUserNotificationsAsync(
                int userId,
                bool unreadOnly = false)
        {
            var query = _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Template)
                .Where(n => n.UserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => n.IsRead != true);
            }

            return await query
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        // ============================================================
        // GET NOTIFICATION BY ID
        // ============================================================

        public async Task<ERPWMS.Database.Models.Notification?>
            GetNotificationByIdAsync(int notificationId)
        {
            return await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Template)
                .FirstOrDefaultAsync(
                    n => n.NotificationId == notificationId);
        }

        // ============================================================
        // GET UNREAD COUNT
        // ============================================================

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(
                    n => n.UserId == userId &&
                         n.IsRead != true);
        }

        // ============================================================
        // CREATE NOTIFICATION
        // ============================================================

        public async Task<ERPWMS.Database.Models.Notification>
            CreateNotificationAsync(
                int userId,
                int? templateId,
                string subject,
                string message,
                string? type = null,
                string? referenceNo = null,
                string? referenceModule = null)
        {
            var now = DateTime.UtcNow;

            var notification =
                new ERPWMS.Database.Models.Notification
                {
                    UserId = userId,
                    TemplateId = templateId,
                    Subject = subject,
                    Message = message,
                    Type = type,
                    ReferenceNo = referenceNo,
                    ReferenceModule = referenceModule,
                    SentDate = now,
                    IsRead = false,
                    CreatedBy = userId.ToString(),
                    CreatedDate = now
                };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return notification;
        }

        // ============================================================
        // SEND TEMPLATE NOTIFICATION
        // ============================================================

        public async Task<ERPWMS.Database.Models.Notification>
            SendTemplateNotificationAsync(
                int userId,
                string templateCode,
                Dictionary<string, string> variables,
                string? type = null,
                string? referenceNo = null,
                string? referenceModule = null)
        {
            // --------------------------------------------------------
            // GET TEMPLATE
            // --------------------------------------------------------

            var template =
                await _templateService.GetTemplateByCodeAsync(
                    templateCode);

            if (template is null)
            {
                throw new InvalidOperationException(
                    $"Template '{templateCode}' not found.");
            }

            if (template.IsActive != true)
            {
                throw new InvalidOperationException(
                    $"Template '{templateCode}' is inactive.");
            }

            // --------------------------------------------------------
            // PROCESS TEMPLATE
            // --------------------------------------------------------

            var processed =
                await _templateService.ProcessTemplateAsync(
                    template,
                    variables);

            var parts = processed.Split("|||");

            var subject =
                parts.Length > 0 &&
                !string.IsNullOrWhiteSpace(parts[0])
                    ? parts[0]
                    : template.Subject;

            var body =
                parts.Length > 1 &&
                !string.IsNullOrWhiteSpace(parts[1])
                    ? parts[1]
                    : template.Body;

            // --------------------------------------------------------
            // CREATE NOTIFICATION
            // --------------------------------------------------------

            return await CreateNotificationAsync(
                userId,
                template.TemplateId,
                subject,
                body,
                type ?? template.Type,
                referenceNo,
                referenceModule);
        }

        // ============================================================
        // MARK AS READ
        // ============================================================

        public async Task<bool> MarkAsReadAsync(
            int notificationId,
            int userId)
        {
            var notification =
                await GetNotificationByIdAsync(notificationId);

            if (notification is null)
            {
                return false;
            }

            if (notification.UserId != userId)
            {
                return false;
            }

            notification.IsRead = true;
            notification.ReadDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        // ============================================================
        // MARK ALL AS READ
        // ============================================================

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var notifications =
                await _context.Notifications
                    .Where(n =>
                        n.UserId == userId &&
                        n.IsRead != true)
                    .ToListAsync();

            if (notifications.Count == 0)
            {
                return true;
            }

            var now = DateTime.UtcNow;

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadDate = now;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        // ============================================================
        // DELETE NOTIFICATION
        // ============================================================

        public async Task<bool> DeleteNotificationAsync(
            int notificationId,
            int userId)
        {
            var notification =
                await GetNotificationByIdAsync(notificationId);

            if (notification is null)
            {
                return false;
            }

            if (notification.UserId != userId)
            {
                return false;
            }

            _context.Notifications.Remove(notification);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}

