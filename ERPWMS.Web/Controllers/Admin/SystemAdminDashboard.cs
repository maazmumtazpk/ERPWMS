using ERPWMS.Application.DTOs.Audit;
using ERPWMS.Application.DTOs.Identity;
using ERPWMS.Application.DTOs.System;
using ERPWMS.Domain.Services;
using ERPWMS.Infrastructure.Services.System;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Database;

namespace ERPWMS.Web.Controllers.Admin
{
    [Authorize(Policy = "SystemAdmin.View")]
    public class SystemAdminDashboardController(
        DashboardService dashboardService,
        IAuditLogService auditLogService,
        IUserSessionService sessionService,
        AppDbContext context) : Controller
    {
        private readonly DashboardService _dashboardService = dashboardService;
        private readonly IAuditLogService _auditLogService = auditLogService;
        private readonly IUserSessionService _sessionService = sessionService;
        private readonly AppDbContext _context = context;

        // ============================================================
        // DASHBOARD
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var dashboardData = await _dashboardService.GetDashboardDataAsync();

            var recentAudits = await _auditLogService.GetAuditLogsAsync(page: 1, pageSize: 10);

            var recentAuditDtos = recentAudits
                .Select(a => new AuditLogDto
                {
                    AuditId = a.AuditId,
                    UserId = a.UserId,
                    Username = a.User?.UserName ?? "Unknown",
                    ModuleId = a.ModuleId,
                    ModuleName = a.Module?.ModuleName,
                    Action = a.Action,
                    RecordId = a.RecordId,
                    RecordType = a.RecordType,
                    OldValue = a.OldValue,
                    NewValue = a.NewValue,
                    IPAddress = a.IPAddress,
                    UserAgent = a.UserAgent,
                    Timestamp = a.Timestamp
                })
                .ToList();

            ViewBag.RecentAudits = recentAuditDtos;

            var activeSessions = await _sessionService.GetActiveSessionsAsync();

            var sessionDtos = activeSessions
                .Select(s => new UserSessionDto
                {
                    SessionId = s.SessionId,
                    UserId = s.UserId,
                    Username = s.User?.UserName ?? "Unknown",
                    SessionToken = s.SessionToken,
                    LoginTime = s.LoginTime,
                    LastActivity = s.LastActivity,
                    IPAddress = s.IPAddress,
                    DeviceInfo = s.DeviceInfo,
                    IsActive = s.IsActive,
                    LogoutTime = s.LogoutTime
                })
                .ToList();

            ViewBag.ActiveSessions = sessionDtos;

            return View(dashboardData);
        }

        // ============================================================
        // AUDIT LOGS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> AuditLogs(
            int? userId = null,
            int? moduleId = null,
            string? action = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;

            var logs = await _auditLogService.GetAuditLogsAsync(
                userId, moduleId, action, fromDate, toDate, null, null, page, pageSize);

            var totalCount = await _auditLogService.GetAuditLogCountAsync(
                userId, moduleId, action, fromDate, toDate);

            var dtos = logs
                .Select(a => new AuditLogDto
                {
                    AuditId = a.AuditId,
                    UserId = a.UserId,
                    Username = a.User?.UserName ?? "Unknown",
                    ModuleId = a.ModuleId,
                    ModuleName = a.Module?.ModuleName,
                    Action = a.Action,
                    RecordId = a.RecordId,
                    RecordType = a.RecordType,
                    OldValue = a.OldValue,
                    NewValue = a.NewValue,
                    IPAddress = a.IPAddress,
                    UserAgent = a.UserAgent,
                    Timestamp = a.Timestamp
                })
                .ToList();

            var distinctActions = await _auditLogService.GetDistinctActionsAsync();
            var distinctRecordTypes = await _auditLogService.GetDistinctRecordTypesAsync();

