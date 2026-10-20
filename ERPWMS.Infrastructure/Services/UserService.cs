using ERPWMS.Domain.Services;
using ERPWMS.Infrastructure.Data;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // AUTHENTICATION
        // =========================================================

        public async Task<User?> FindByUsernameOrEmailAsync(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                return null;

            var normalizedLogin = login.Trim().ToUpperInvariant();

            return await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.IsDeleted != true &&
                    (
                        x.NormalizedUserName == normalizedLogin ||
                        x.NormalizedEmail == normalizedLogin
                    ));
        }

        public async Task<bool> ValidateCredentialsAsync(
            User user,
            string password)
        {
            if (user == null ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                return false;
            }

            // =====================================================
            // PLAIN TEXT PASSWORD COMPARISON
            // =====================================================

            bool isValid;
            try
            {
                isValid = (user.PasswordHash == password) || BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }
            catch
            {
                isValid = false; // Legacy plain-text fallback removed
                if (isValid)
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                }
            }
            return isValid;
        }

        public async Task<bool> CanLoginAsync(User user)
        {
            if (user == null)
                return false;

            if (user.IsDeleted == true)
                return false;

            if (user.IsActive != true)
                return false;

            return true;
        }

        // =========================================================
        // USER MANAGEMENT
        // =========================================================

        public async Task<User> CreateUserAsync(
            string username,
            string email,
            string password,
            string firstName,
            string lastName,
            string? employeeCode = null,
            int? warehouseId = null,
            bool isActive = true)
        {
            username = username.Trim();
            email = email.Trim();

            if (await ExistsByUsernameAsync(username))
                throw new InvalidOperationException(
                    "Username already exists.");

            if (await ExistsByEmailAsync(email))
                throw new InvalidOperationException(
                    "Email already exists.");

            var user = new User
            {
                UserName = username,

                NormalizedUserName =
                    username.ToUpperInvariant(),

                Email = email,

                NormalizedEmail =
                    email.ToUpperInvariant(),

                EmailConfirmed = false,

                // =================================================
                // PLAIN PASSWORD
                // =================================================
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),

                SecurityStamp =
                    Guid.NewGuid().ToString(),

                ConcurrencyStamp =
                    Guid.NewGuid().ToString(),

                PhoneNumberConfirmed = false,

                TwoFactorEnabled = false,

                LockoutEnabled = true,

                AccessFailedCount = 0,

                FirstName = firstName.Trim(),

                LastName = lastName.Trim(),

                EmployeeCode =
                    string.IsNullOrWhiteSpace(employeeCode)
                        ? null
                        : employeeCode.Trim(),

                WarehouseId = warehouseId,

                IsActive = isActive,

                IsDeleted = false,

                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == userId &&
                    x.IsDeleted != true);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(
            bool includeDeleted = false)
        {
            IQueryable<User> query = _context.Users;

            if (!includeDeleted)
            {
                query = query.Where(x =>
                    x.IsDeleted != true);
            }

            return await query
                .OrderBy(x => x.UserName)
                .ToListAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser =
                await _context.Users
                    .FirstOrDefaultAsync(x =>
                        x.Id == user.Id);

            if (existingUser == null)
                throw new InvalidOperationException(
                    "User not found.");

            existingUser.UserName =
                user.UserName;

            existingUser.NormalizedUserName =
                user.UserName.ToUpperInvariant();

            existingUser.Email =
                user.Email;

            existingUser.NormalizedEmail =
                user.Email.ToUpperInvariant();

            existingUser.FirstName =
                user.FirstName;

            existingUser.LastName =
                user.LastName;

            existingUser.EmployeeCode =
                user.EmployeeCode;

            existingUser.WarehouseId =
                user.WarehouseId;

            existingUser.IsActive =
                user.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ActivateUserAsync(int userId)
        {
            var user =
                await _context.Users
                    .FirstOrDefaultAsync(x =>
                        x.Id == userId &&
                        x.IsDeleted != true);

            if (user == null)
                return false;

            user.IsActive = true;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateUserAsync(int userId)
        {
            var user =
                await _context.Users
                    .FirstOrDefaultAsync(x =>
                        x.Id == userId &&
                        x.IsDeleted != true);

            if (user == null)
                return false;

            user.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // CHANGE PASSWORD
        // =========================================================

        public async Task<bool> ChangePasswordAsync(
            int userId,
            string currentPassword,
            string newPassword)
        {
            var user =
                await _context.Users
                    .FirstOrDefaultAsync(x =>
                        x.Id == userId &&
                        x.IsDeleted != true);

            if (user == null)
                return false;

           // Check current password with BCrypt
bool currentIsValid;
try
{
    currentIsValid = BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash);
}
catch
{
    currentIsValid = user.PasswordHash == currentPassword;
}
if (!currentIsValid)
    return false;

            // Save new plain password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            user.SecurityStamp =
                Guid.NewGuid().ToString();

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // CHECKS
        // =========================================================

        public async Task<bool> ExistsByUsernameAsync(
            string username,
            int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            var normalizedUsername =
                username.Trim().ToUpperInvariant();

            return await _context.Users
                .AnyAsync(x =>
                    x.IsDeleted != true &&
                    x.NormalizedUserName == normalizedUsername &&
                    (
                        !excludeUserId.HasValue ||
                        x.Id != excludeUserId
                    ));
        }

        public async Task<bool> ExistsByEmailAsync(
            string email,
            int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var normalizedEmail =
                email.Trim().ToUpperInvariant();

            return await _context.Users
                .AnyAsync(x =>
                    x.IsDeleted != true &&
                    x.NormalizedEmail == normalizedEmail &&
                    (
                        !excludeUserId.HasValue ||
                        x.Id != excludeUserId
                    ));
        }

        // =========================================================
        // ROLES
        // =========================================================

        public async Task<IEnumerable<string>> GetUserRolesAsync(
            int userId)
        {
            var user =
                await _context.Users
                    .Include(x => x.Roles)
                    .FirstOrDefaultAsync(x =>
                        x.Id == userId);

            if (user == null)
                return Enumerable.Empty<string>();

            return user.Roles
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .Select(x => x.Name)
                .ToList();
        }

        public async Task<bool> AssignRoleAsync(
            int userId,
            int roleId)
        {
            var user =
                await _context.Users
                    .Include(x => x.Roles)
                    .FirstOrDefaultAsync(x =>
                        x.Id == userId &&
                        x.IsDeleted != true);

            if (user == null)
                return false;

            var role =
                await _context.AspNetRoles
                    .FirstOrDefaultAsync(x =>
                        x.Id == roleId);

            if (role == null)
                return false;

            if (!user.Roles.Any(x => x.Id == roleId))
            {
                user.Roles.Add(role);

                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> RemoveRoleAsync(
            int userId,
            int roleId)
        {
            var user =
                await _context.Users
                    .Include(x => x.Roles)
                    .FirstOrDefaultAsync(x =>
                        x.Id == userId);

            if (user == null)
                return false;

            var role =
                user.Roles
                    .FirstOrDefault(x =>
                        x.Id == roleId);

            if (role == null)
                return false;

            user.Roles.Remove(role);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsInRoleAsync(
            int userId,
            string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;

            var normalizedRoleName =
                roleName.Trim().ToUpperInvariant();

            return await _context.Users
                .Where(x =>
                    x.Id == userId &&
                    x.IsDeleted != true)
                .SelectMany(x => x.Roles)
                .AnyAsync(x =>
                    x.NormalizedName == normalizedRoleName);
        }

        // =========================================================
        // PERMISSIONS
        // =========================================================

        public async Task<bool> HasPermissionAsync(
            int userId,
            string moduleCode,
            string permissionType)
        {
            if (string.IsNullOrWhiteSpace(moduleCode) ||
                string.IsNullOrWhiteSpace(permissionType))
            {
                return false;
            }

            var normalizedModuleCode =
                moduleCode.Trim().ToUpperInvariant();

            var normalizedPermission =
                permissionType.Trim().ToLowerInvariant();

            var roleIds =
                await _context.Users
                    .Where(x =>
                        x.Id == userId &&
                        x.IsDeleted != true &&
                        x.IsActive == true)
                    .SelectMany(x => x.Roles)
                    .Select(x => x.Id)
                    .ToListAsync();

            if (!roleIds.Any())
                return false;

            var permissions =
                await _context.ModulePermissions
                    .Include(x => x.Module)
                    .Where(x =>
                        roleIds.Contains(x.RoleId) &&
                        x.Module.IsActive == true &&
                        x.Module.ModuleCode.ToUpper() ==
                            normalizedModuleCode)
                    .ToListAsync();

            foreach (var permission in permissions)
            {
                var allowed =
                    normalizedPermission switch
                    {
                        "view" =>
                            permission.CanView == true,

                        "create" =>
                            permission.CanCreate == true,

                        "edit" =>
                            permission.CanEdit == true,

                        "delete" =>
                            permission.CanDelete == true,

                        "approve" =>
                            permission.CanApprove == true,

                        _ => false
                    };

                if (allowed)
                    return true;
            }

            return false;
        }
    

        // ============================================================
        // PASSWORD RESET IMPLEMENTATION
        // ============================================================
        
        public async Task<(string Token, string Email)> GeneratePasswordResetTokenAsync(string emailOrUsername)
        {
            if (string.IsNullOrWhiteSpace(emailOrUsername))
                return (null!, null!);

            var normalizedInput = emailOrUsername.Trim().ToUpperInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.IsDeleted != true &&
                    (x.NormalizedEmail == normalizedInput || x.NormalizedUserName == normalizedInput));

            if (user == null)
                return (null!, null!);

            // Generate 6-digit OTP
            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();

            // Invalidate old tokens for this user
            var oldTokens = await _context.PasswordResetTokens
                .Where(t => t.UserId == user.Id && t.IsUsed == false)
                .ToListAsync();
            foreach (var old in oldTokens) { old.IsUsed = true; }

            // Create new token
            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = otp,
                ExpiryDate = DateTime.UtcNow.AddMinutes(10),  // 10 min validity
                IsUsed = false,
                CreatedDate = DateTime.UtcNow
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            return (otp, user.Email ?? "");
        }

        public async Task<int?> ValidatePasswordResetTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var resetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t =>
                    t.Token == token &&
                    t.IsUsed == false &&
                    t.ExpiryDate > DateTime.UtcNow);

            if (resetToken == null)
                return null;

            return resetToken.UserId;
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                return false;

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId && x.IsDeleted != true);

            if (user == null)
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.SecurityStamp = Guid.NewGuid().ToString();

            // Mark all tokens as used
            var tokens = await _context.PasswordResetTokens
                .Where(t => t.UserId == userId && t.IsUsed == false)
                .ToListAsync();
            foreach (var t in tokens) { t.IsUsed = true; }

            await _context.SaveChangesAsync();
            return true;
        }
    }}

