using ERPWMS.Application.DTOs.Identity;
using ERPWMS.Domain.Services;
using ERPWMS.Infrastructure.Data;
using ERPWMS.Database.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Web.Controllers.Admin
{
    public class AccountController(
        IUserService userService,
        IRoleService roleService,
        AppDbContext context) : Controller
    {
        private readonly IUserService _userService = userService;
        private readonly IRoleService _roleService = roleService;
        private readonly AppDbContext _context = context;

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
                ModelState.AddModelError(string.Empty, "Your account is inactive or deleted.");
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
                ModelState.AddModelError(string.Empty, "No company is configured in the system.");
                return View(model);
            }

            // Login debug
            Console.WriteLine("");
            Console.WriteLine("============================================");
            Console.WriteLine("              LOGIN DEBUG");
            Console.WriteLine("============================================");
            Console.WriteLine($"USER ID       : {user.Id}");
            Console.WriteLine($"USERNAME      : {user.UserName}");
            Console.WriteLine($"EMAIL         : {user.Email}");
            Console.WriteLine($"ROLE COUNT    : {roles.Count}");
            foreach (var role in roles)
            {
                Console.WriteLine($"ROLE          : [{role}]");
            }
            Console.WriteLine($"IS ADMIN      : {isAdmin}");
            Console.WriteLine($"COMPANY ID    : {company.CompanyId}");
            Console.WriteLine($"COMPANY NAME  : {company.CompanyName}");
            Console.WriteLine("============================================");

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

            Console.WriteLine("");
            Console.WriteLine("============================================");
            Console.WriteLine("       FINAL AUTHENTICATION DEBUG");
            Console.WriteLine("============================================");
            Console.WriteLine($"Authenticated   : {principal.Identity?.IsAuthenticated}");
            Console.WriteLine($"Name            : {principal.Identity?.Name}");
            Console.WriteLine($"Is Admin        : {principal.IsInRole("Admin")}");
            Console.WriteLine($"CompanyId       : {company.CompanyId}");
            Console.WriteLine($"CompanyName     : {company.CompanyName}");
            foreach (var claim in principal.Claims)
            {
                Console.WriteLine($"CLAIM => {claim.Type} = {claim.Value}");
            }
            Console.WriteLine("============================================");

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
                Console.WriteLine($"ROLE DATABASE ERROR: {ex.Message}");
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
                Console.WriteLine($"PERMISSION ERROR: {ex.Message}");
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
    }
}