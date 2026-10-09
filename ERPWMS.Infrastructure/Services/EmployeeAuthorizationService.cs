using ERPWMS.Domain.Exceptions;
using ERPWMS.Domain.Interfaces;    // <-- YEH ADD KIYA (Sahi jagah)
using ERPWMS.Domain.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Infrastructure.Services
{
    public class EmployeeAuthorizationService(
        IEmployeeRepository repo,
        IHttpContextAccessor httpContextAccessor)
        : IEmployeeAuthorizationService
    {
        private readonly IEmployeeRepository _repo = repo;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        private bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }

        private int? GetDepartmentId()
        {
            var departmentClaim = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst("DepartmentId")?.Value;

            if (departmentClaim != null &&
                int.TryParse(departmentClaim, out int departmentId))
            {
                return departmentId;
            }

            return null;
        }

        public async Task<bool> CanAccessEmployeeAsync(
            int employeeId,
            int userId)
        {
            // Super Admin / Admin can access all employees
            if (IsInRole("SuperAdmin") || IsInRole("Admin"))
                return true;

            // HR can access all employees
            if (IsInRole("HR"))
                return true;

            // Manager can access employees in their department
            if (IsInRole("Manager"))
            {
                var employee = await _repo.GetByIdAsync(employeeId);

                if (employee == null)
                    return false;

                var managerDepartmentId = GetDepartmentId();

                return managerDepartmentId.HasValue &&
                       employee.DepartmentId == managerDepartmentId;
            }

            // Employee can access own record
            return employeeId == userId;
        }

        public async Task<bool> CanManageEmployeeAsync(
            int employeeId,
            int userId)
        {
            // Super Admin / Admin can manage all employees
            if (IsInRole("SuperAdmin") || IsInRole("Admin"))
                return true;

            // HR can manage all employees
            if (IsInRole("HR"))
                return true;

            // Manager can manage employees in their department
            if (IsInRole("Manager"))
            {
                var employee = await _repo.GetByIdAsync(employeeId);

                if (employee == null)
                    return false;

                var managerDepartmentId = GetDepartmentId();

                return managerDepartmentId.HasValue &&
                       employee.DepartmentId == managerDepartmentId;
            }

            return false;
        }

        public Task<bool> CanManageAttendanceAsync(
            int employeeId,
            int userId)
        {
            if (IsInRole("HR") ||
                IsInRole("Manager") ||
                IsInRole("SuperAdmin") ||
                IsInRole("Admin"))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(employeeId == userId);
        }

        public Task<bool> CanManagePayrollAsync(
            int employeeId,
            int userId)
        {
            return Task.FromResult(
                IsInRole("HR") ||
                IsInRole("Admin") ||
                IsInRole("SuperAdmin"));
        }

        public Task<bool> CanApproveLeaveAsync(
            int employeeId,
            int userId)
        {
            return Task.FromResult(
                IsInRole("HR") ||
                IsInRole("Manager") ||
                IsInRole("Admin") ||
                IsInRole("SuperAdmin"));
        }

        public async Task EnsureAccessAsync(
            int employeeId,
            int userId)
        {
            if (!await CanAccessEmployeeAsync(employeeId, userId))
            {
                throw new UnauthorizedEmployeeAccessException(
                    userId,
                    employeeId);
            }
        }

        public async Task EnsureManagementAsync(
            int employeeId,
            int userId)
        {
            if (!await CanManageEmployeeAsync(employeeId, userId))
            {
                throw new UnauthorizedEmployeeAccessException(
                    userId,
                    employeeId);
            }
        }

        public async Task EnsureAttendanceManagementAsync(
            int employeeId,
            int userId)
        {
            if (!await CanManageAttendanceAsync(employeeId, userId))
            {
                throw new UnauthorizedEmployeeAccessException(
                    userId,
                    employeeId);
            }
        }

        public async Task EnsurePayrollManagementAsync(
            int employeeId,
            int userId)
        {
            if (!await CanManagePayrollAsync(employeeId, userId))
            {
                throw new UnauthorizedEmployeeAccessException(
                    userId,
                    employeeId);
            }
        }
    }
}
