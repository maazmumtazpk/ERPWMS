using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface INotificationService
    {
        // Notification management
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
        Task<Notification?> GetNotificationByIdAsync(int notificationId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<Notification> CreateNotificationAsync(int userId, int? templateId, string subject, string message,
            string? type = null, string? referenceNo = null, string? referenceModule = null);
        Task<bool> MarkAsReadAsync(int notificationId, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);

        // Template-based notification
        Task<Notification> SendTemplateNotificationAsync(
            int userId,
            string templateCode,
            Dictionary<string, string> variables,
            string? type = null,
            string? referenceNo = null,
            string? referenceModule = null);
    }
}