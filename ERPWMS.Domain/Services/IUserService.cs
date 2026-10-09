using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IUserService
    {
        // Authentication
        Task<User?> FindByUsernameOrEmailAsync(string login);
        Task<bool> ValidateCredentialsAsync(User user, string password);
        Task<bool> CanLoginAsync(User user);

        // User management
        Task<User> CreateUserAsync(string username, string email, string password, string firstName, string lastName,
            string? employeeCode = null, int? warehouseId = null, bool isActive = true);
        Task<User?> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync(bool includeDeleted = false);
        Task UpdateUserAsync(User user);
        Task<bool> ActivateUserAsync(int userId);
        Task<bool> DeactivateUserAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

        // Checks
        Task<bool> ExistsByUsernameAsync(string username, int? excludeUserId = null);
        Task<bool> ExistsByEmailAsync(string email, int? excludeUserId = null);

        // Roles
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);
        Task<bool> AssignRoleAsync(int userId, int roleId);
        Task<bool> RemoveRoleAsync(int userId, int roleId);
        Task<bool> IsInRoleAsync(int userId, string roleName);

        // Permissions (via ModulePermission)
        Task<bool> HasPermissionAsync(int userId, string moduleCode, string permissionType); // e.g., "View", "Create"
    }
}