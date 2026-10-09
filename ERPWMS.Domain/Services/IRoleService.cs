
using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IRoleService
    {
        // =========================================================
        // ROLE MANAGEMENT
        // =========================================================

        Task<IEnumerable<AspNetRole>> GetAllRolesAsync();

        Task<AspNetRole?> GetRoleByIdAsync(int roleId);

        Task<AspNetRole?> GetRoleByNameAsync(string roleName);

        Task<AspNetRole> CreateRoleAsync(string roleName);

        Task UpdateRoleAsync(AspNetRole role);

        Task<bool> DeleteRoleAsync(int roleId);

        Task<bool> RoleExistsAsync(
            string roleName,
            int? excludeRoleId = null);


        // =========================================================
        // USER - ROLE ASSIGNMENT
        // =========================================================

        Task<IEnumerable<User>> GetUsersInRoleAsync(
            int roleId);

        Task<bool> IsUserInRoleAsync(
            int userId,
            int roleId);

        Task<bool> AddUserToRoleAsync(
            int userId,
            int roleId);

        Task<bool> RemoveUserFromRoleAsync(
            int userId,
            int roleId);
    }
}

