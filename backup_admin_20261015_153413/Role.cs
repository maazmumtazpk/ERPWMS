using ERPWMS.Application.DTOs.Identity;
using ERPWMS.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Controllers.Admin
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;

        public RoleController(IRoleService roleService, IPermissionService permissionService)
        {
            _roleService = roleService;
            _permissionService = permissionService;
        }

        [Authorize(Policy = "SystemAdmin.View")]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return View(roles);
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpGet]
        public IActionResult Create() => View();

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoleRequest request)
        {
            if (!ModelState.IsValid) return View(request);
            await _roleService.CreateRoleAsync(request.Name);
            TempData["Success"] = "Role created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();

            var model = new UpdateRoleRequest { RoleId = role.Id, Name = role.Name };
            return View(model);
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateRoleRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            var role = await _roleService.GetRoleByIdAsync(request.RoleId);
            if (role == null) return NotFound();

            role.Name = request.Name;
            role.NormalizedName = request.Name.ToUpperInvariant();
            await _roleService.UpdateRoleAsync(role);

            TempData["Success"] = "Role updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _roleService.DeleteRoleAsync(id);
                TempData["Success"] = "Role deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpGet]
        public async Task<IActionResult> Permissions(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();

            var modules = await _permissionService.GetAllModulesAsync();
            var permissions = await _permissionService.GetRolePermissionsAsync(id);

            ViewBag.RoleId = id;
            ViewBag.RoleName = role.Name;
            ViewBag.Permissions = permissions;
            return View(modules);
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePermissions(PermissionUpdateRequest request)
        {
            var r = await _permissionService.AssignPermissionAsync(
                request.RoleId, request.ModuleId,
                request.CanView, request.CanCreate,
                request.CanEdit, request.CanDelete, request.CanApprove);

            TempData[r ? "Success" : "Error"] = r ? "Permissions updated." : "Failed.";
            return RedirectToAction(nameof(Permissions), new { id = request.RoleId });
        }

        [Authorize(Policy = "SystemAdmin.View")]
        [HttpGet]
        public IActionResult AddPartial() => PartialView("_AddForm");

        [Authorize(Policy = "SystemAdmin.View")]
        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var r = await _roleService.GetRoleByIdAsync(id);
            if (r == null) return NotFound();
            return PartialView("_EditForm", r);
        }

        [Authorize(Policy = "SystemAdmin.View")]
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var roles = (await _roleService.GetAllRolesAsync())?.ToList()
                ?? new List<ERPWMS.Database.Models.AspNetRole>();
            return Json(new
            {
                total = roles.Count,
                active = roles.Count,
                inactive = 0,
                userCount = roles.Sum(r => r.Users != null ? r.Users.Count : 0)
            });
        }

        [Authorize(Policy = "SystemAdmin.View")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = (await _roleService.GetAllRolesAsync())?.ToList()
                ?? new List<ERPWMS.Database.Models.AspNetRole>();
            var list = roles.Select(r => new
            {
                roleId = r.Id,
                roleName = r.Name,
                normalizedName = r.NormalizedName,
                userCount = r.Users != null ? r.Users.Count : 0
            }).ToList();
            return Json(list);
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickAdd(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Role name required" });
                await _roleService.CreateRoleAsync(name.Trim());
                return Json(new { success = true, message = "Role created successfully" });
            }
            catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickEdit(int roleId, string name)
        {
            try
            {
                var r = await _roleService.GetRoleByIdAsync(roleId);
                if (r == null) return Json(new { success = false, message = "Not found" });
                r.Name = name;
                r.NormalizedName = name?.ToUpperInvariant();
                await _roleService.UpdateRoleAsync(r);
                return Json(new { success = true, message = "Role updated successfully" });
            }
            catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickDelete(int id)
        {
            try
            {
                await _roleService.DeleteRoleAsync(id);
                return Json(new { success = true, message = "Role deleted" });
            }
            catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }
    }
}
