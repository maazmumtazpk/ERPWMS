using ERPWMS.Application.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Infrastructure.Services
{
    public class CurrentUserService :
        ERPWMS.Application.Common.ICurrentUserService,
        ERPWMS.Domain.Services.ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // =====================================================
        // USER ID  ✅ string?
        // =====================================================
        public string? UserId
        {
            get
            {
                if (!IsAuthenticated)
                    return null;

                var userIdClaim =
                    _httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value;

                return userIdClaim;
            }
        }

        // =====================================================
        // USER NAME
        // =====================================================
        public string? UserName
        {
            get
            {
                if (!IsAuthenticated)
                    return null;

                return _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.Name)?
                    .Value;
            }
        }

        // =====================================================
        // EMAIL
        // =====================================================
        public string? Email
        {
            get
            {
                if (!IsAuthenticated)
                    return null;

                return _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.Email)?
                    .Value;
            }
        }

        // =====================================================
        // COMPANY ID  (int? rakha — Company.Id int hai)
        // =====================================================
        public int? CompanyId
        {
            get
            {
                if (!IsAuthenticated)
                    return null;

                var companyIdClaim =
                    _httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst("CompanyId")?
                        .Value;

                return int.TryParse(companyIdClaim, out var companyId)
                    ? companyId
                    : null;
            }
        }

        // =====================================================
        // COMPANY NAME
        // =====================================================
        public string? CompanyName
        {
            get
            {
                if (!IsAuthenticated)
                    return null;

                return _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst("CompanyName")?
                    .Value;
            }
        }

        // =====================================================
        // ROLES
        // =====================================================
        public string[]? Roles
        {
            get
            {
                if (!IsAuthenticated)
                    return Array.Empty<string>();

                return _httpContextAccessor.HttpContext?
                    .User?
                    .FindAll(ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToArray();
            }
        }

        // =====================================================
        // AUTHENTICATION
        // =====================================================
        public bool IsAuthenticated
        {
            get
            {
                return _httpContextAccessor.HttpContext?
                    .User?
                    .Identity?
                    .IsAuthenticated ?? false;
            }
        }

        // =====================================================
        // USER PRINCIPAL
        // =====================================================
        public IPrincipal? User
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User;
            }
        }

        // =====================================================
        // ROLE CHECK
        // =====================================================
        public bool IsInRole(string role)
        {
            if (!IsAuthenticated)
                return false;

            return _httpContextAccessor.HttpContext?
                .User?
                .IsInRole(role) ?? false;
        }

        // =====================================================
        // VALIDATED USER ID  ✅ string
        // =====================================================
        public string GetValidatedUserId()
        {
            if (!IsAuthenticated)
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            if (string.IsNullOrEmpty(UserId))
            {
                throw new UnauthorizedAccessException(
                    "User ID could not be determined.");
            }

            return UserId;
        }

        // =====================================================
        // VALIDATED USER NAME
        // =====================================================
        public string GetValidatedUserName()
        {
            if (!IsAuthenticated)
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            if (string.IsNullOrEmpty(UserName))
            {
                throw new UnauthorizedAccessException(
                    "User name could not be determined.");
            }

            return UserName;
        }

        // =====================================================
        // VALIDATED COMPANY ID
        // =====================================================
        public int GetValidatedCompanyId()
        {
            if (!IsAuthenticated)
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            if (!CompanyId.HasValue || CompanyId <= 0)
            {
                throw new UnauthorizedAccessException(
                    "Company ID could not be determined.");
            }

            return CompanyId ?? 0;
        }

        // =====================================================
        // VALIDATED COMPANY NAME
        // =====================================================
        public string GetValidatedCompanyName()
        {
            if (!IsAuthenticated)
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            if (string.IsNullOrWhiteSpace(CompanyName))
            {
                throw new UnauthorizedAccessException(
                    "Company name could not be determined.");
            }

            return CompanyName;
        }

        // =====================================================
        // APPLICATION COMMON - USER ID  ✅ string
        // =====================================================
        public string GetUserId()
        {
            return GetValidatedUserId();
        }

        // =====================================================
        // APPLICATION COMMON - COMPANY ID
        // =====================================================
        public int GetCompanyId()
        {
            return GetValidatedCompanyId();
        }

        // =====================================================
        // APPLICATION COMMON - COMPANY NAME
        // =====================================================
        public string GetCompanyName()
        {
            return GetValidatedCompanyName();
        }

        // =====================================================
        // DOMAIN SERVICE - ASYNC USER ID  ✅ Task<string>
        // =====================================================
        public Task<string> GetUserIdAsync()
        {
            return Task.FromResult(
                GetValidatedUserId());
        }

        // =====================================================
        // DOMAIN SERVICE - ASYNC USER NAME
        // =====================================================
        public Task<string> GetUserNameAsync()
        {
            return Task.FromResult(
                GetValidatedUserName());
        }
    }
}