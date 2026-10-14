
using ERPWMS.Domain.Services;
using ERPWMS.Infrastructure.Data;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET ALL ROLES
        // =========================================================

        public async Task<IEnumerable<AspNetRole>> GetAllRolesAsync()
        {
            return await _context.AspNetRoles
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================================================
        // GET ROLE BY ID
        // =========================================================

        public async Task<AspNetRole?> GetRoleByIdAsync(int roleId)
        {
            return await _context.AspNetRoles
                .FirstOrDefaultAsync(x => x.Id == roleId);
        }

        // =========================================================
        // GET ROLE BY NAME
        // =========================================================

        public async Task<AspNetRole?> GetRoleByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return null;

            var normalizedName = roleName.Trim().ToUpperInvariant();

            return await _context.AspNetRoles
                .FirstOrDefaultAsync(x =>
                    x.NormalizedName == normalizedName);
        }

        // =========================================================
        // CREATE ROLE
        // =========================================================

        public async Task<AspNetRole> CreateRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException(
                    "Role name is required.",
                    nameof(roleName));

            roleName = roleName.Trim();

            var normalizedName =
                roleName.ToUpperInvariant();

            var exists = await _context.AspNetRoles
                .AnyAsync(x =>
                    x.NormalizedName == normalizedName);

            if (exists)
            {
                throw new InvalidOperationException(
                    "A role with this name already exists.");
            }

            var role = new AspNetRole
            {
                Name = roleName,
                NormalizedName = normalizedName,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            _context.AspNetRoles.Add(role);

            await _context.SaveChangesAsync();

            return role;
        }

        // =========================================================
        // UPDATE ROLE
        // =========================================================

        public async Task UpdateRoleAsync(AspNetRole role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (string.IsNullOrWhiteSpace(role.Name))
                throw new ArgumentException(
                    "Role name is required.",
                    nameof(role));

            role.Name = role.Name.Trim();
            role.NormalizedName =
                role.Name.ToUpperInvariant();

            _context.AspNetRoles.Update(role);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // DELETE ROLE
        // =========================================================

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var role = await _context.AspNetRoles
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.Id == roleId);

            if (role == null)
                return false;

            if (role.Users.Any())
            {
                throw new InvalidOperationException(
                    "Cannot delete role because it has assigned users.");
            }

            _context.AspNetRoles.Remove(role);

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // ROLE EXISTS
        // =========================================================

        public async Task<bool> RoleExistsAsync(
            string roleName,
            int? excludeRoleId = null)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;

            var normalizedName =
                roleName.Trim().ToUpperInvariant();

            var query = _context.AspNetRoles
                .Where(x =>
                    x.NormalizedName == normalizedName);

            if (excludeRoleId.HasValue)
            {
                query = query.Where(x =>
                    x.Id != excludeRoleId);
            }

            return await query.AnyAsync();
        }

        // =========================================================
        // GET USERS IN ROLE
        // =========================================================

        public async Task<IEnumerable<User>> GetUsersInRoleAsync(
            int roleId)
        {
            var role = await _context.AspNetRoles
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.Id == roleId);

            if (role == null)
                return Enumerable.Empty<User>();

            return role.Users
                .Where(x => x.IsDeleted != true)
                .ToList();
        }

        // =========================================================
        // CHECK USER IN ROLE
        // =========================================================

        public async Task<bool> IsUserInRoleAsync(
            int userId,
            int roleId)
        {
            return await _context.AspNetRoles
                .Where(role => role.Id == roleId)
                .SelectMany(role => role.Users)
                .AnyAsync(user =>
                    user.Id == userId &&
                    user.IsDeleted != true);
        }

        // =========================================================
        // ADD USER TO ROLE
        // =========================================================

        public async Task<bool> AddUserToRoleAsync(
            int userId,
            int roleId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == userId &&
                    x.IsDeleted != true);

            if (user == null)
                return false;

            var role = await _context.AspNetRoles
                .FirstOrDefaultAsync(x =>
                    x.Id == roleId);

            if (role == null)
                return false;

            if (role.Users.Any(x => x.Id == userId))
                return true;

            role.Users.Add(user);

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // REMOVE USER FROM ROLE
        // =========================================================

        public async Task<bool> RemoveUserFromRoleAsync(
            int userId,
            int roleId)
        {
            var role = await _context.AspNetRoles
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x =>
                    x.Id == roleId);

            if (role == null)
                return false;

            var user = role.Users
                .FirstOrDefault(x => x.Id == userId);

            if (user == null)
                return true;

            role.Users.Remove(user);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}

