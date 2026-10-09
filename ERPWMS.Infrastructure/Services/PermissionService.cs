using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Services.Identity
{
    // ✅ Primary Constructor
    public class PermissionService(AppDbContext context) : IPermissionService
    {
        public async Task<IEnumerable<ModulePermission>> GetRolePermissionsAsync(int roleId)
        {
            return await context.ModulePermissions
                .Include(p => p.Module)
                .Where(p => p.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<bool> AssignPermissionAsync(
            int roleId,
            int moduleId,
            bool canView,
            bool canCreate,
            bool canEdit,
            bool canDelete,
            bool canApprove)
        {
            var existing = await context.ModulePermissions
                .FirstOrDefaultAsync(p => p.RoleId == roleId && p.ModuleId == moduleId);

            if (existing is not null)   // ✅ simplified
            {
                existing.CanView = canView;
                existing.CanCreate = canCreate;
                existing.CanEdit = canEdit;
                existing.CanDelete = canDelete;
                existing.CanApprove = canApprove;
                existing.UpdatedDate = DateTime.UtcNow;
                context.ModulePermissions.Update(existing);
            }
            else
            {
                var permission = new ModulePermission
                {
                    RoleId = roleId,
                    ModuleId = moduleId,
                    CanView = canView,
                    CanCreate = canCreate,
                    CanEdit = canEdit,
                    CanDelete = canDelete,
                    CanApprove = canApprove,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "1"   // ✅ FIX: int -> string ("1" = System user)
                };
                await context.ModulePermissions.AddAsync(permission);
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePermissionAsync(ModulePermission permission)
        {
            permission.UpdatedDate = DateTime.UtcNow;
            context.ModulePermissions.Update(permission);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePermissionAsync(int permissionId)
        {
            var permission = await context.ModulePermissions.FindAsync(permissionId);
            if (permission is null) return false;   // ✅ simplified

            context.ModulePermissions.Remove(permission);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Module>> GetAllModulesAsync()
        {
            return await context.Modules
                .Where(m => m.IsActive == true)
                .OrderBy(m => m.ModuleName)
                .ToListAsync();
        }

        public async Task<Module?> GetModuleByCodeAsync(string moduleCode)
        {
            return await context.Modules
                .FirstOrDefaultAsync(m => m.ModuleCode == moduleCode && m.IsActive == true);
        }
    }
}