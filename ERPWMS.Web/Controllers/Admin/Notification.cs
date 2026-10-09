using ERPWMS.Application.DTOs.Notification;
using ERPWMS.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPWMS.Controllers.Admin
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int id) ? id : 0;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId);

            var dtos = notifications.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                Username = n.User?.UserName ?? string.Empty,
                TemplateId = n.TemplateId,
                TemplateName = n.Template?.TemplateName,
                Subject = n.Subject,
                Message = n.Message,
                SentDate = n.SentDate,
                IsRead = n.IsRead,
                ReadDate = n.ReadDate,
                Type = n.Type,
                ReferenceNo = n.ReferenceNo,
                ReferenceModule = n.ReferenceModule
            });

            ViewBag.UnreadCount = unreadCount;
            return View(dtos);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Json(new { count = 0 });

            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Json(new { count });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var result = await _notificationService.MarkAsReadAsync(id, userId);
            if (result)
                TempData["Success"] = "Notification marked as read.";
            else
                TempData["Error"] = "Failed to mark notification as read.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            await _notificationService.MarkAllAsReadAsync(userId);
            TempData["Success"] = "All notifications marked as read.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null || notification.UserId != userId)
                return NotFound();

            // Mark as read when viewed
            if (notification.IsRead != true)
            {
                await _notificationService.MarkAsReadAsync(id, userId);
                notification.IsRead = true;
            }

            var dto = new NotificationDto
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserId,
                Username = notification.User?.UserName ?? string.Empty,
                TemplateId = notification.TemplateId,
                TemplateName = notification.Template?.TemplateName,
                Subject = notification.Subject,
                Message = notification.Message,
                SentDate = notification.SentDate,
                IsRead = notification.IsRead,
                ReadDate = notification.ReadDate,
                Type = notification.Type,
                ReferenceNo = notification.ReferenceNo,
                ReferenceModule = notification.ReferenceModule
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var result = await _notificationService.DeleteNotificationAsync(id, userId);
            if (result)
                TempData["Success"] = "Notification deleted.";
            else
                TempData["Error"] = "Failed to delete notification.";

            return RedirectToAction(nameof(Index));
        }
    

        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Json(new System.Collections.Generic.List<object>());

            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            var list = (notifications ?? System.Linq.Enumerable.Empty<ERPWMS.Database.Models.Notification>()).Select(n => new
            {
                notificationId = n.NotificationId,
                subject = n.Subject,
                message = n.Message,
                type = n.Type,
                isRead = n.IsRead == true,
                sentDate = n.SentDate
            }).ToList();
            return Json(list);
        }
    }
}

