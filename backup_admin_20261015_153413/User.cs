using ERPWMS.Application.DTOs.Identity;
using ERPWMS.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPWMS.Controllers.Admin
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public UserController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [Authorize(Policy = "SystemAdmin.View")]
        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm = null, bool? isActive = null, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var serviceUsers = await _userService.GetAllUsersAsync(false);
            var users = serviceUsers?.ToList() ?? new List<ERPWMS.Database.Models.User>();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var s = searchTerm.Trim();
                users = users.Where(u =>
                    (!string.IsNullOrWhiteSpace(u.UserName) && u.UserName.Contains(s, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(u.Email) && u.Email.Contains(s, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(u.FirstName) && u.FirstName.Contains(s, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(u.LastName) && u.LastName.Contains(s, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (isActive.HasValue)
                users = users.Where(u => u.IsActive == isActive).ToList();

            var totalRecords = users.Count;
            var totalPages = totalRecords == 0 ? 0 : (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var paged = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var dtos = paged.Select(u => new UserDto
            {
                Id = u.Id, UserName = u.UserName, Email = u.Email,
                FirstName = u.FirstName, LastName = u.LastName,
                EmployeeCode = u.EmployeeCode, WarehouseId = u.WarehouseId,
                IsActive = u.IsActive, LastLoginDate = u.LastLoginDate
            }).ToList();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.TotalPages = totalPages;

            return View(dtos);
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpGet]
        public IActionResult Create() => View();

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            if (!ModelState.IsValid) return View(request);
            await _userService.CreateUserAsync(request.UserName, request.Email, request.Password,
                request.FirstName, request.LastName, request.EmployeeCode, request.WarehouseId, request.IsActive);
            TempData["Success"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var model = new UpdateUserRequest
            {
                UserId = user.Id, UserName = user.UserName, Email = user.Email,
                FirstName = user.FirstName, LastName = user.LastName,
                EmployeeCode = user.EmployeeCode, WarehouseId = user.WarehouseId,
                IsActive = user.IsActive
            };
            return View(model);
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateUserRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            var user = await _userService.GetUserByIdAsync(request.UserId);
            if (user == null) return NotFound();

            user.UserName = request.UserName;
            user.NormalizedUserName = request.UserName?.ToUpperInvariant();
            user.Email = request.Email;
            user.NormalizedEmail = request.Email?.ToUpperInvariant();
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.EmployeeCode = request.EmployeeCode;
            user.WarehouseId = request.WarehouseId;
            if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;

            await _userService.UpdateUserAsync(user);
            TempData["Success"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            var r = await _userService.ActivateUserAsync(id);
            TempData[r ? "Success" : "Error"] = r ? "User activated." : "User not found.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            // Prevent self-deactivation
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == id.ToString())
            {
                TempData["Error"] = "You cannot deactivate yourself.";
                return RedirectToAction(nameof(Index));
            }

            var r = await _userService.DeactivateUserAsync(id);
            TempData[r ? "Success" : "Error"] = r ? "User deactivated." : "User not found.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpGet]
        public async Task<IActionResult> Roles(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var allRoles = await _roleService.GetAllRolesAsync();
            var userRoles = await _userService.GetUserRolesAsync(id);

            ViewBag.UserId = id;
            ViewBag.UserName = user.UserName;
            ViewBag.UserRoles = userRoles;
            return View(allRoles);
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(int userId, int roleId)
        {
            var r = await _roleService.AddUserToRoleAsync(userId, roleId);
            TempData[r ? "Success" : "Error"] = r ? "Role assigned." : "Failed.";
            return RedirectToAction(nameof(Roles), new { id = userId });
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(int userId, int roleId)
        {
            var r = await _roleService.RemoveUserFromRoleAsync(userId, roleId);
            TempData[r ? "Success" : "Error"] = r ? "Role removed." : "Failed.";
            return RedirectToAction(nameof(Roles), new { id = userId });
        }

        [Authorize(Policy = "SystemAdmin.View")]
        [HttpGet]
        public IActionResult AddPartial() => PartialView("_AddForm");

        [Authorize(Policy = "SystemAdmin.View")]
        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var u = await _userService.GetUserByIdAsync(id);
            if (u == null) return NotFound();
            return PartialView("_EditForm", u);
        }

        [Authorize(Policy = "SystemAdmin.View")]
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var users = await _userService.GetAllUsersAsync(false);
            var list = users?.ToList() ?? new List<ERPWMS.Database.Models.User>();
            return Json(new
            {
                total = list.Count,
                active = list.Count(u => u.IsActive == true),
                inactive = list.Count(u => u.IsActive != true),
                addedThisMonth = 0
            });
        }

        [Authorize(Policy = "SystemAdmin.View")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync(false);
            var list = (users ?? Enumerable.Empty<ERPWMS.Database.Models.User>()).Select(u => new
            {
                userId = u.Id,
                userName = u.UserName,
                fullName = ((u.FirstName ?? "") + " " + (u.LastName ?? "")).Trim(),
                email = u.Email,
                phoneNumber = u.PhoneNumber ?? "",
                roleName = "User",
                isActive = u.IsActive == true
            }).ToList();
            return Json(list);
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickAdd(string userName, string email, string firstName,
            string lastName, string employeeCode, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName)) return Json(new { success = false, message = "Username required" });
                if (string.IsNullOrWhiteSpace(email)) return Json(new { success = false, message = "Email required" });

                // Password strength validation
                if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                    return Json(new { success = false, message = "Password min 8 chars" });
                if (!password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
                    return Json(new { success = false, message = "Password must contain upper, lower, and digit" });

                await _userService.CreateUserAsync(userName, email, password, firstName, lastName, employeeCode, null, true);
                return Json(new { success = true, message = "User created successfully" });
            }
            catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickEdit(int userId, string userName, string email,
            string firstName, string lastName, string employeeCode, bool isActive = true)
        {
            try
            {
                var u = await _userService.GetUserByIdAsync(userId);
                if (u == null) return Json(new { success = false, message = "Not found" });

                u.UserName = userName;
                u.NormalizedUserName = userName?.ToUpperInvariant();
                u.Email = email;
                u.NormalizedEmail = email?.ToUpperInvariant();
                u.FirstName = firstName;
                u.LastName = lastName;
                u.EmployeeCode = employeeCode;
                u.IsActive = isActive;

                await _userService.UpdateUserAsync(u);
                return Json(new { success = true, message = "User updated successfully" });
            }
            catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickDelete(int id)
        {
            try
            {
                // Prevent self-deactivation
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == id.ToString())
                {
                    return Json(new { success = false, message = "You cannot deactivate yourself." });
                }

                var u = await _userService.GetUserByIdAsync(id);
                if (u == null) return Json(new { success = false, message = "Not found" });
                u.IsActive = false;
                await _userService.UpdateUserAsync(u);
                return Json(new { success = true, message = "User deactivated" });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [Authorize(Policy = "SystemAdmin.Manage")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickToggle(int id)
        {
            try
            {
                var u = await _userService.GetUserByIdAsync(id);
                if (u == null) return Json(new { success = false, message = "Not found" });
                u.IsActive = !(u.IsActive ?? false);
                await _userService.UpdateUserAsync(u);
                return Json(new { success = true, message = u.IsActive == true ? "Activated" : "Deactivated" });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }
    }
}
