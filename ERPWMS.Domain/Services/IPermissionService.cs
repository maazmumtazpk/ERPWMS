using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IPermissionService
    {
        // Module permissions
        Task<IEnumerable<ModulePermission>> GetRolePermissionsAsync(int roleId);
        Task<bool> AssignPermissionAsync(int roleId, int moduleId, bool canView, bool canCreate, bool canEdit, bool canDelete, bool canApprove);
        Task<bool> UpdatePermissionAsync(ModulePermission permission);
        Task<bool> RemovePermissionAsync(int permissionId);

        // Module management
        Task<IEnumerable<Module>> GetAllModulesAsync();
        Task<Module?> GetModuleByCodeAsync(string moduleCode);
    }
}