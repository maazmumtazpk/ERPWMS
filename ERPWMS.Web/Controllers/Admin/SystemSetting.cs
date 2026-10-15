using ERPWMS.Application.DTOs.System;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Web.Controllers.Admin
{
    [Authorize(Policy = "SystemAdmin.View")]
    public class SystemSettingController : Controller
    {
        private readonly ISystemSettingService _settingService;
        private readonly IUserService _userService;

        public SystemSettingController(ISystemSettingService settingService, IUserService userService)
        {
            _settingService = settingService;
            _userService = userService;
        }

        // -------------------------------------------------------
        // INDEX
        // -------------------------------------------------------
        public async Task<IActionResult> Index(string? searchTerm)
        {
            var settings = await _settingService.GetAllSettingsAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                settings = settings.Where(s =>
                    s.SettingKey.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (s.Description != null && s.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            return View(settings);
        }

        // -------------------------------------------------------
        // EDIT - GET
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var setting = await _settingService.GetSettingByIdAsync(id);
            if (setting == null) return NotFound();

            var model = new UpdateSystemSettingRequest
            {
                SettingId = setting.SettingId,
                SettingKey = setting.SettingKey,
                SettingValue = setting.IsEncrypted == true ? "" : setting.SettingValue,
                DataType = setting.DataType,
                IsEncrypted = setting.IsEncrypted,
                Description = setting.Description,
                UpdatedBy = 0
            };

            return View(model);
        }

        // -------------------------------------------------------
        // EDIT - POST
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Edit(UpdateSystemSettingRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            if (request.UpdatedBy == 0)
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                    request.UpdatedBy = userId;
            }

            var oldSetting = await _settingService.GetSettingByIdAsync(request.SettingId);
            if (oldSetting == null) return NotFound();

            var oldValue = oldSetting.IsEncrypted == true ? "********" : oldSetting.SettingValue;

            oldSetting.SettingKey = request.SettingKey;
            oldSetting.SettingValue = request.IsEncrypted == true
                ? Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.SettingValue ?? ""))
                : request.SettingValue;
            oldSetting.DataType = request.DataType;
            oldSetting.IsEncrypted = request.IsEncrypted;
            oldSetting.Description = request.Description;
            oldSetting.UpdatedBy = request.UpdatedBy.ToString();   // âœ… UpdatedBy: int â†’ string
            oldSetting.UpdatedDate = DateTime.Now;

            await _settingService.UpdateSettingAsync(oldSetting);

            await _settingService.LogSettingChangeAsync(
                request.SettingId,
                oldValue,
                request.SettingValue,
                request.UpdatedBy                                  // âœ… int (no .ToString())
            );

            TempData["Success"] = "Setting updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // -------------------------------------------------------
        // DELETE
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            var setting = await _settingService.GetSettingByIdAsync(id);
            if (setting == null)
            {
                TempData["Error"] = "Setting not found.";
                return RedirectToAction(nameof(Index));
            }

            var criticalKeys = new[] { "SystemName", "DefaultCurrency", "MaintenanceMode" };
            if (criticalKeys.Contains(setting.SettingKey))
            {
                TempData["Error"] = "Cannot delete critical system settings.";
                return RedirectToAction(nameof(Index));
            }

            await _settingService.DeleteSettingAsync(id);
            TempData["Success"] = "Setting deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // -------------------------------------------------------
        // AJAX: ADD/EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult AddPartial() => PartialView("_AddForm");

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var s = await _settingService.GetSettingByIdAsync(id);
            if (s == null) return NotFound();
            return PartialView("_EditForm", s);
        }

        // -------------------------------------------------------
        // AJAX: STATISTICS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var settings = (await _settingService.GetAllSettingsAsync())?.ToList()
                ?? new List<SystemSetting>();
            var now = DateTime.Now;

            return Json(new
            {
                total = settings.Count,
                active = settings.Count(s => s.IsEncrypted != true),
                inactive = settings.Count(s => s.IsEncrypted == true),
                addedThisMonth = settings.Count(s =>
                    s.UpdatedDate.HasValue &&
                    s.UpdatedDate.Value.Month == now.Month &&
                    s.UpdatedDate.Value.Year == now.Year)
            });
        }

        // -------------------------------------------------------
        // AJAX: GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var settings = (await _settingService.GetAllSettingsAsync())?.ToList()
                ?? new List<SystemSetting>();

            var list = settings.Select(s => new
            {
                settingId = s.SettingId,
                settingKey = s.SettingKey,
                settingValue = s.IsEncrypted == true ? "********" : s.SettingValue,
                dataType = s.DataType,
                isEncrypted = s.IsEncrypted == true,
                description = s.Description
            }).ToList();

            return Json(list);
        }

        // -------------------------------------------------------
        // AJAX: QUICK EDIT
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> QuickEdit(int settingId, string settingKey, string settingValue,
            string dataType, string description, bool isEncrypted = false)
        {
            try
            {
                var s = await _settingService.GetSettingByIdAsync(settingId);
                if (s == null) return Json(new { success = false, message = "Not found" });

                s.SettingKey = settingKey;
                s.SettingValue = isEncrypted
                    ? Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(settingValue ?? ""))
                    : settingValue;
                s.DataType = dataType;
                s.IsEncrypted = isEncrypted;
                s.Description = description;
                s.UpdatedDate = DateTime.Now;

                await _settingService.UpdateSettingAsync(s);
                return Json(new { success = true, message = "Setting updated" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // -------------------------------------------------------
        // AJAX: QUICK DELETE
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> QuickDelete(int id)
        {
            try
            {
                await _settingService.DeleteSettingAsync(id);
                return Json(new { success = true, message = "Setting deleted" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }
    }
}
