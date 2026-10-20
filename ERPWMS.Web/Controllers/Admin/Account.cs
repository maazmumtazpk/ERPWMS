using ERPWMS.Application.DTOs.Identity;
using ERPWMS.Domain.Services;
using ERPWMS.Infrastructure.Data;
using ERPWMS.Database.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Web.Controllers.Admin
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly AppDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly ERPWMS.Application.Common.IEmailService _emailService;

        public AccountController(
            IUserService userService,
            IRoleService roleService,
            AppDbContext context,
            ILogger<AccountController> logger,
            ERPWMS.Application.Common.IEmailService emailService)
        {
            _userService = userService;
            _roleService = roleService;
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        // =========================================================
        // LOGIN - GET
        // =========================================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // =========================================================
        // LOGIN - POST
        // =========================================================

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginRequest model,
            string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.FindByUsernameOrEmailAsync(model.UsernameOrEmail);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            if (!await _userService.CanLoginAsync(user))
            {
                _logger.LogWarning("Login attempt on inactive account: {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            if (user.LockoutEnabled == true &&
                user.LockoutEnd.HasValue &&
                user.LockoutEnd.Value > DateTime.UtcNow)
            {
                var remainingMinutes = (int)Math.Ceiling(
                    (user.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes);

                _logger.LogWarning("Login attempt on locked account: {UserId}", user.Id);

                ModelState.AddModelError(string.Empty,
                    $"Account is locked. Please try again after {remainingMinutes} minutes.");
                return View(model);
            }

            var isValid = await _userService.ValidateCredentialsAsync(user, model.Password);

            if (!isValid)
            {
                await RecordLoginHistory(
                    user.Id,
                    "Failed",
                    Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers.UserAgent.ToString());

                _logger.LogWarning("Failed login attempt for user: {UserId}", user.Id);

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var roles = await GetUserRolesFromDatabaseAsync(user.Id);

            var isAdmin = roles.Any(r =>
                string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r, "Administrator", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r, "SuperAdmin", StringComparison.OrdinalIgnoreCase));

            var company = await _context.Companies
                .AsNoTracking()
                .Where(c => c.IsDeleted != true && c.IsActive == true)
                .OrderBy(c => c.CompanyId)
                .Select(c => new { c.CompanyId, c.CompanyName })
                .FirstOrDefaultAsync();

            if (company == null)
            {
                company = await _context.Companies
                    .AsNoTracking()
                    .Where(c => c.IsDeleted != true)
                    .OrderBy(c => c.CompanyId)
                    .Select(c => new { c.CompanyId, c.CompanyName })
                    .FirstOrDefaultAsync();
            }

            if (company == null || company.CompanyId <= 0)
            {
                _logger.LogError("No company configured in the system.");
                ModelState.AddModelError(string.Empty, "No company is configured in the system.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("FirstName", user.FirstName ?? string.Empty),
                new Claim("LastName", user.LastName ?? string.Empty),
                new Claim("CompanyId", company.CompanyId.ToString()),
                new Claim("CompanyName", company.CompanyName ?? string.Empty)
            };

            foreach (var role in roles)
            {
                if (string.IsNullOrWhiteSpace(role))
                    continue;

                var cleanRole = role.Trim();

                if (!claims.Any(c =>
                    c.Type == ClaimTypes.Role &&
                    string.Equals(c.Value, cleanRole, StringComparison.OrdinalIgnoreCase)))
                {
                    claims.Add(new Claim(ClaimTypes.Role, cleanRole));
                }
            }

            if (isAdmin)
            {
                var adminClaimExists = claims.Any(c =>
                    c.Type == ClaimTypes.Role &&
                    string.Equals(c.Value, "Admin", StringComparison.OrdinalIgnoreCase));

                if (!adminClaimExists)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                }
            }

            var permissions = await GetUserPermissionsAsync(user.Id);

            foreach (var permission in permissions)
            {
                AddPermissionClaim(claims, permission);
            }

            if (isAdmin)
            {
                var adminPermissions = GetAdminPermissions();
                foreach (var permission in adminPermissions)
                {
                    AddPermissionClaim(claims, permission);
                }
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name,
                ClaimTypes.Role);

            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(model.RememberMe ? 10080 : 60),
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            user.LastLoginDate = DateTime.UtcNow;
            await _userService.UpdateUserAsync(user);

            await RecordLoginHistory(
                user.Id,
                "Success",
                Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString());

            await CreateUserSession(
                user.Id,
                Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString());

            _logger.LogInformation("User {UserId} logged in successfully.", user.Id);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        // =========================================================
        // LOGOUT
        // =========================================================

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdString, out var userId))
            {
                var session = await _context.UserSessions
                    .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive == true);

                if (session != null)
                {
                    session.IsActive = false;
                    session.LogoutTime = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                var history = await _context.LoginHistories
                    .Where(h => h.UserId == userId && h.LogoutTime == null)
                    .OrderByDescending(h => h.LoginTime)
                    .FirstOrDefaultAsync();

                if (history != null)
                {
                    history.LogoutTime = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("User {UserId} logged out.", userId);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }

        // =========================================================
        // ACCESS DENIED
        // =========================================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // =========================================================
        // GET USER ROLES
        // =========================================================

        private async Task<List<string>> GetUserRolesFromDatabaseAsync(int userId)
        {
            try
            {
                var roles = await _context.AspNetRoles
                    .Where(role => role.Users.Any(user => user.Id == userId))
                    .Select(role => role.Name)
                    .Where(name => name != null && name != "")
                    .ToListAsync();

                return roles
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Select(name => name!.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching roles for user {UserId}", userId);
                return new List<string>();
            }
        }

        // =========================================================
        // GET USER PERMISSIONS
        // =========================================================

        private async Task<List<string>> GetUserPermissionsAsync(int userId)
        {
            try
            {
                var roleIds = await _context.AspNetRoles
                    .Where(role => role.Users.Any(user => user.Id == userId))
                    .Select(role => role.Id)
                    .ToListAsync();

                if (roleIds.Count == 0)
                    return new List<string>();

                var permissions = await _context.ModulePermissions
                    .Include(x => x.Module)
                    .Where(x =>
                        roleIds.Contains(x.RoleId) &&
                        x.Module != null &&
                        x.Module.IsActive == true)
                    .ToListAsync();

                var result = new List<string>();

                foreach (var permission in permissions)
                {
                    if (permission.Module == null)
                        continue;

                    var moduleCode = permission.Module.ModuleCode;

                    if (string.IsNullOrWhiteSpace(moduleCode))
                        continue;

                    moduleCode = moduleCode.Trim();

                    if (permission.CanView == true)
                        result.Add($"{moduleCode}View");
                    if (permission.CanCreate == true)
                        result.Add($"{moduleCode}Create");
                    if (permission.CanEdit == true)
                        result.Add($"{moduleCode}Update");
                    if (permission.CanDelete == true)
                        result.Add($"{moduleCode}Delete");
                    if (permission.CanApprove == true)
                        result.Add($"{moduleCode}Approve");
                }

                return result
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching permissions for user {UserId}", userId);
                return new List<string>();
            }
        }

        // =========================================================
        // ADD PERMISSION CLAIM
        // =========================================================

        private static void AddPermissionClaim(List<Claim> claims, string permission)
        {
            if (string.IsNullOrWhiteSpace(permission))
                return;

            var exists = claims.Any(c =>
                c.Type == "Permission" &&
                string.Equals(c.Value, permission, StringComparison.OrdinalIgnoreCase));

            if (!exists)
            {
                claims.Add(new Claim("Permission", permission));
            }
        }

        // =========================================================
        // ADMIN FULL PERMISSIONS
        // =========================================================

        private static List<string> GetAdminPermissions()
        {
            return new List<string>
            {
                "CompanyView", "CompanyCreate", "CompanyUpdate", "CompanyDelete",
                "BranchView", "BranchCreate", "BranchUpdate", "BranchDelete",
                "DepartmentView", "DepartmentCreate", "DepartmentUpdate", "DepartmentDelete",
                "DesignationView", "DesignationCreate", "DesignationUpdate", "DesignationDelete",
                "SystemAdmin.View", "SystemAdmin.Manage",
                "UserView", "UserCreate", "UserUpdate", "UserDelete",
                "RoleView", "RoleCreate", "RoleUpdate", "RoleDelete",
                "WarehouseView", "WarehouseCreate", "WarehouseUpdate", "WarehouseDelete",
                "WarehouseActivate", "WarehouseDeactivate",
                "ZoneView", "ZoneCreate", "ZoneUpdate", "ZoneDelete",
                "RackView", "RackCreate", "RackUpdate", "RackDelete",
                "BinView", "BinCreate", "BinUpdate", "BinDelete",
                "EmployeeView", "EmployeeCreate", "EmployeeUpdate", "EmployeeDelete",
                "ChartOfAccountView", "ChartOfAccountCreate", "ChartOfAccountUpdate", "ChartOfAccountDelete",
                "BankAccountView", "BankAccountCreate", "BankAccountUpdate", "BankAccountDelete",
                "TaxSettingView", "TaxSettingCreate", "TaxSettingUpdate", "TaxSettingDelete",
                "ShiftView", "ShiftCreate", "ShiftUpdate", "ShiftDelete",
                "DashboardView"
            };
        }

        // =========================================================
        // RECORD LOGIN HISTORY
        // =========================================================

        private async Task RecordLoginHistory(int userId, string status, string? ipAddress, string? userAgent)
        {
            var history = new LoginHistory
            {
                UserId = userId,
                LoginTime = DateTime.UtcNow,
                IPAddress = ipAddress,
                DeviceInfo = userAgent,
                Status = status
            };

            _context.LoginHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // CREATE USER SESSION
        // =========================================================

        private async Task CreateUserSession(int userId, string? ipAddress, string? userAgent)
        {
            var session = new UserSession
            {
                UserId = userId,
                SessionToken = Guid.NewGuid().ToString(),
                LoginTime = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow,
                IPAddress = ipAddress,
                DeviceInfo = userAgent,
                IsActive = true
            };

            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();
        }
    

        // ============================================================
        // FORGOT PASSWORD FLOW
        // ============================================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string emailOrUsername)
        {
            if (string.IsNullOrWhiteSpace(emailOrUsername))
            {
                ModelState.AddModelError("", "Please enter your email or username.");
                return View();
            }

            var result = await _userService.GeneratePasswordResetTokenAsync(emailOrUsername);

            if (string.IsNullOrEmpty(result.Token))
            {
                ModelState.AddModelError("", "If this email exists, an OTP has been sent.");
                return View();
            }

            // 1. ASAL EMAIL BHEJO
            var emailSent = await _emailService.SendEmailAsync(result.Email, "ERPWMS Password Reset OTP", result.Token);

            if (!emailSent && !_emailService.IsTestMode)
            {
                ModelState.AddModelError("", "Email send failed. Check internet or App Password.");
                return View();
            }

            TempData["ResetEmail"] = emailOrUsername;
            TempData["SuccessMessage"] = $"An OTP has been sent to {result.Email}";

            // 2. SIRF TEST MODE MEIN SCREEN PAR DIKHAO
            if (_emailService.IsTestMode)
            {
                TempData["TestOtp"] = result.Token;
            }

            return RedirectToAction(nameof(VerifyOtp));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyOtp()
        {
            if (TempData["ResetEmail"] == null) return RedirectToAction(nameof(ForgotPassword));
            TempData.Keep("ResetEmail");
            TempData.Keep("TestOtp");
            TempData.Keep("SuccessMessage");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(string otp)
        {
            if (string.IsNullOrWhiteSpace(otp))
            {
                ModelState.AddModelError("", "Please enter the OTP.");
                return View();
            }

            var userId = await _userService.ValidatePasswordResetTokenAsync(otp);
            if (userId == null)
            {
                ModelState.AddModelError("", "Invalid or expired OTP. Please request a new one.");
                return View();
            }

            TempData["ResetUserId"] = userId.Value;
            return RedirectToAction(nameof(ResetPassword));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            if (TempData["ResetUserId"] == null) return RedirectToAction(nameof(ForgotPassword));
            TempData.Keep("ResetUserId");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string newPassword, string confirmPassword)
        {
            if (TempData["ResetUserId"] == null) return RedirectToAction(nameof(ForgotPassword));

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                ModelState.AddModelError("", "Password must be at least 6 characters.");
                TempData.Keep("ResetUserId");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                TempData.Keep("ResetUserId");
                return View();
            }

            var userId = (int)TempData["ResetUserId"]!;
            var success = await _userService.ResetPasswordAsync(userId, newPassword);

            if (success)
            {
                TempData["SuccessMessage"] = "Password reset successful! Please login with your new password.";
                return RedirectToAction(nameof(Login));
            }

            ModelState.AddModelError("", "Password reset failed. Please try again.");
            return View();
        }}
}




