using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Web.Controllers.HRMaster
{
    [Authorize]
    public class PayrollSettingController : Controller
    {
        private readonly AppDbContext _context;

        public PayrollSettingController(AppDbContext c)
        {
            _context = c;
        }

        // Returns current user's username (NOT ID)
        private string CurrentUserName()
        {
            // 1) Primary: Identity.Name (from cookie auth)
            var name = HttpContext?.User?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(name))
                return name.Trim();

            // 2) ClaimTypes.Name
            var claimName = HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            if (!string.IsNullOrWhiteSpace(claimName))
                return claimName.Trim();

            // 3) NameIdentifier → lookup User by Id
            var idClaim = HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrWhiteSpace(idClaim) && int.TryParse(idClaim, out var uid))
            {
                var u = _context.Users.FirstOrDefault(x => x.Id == uid);
                if (u != null && !string.IsNullOrWhiteSpace(u.UserName))
                    return u.UserName;
            }

            // 4) Legacy: UserName claim (custom claim, if any)
            var unameClaim = HttpContext?.User?.FindFirst("UserName")?.Value;
            if (!string.IsNullOrWhiteSpace(unameClaim))
                return unameClaim.Trim();

            // 5) Fallback: first user
            var first = _context.Users.FirstOrDefault();
            return first?.UserName ?? "System";
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetSetting()
        {
            var s = await _context.PayrollSettings
                .OrderByDescending(p => p.SettingId)
                .FirstOrDefaultAsync();

            if (s == null)
            {
                return Json(new
                {
                    exists = false,
                    settingId = 0,
                    frequency = "Monthly",
                    salaryDivisor = 30,
                    workingDays = 26,
                    gracePeriodMinutes = 15,
                    lateThresholdMinutes = 30,
                    earlyThresholdMinutes = 30,
                    otMultiplier = 1.5,
                    roundingRule = "Nearest",
                    currency = "PKR",
                    approvalRequired = true,
                    isActive = true,
                    lateWarningCount1 = 3,
                    lateWarningCount2 = 5,
                    lateHalfDayCount = 3,
                    lateFullDayCount = 6,
                    earlyWarningCount = 3,
                    earlyHalfDayCount = 5,
                    enableLateDeduction = true,
                    enableEarlyDeduction = true,
                    autoCalculatePayroll = false,
                    lockAfterApproval = false,
                    allowManualAdjustments = true,
                    sendPayrollNotifications = false,
                    updatedBy = (string)null,
                    updatedDate = (string)null
                });
            }

            return Json(new
            {
                exists = true,
                settingId = s.SettingId,
                frequency = s.Frequency,
                salaryDivisor = s.SalaryDivisor,
                workingDays = s.WorkingDays,
                gracePeriodMinutes = s.GracePeriodMinutes,
                lateThresholdMinutes = s.LateThresholdMinutes,
                earlyThresholdMinutes = s.EarlyThresholdMinutes,
                otMultiplier = s.OTMultiplier,
                roundingRule = s.RoundingRule,
                currency = s.Currency,
                approvalRequired = s.ApprovalRequired,
                isActive = s.IsActive,
                lateWarningCount1 = s.LateWarningCount1,
                lateWarningCount2 = s.LateWarningCount2,
                lateHalfDayCount = s.LateHalfDayCount,
                lateFullDayCount = s.LateFullDayCount,
                earlyWarningCount = s.EarlyWarningCount,
                earlyHalfDayCount = s.EarlyHalfDayCount,
                enableLateDeduction = s.EnableLateDeduction,
                enableEarlyDeduction = s.EnableEarlyDeduction,
                autoCalculatePayroll = s.AutoCalculatePayroll,
                lockAfterApproval = s.LockAfterApproval,
                allowManualAdjustments = s.AllowManualAdjustments,
                sendPayrollNotifications = s.SendPayrollNotifications,
                updatedBy = s.UpdatedBy,
                updatedDate = s.UpdatedDate.HasValue ? s.UpdatedDate.Value.ToString("dd MMM yyyy HH:mm") : null
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(
            string frequency,
            int salaryDivisor,
            int workingDays,
            int gracePeriodMinutes,
            int lateThresholdMinutes,
            int earlyThresholdMinutes,
            decimal otMultiplier,
            string roundingRule,
            string currency,
            bool approvalRequired,
            bool isActive,
            int lateWarningCount1,
            int lateWarningCount2,
            int lateHalfDayCount,
            int lateFullDayCount,
            int earlyWarningCount,
            int earlyHalfDayCount,
            bool enableLateDeduction,
            bool enableEarlyDeduction,
            bool autoCalculatePayroll,
            bool lockAfterApproval,
            bool allowManualAdjustments,
            bool sendPayrollNotifications)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(frequency))
                    return Json(new { success = false, message = "Frequency required." });
                if (salaryDivisor < 1 || salaryDivisor > 31)
                    return Json(new { success = false, message = "Salary divisor must be 1-31." });
                if (workingDays < 1 || workingDays > 31)
                    return Json(new { success = false, message = "Working days must be 1-31." });
                if (otMultiplier < 0.5m || otMultiplier > 5)
                    return Json(new { success = false, message = "OT multiplier must be 0.5-5." });
                if (string.IsNullOrWhiteSpace(currency))
                    return Json(new { success = false, message = "Currency required." });

                var userName = CurrentUserName();

                var s = await _context.PayrollSettings
                    .OrderByDescending(p => p.SettingId)
                    .FirstOrDefaultAsync();

                if (s == null)
                {
                    s = new PayrollSetting
                    {
                        CreatedBy = userName,
                        CreatedDate = DateTime.Now
                    };
                    _context.PayrollSettings.Add(s);
                }

                s.Frequency = frequency.Trim();
                s.SalaryDivisor = salaryDivisor;
                s.WorkingDays = workingDays;
                s.GracePeriodMinutes = gracePeriodMinutes;
                s.LateThresholdMinutes = lateThresholdMinutes;
                s.EarlyThresholdMinutes = earlyThresholdMinutes;
                s.OTMultiplier = otMultiplier;
                s.RoundingRule = roundingRule ?? "Nearest";
                s.Currency = currency.Trim().ToUpper();
                s.ApprovalRequired = approvalRequired;
                s.IsActive = isActive;
                s.LateWarningCount1 = lateWarningCount1;
                s.LateWarningCount2 = lateWarningCount2;
                s.LateHalfDayCount = lateHalfDayCount;
                s.LateFullDayCount = lateFullDayCount;
                s.EarlyWarningCount = earlyWarningCount;
                s.EarlyHalfDayCount = earlyHalfDayCount;
                s.EnableLateDeduction = enableLateDeduction;
                s.EnableEarlyDeduction = enableEarlyDeduction;
                s.AutoCalculatePayroll = autoCalculatePayroll;
                s.LockAfterApproval = lockAfterApproval;
                s.AllowManualAdjustments = allowManualAdjustments;
                s.SendPayrollNotifications = sendPayrollNotifications;
                s.UpdatedBy = userName;
                s.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Payroll settings saved!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetToDefaults()
        {
            try
            {
                var userName = CurrentUserName();
                var s = await _context.PayrollSettings
                    .OrderByDescending(p => p.SettingId)
                    .FirstOrDefaultAsync();

                if (s == null)
                    return Json(new { success = false, message = "No setting found." });

                s.Frequency = "Monthly";
                s.SalaryDivisor = 30;
                s.WorkingDays = 26;
                s.GracePeriodMinutes = 15;
                s.LateThresholdMinutes = 30;
                s.EarlyThresholdMinutes = 30;
                s.OTMultiplier = 1.5m;
                s.RoundingRule = "Nearest";
                s.Currency = "PKR";
                s.ApprovalRequired = true;
                s.IsActive = true;
                s.LateWarningCount1 = 3;
                s.LateWarningCount2 = 5;
                s.LateHalfDayCount = 3;
                s.LateFullDayCount = 6;
                s.EarlyWarningCount = 3;
                s.EarlyHalfDayCount = 5;
                s.EnableLateDeduction = true;
                s.EnableEarlyDeduction = true;
                s.AutoCalculatePayroll = false;
                s.LockAfterApproval = false;
                s.AllowManualAdjustments = true;
                s.SendPayrollNotifications = false;
                s.UpdatedBy = userName;
                s.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Reset to defaults!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}