            ViewBag.DistinctActions = distinctActions;
            ViewBag.DistinctRecordTypes = distinctRecordTypes;
            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(dtos);
        }

        // ============================================================
        // AUDIT LOG DETAILS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> AuditLogDetails(int id)
        {
            var log = await _auditLogService.GetAuditLogByIdAsync(id);
            if (log == null) return NotFound();

            var dto = new AuditLogDto
            {
                AuditId = log.AuditId,
                UserId = log.UserId,
                Username = log.User?.UserName ?? "Unknown",
                ModuleId = log.ModuleId,
                ModuleName = log.Module?.ModuleName,
                Action = log.Action,
                RecordId = log.RecordId,
                RecordType = log.RecordType,
                OldValue = log.OldValue,
                NewValue = log.NewValue,
                IPAddress = log.IPAddress,
                UserAgent = log.UserAgent,
                Timestamp = log.Timestamp
            };

            return View(dto);
        }

        // ============================================================
        // LOGIN HISTORY
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> LoginHistory(
            int? userId = null,
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;

            IQueryable<LoginHistory> query = _context.LoginHistories.Include(h => h.User);

            if (userId.HasValue)
                query = query.Where(h => h.UserId == userId);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(h => h.Status == status);

            if (fromDate.HasValue)
                query = query.Where(h => h.LoginTime >= fromDate);

            if (toDate.HasValue)
                query = query.Where(h => h.LoginTime <= toDate);

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(h => h.LoginTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = logs
                .Select(h => new LoginHistoryDto
                {
                    LoginId = h.LoginId,
                    UserId = h.UserId,
                    Username = h.User?.UserName ?? "Unknown",
                    LoginTime = h.LoginTime,
                    LogoutTime = h.LogoutTime,
                    IPAddress = h.IPAddress,
                    DeviceInfo = h.DeviceInfo,
                    Status = h.Status
                })
                .ToList();

            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(dtos);
        }

        // ============================================================
        // ACTIVE SESSIONS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> ActiveSessions()
        {
            var sessions = await _sessionService.GetActiveSessionsAsync();

            var dtos = sessions
                .Select(s => new UserSessionDto
                {
                    SessionId = s.SessionId,
                    UserId = s.UserId,
                    Username = s.User?.UserName ?? "Unknown",
                    SessionToken = s.SessionToken,
                    LoginTime = s.LoginTime,
                    LastActivity = s.LastActivity,
                    IPAddress = s.IPAddress,
                    DeviceInfo = s.DeviceInfo,
                    IsActive = s.IsActive,
                    LogoutTime = s.LogoutTime
                })
                .ToList();

            return View(dtos);
        }

        // ============================================================
        // TERMINATE SESSION
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> TerminateSession(int id)
        {
            var result = await _sessionService.TerminateSessionAsync(id);

            if (result)
                TempData["Success"] = "Session terminated successfully.";
            else
                TempData["Error"] = "Session not found.";

            return RedirectToAction(nameof(ActiveSessions));
        }

        // ============================================================
        // BACKUP LOGS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> BackupLogs(int page = 1, int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;

            IQueryable<DataBackupLog> query = _context.DataBackupLogs;

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(b => b.BackupDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // FileSizeMB property (agar model mein nahi hai to calculate karo)
            var dtos = logs
                .Select(b => new DataBackupLogDto
                {
                    BackupId = b.BackupId,
                    BackupDate = b.BackupDate,
                    BackupType = b.BackupType,
                    FileSizeMB = b.FileSizeMb,
                    FilePath = b.FilePath,
                    Status = b.Status,
                    ErrorMessage = b.ErrorMessage,
                    CreatedBy = b.CreatedBy,
                    CreatedByUsername = b.CreatedBy ?? "System",
                    CreatedDate = b.CreatedDate
                })
                .ToList();

            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(dtos);
        }

        // ============================================================
        // AJAX ENDPOINTS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var activeSessions = await _context.UserSessions.CountAsync(s => s.IsActive == true);
                var unreadNotif = await _context.Notifications.CountAsync(n => n.IsRead != true);
                var totalBackups = await _context.DataBackupLogs.CountAsync();
                var failedBackups = await _context.DataBackupLogs.CountAsync(b => b.Status != "Success");
                var totalRoles = await _context.AspNetRoles.CountAsync();
                var totalAuditLogs = await _context.AuditLogs.CountAsync();
                var today = System.DateTime.Today;
                var loginsToday = await _context.LoginHistories.CountAsync(h => h.LoginTime >= today);

                return Json(new
                {
                    totalUsers,
                    activeSessions,
                    unreadNotifications = unreadNotif,
                    totalBackups,
                    failedBackups,
                    totalRoles,
                    totalAuditLogs,
                    loginsToday
                });
            }
            catch
            {
                return Json(new { totalUsers = 0, activeSessions = 0, unreadNotifications = 0, totalBackups = 0, failedBackups = 0, totalRoles = 0, totalAuditLogs = 0, loginsToday = 0 });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentAudits()
        {
            try
            {
                var list = await _context.AuditLogs
                    .OrderByDescending(a => a.AuditId)
                    .Take(10)
                    .Select(a => new
                    {
                        auditId = a.AuditId,
                        userId = a.UserId,
                        username = a.User != null ? a.User.UserName : "System",
                        action = a.Action,
                        recordId = a.RecordId,
                        recordType = a.RecordType,
                        timestamp = a.Timestamp
                    })
                    .ToListAsync();
                return Json(list);
            }
            catch
            {
                return Json(new System.Collections.Generic.List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveSessionsList()
        {
            try
            {
                var now = System.DateTime.UtcNow;
                var list = await _context.UserSessions
                    .Where(s => s.IsActive == true)
                    .OrderByDescending(s => s.LoginTime)
                    .Take(10)
                    .Select(s => new
                    {
                        sessionId = s.SessionId,
                        userId = s.UserId,
                        username = s.User != null ? s.User.UserName : "-",
                        loginTime = s.LoginTime
                    })
                    .ToListAsync();

                var result = list.Select(s => new
                {
                    s.sessionId,
                    s.userId,
                    s.username,
                    s.loginTime,
                    durationMinutes = s.loginTime.HasValue ? (int)(now - s.loginTime.Value).TotalMinutes : 0
                }).ToList();

                return Json(result);
            }
            catch
            {
                return Json(new System.Collections.Generic.List<object>());
            }
        }
    }
}
