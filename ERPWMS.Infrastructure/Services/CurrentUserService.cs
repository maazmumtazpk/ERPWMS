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

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId
        {
            get
            {
                if (!IsAuthenticated)
                    return null;

                return _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;
            }
        }

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

        public int? CompanyId
        {
            get
            {
                if (!IsAuthenticated)
                    return null;

                var companyIdClaim = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst("CompanyId")?
                    .Value;

                return int.TryParse(companyIdClaim, out var companyId)
                    ? companyId
                    : null;
            }
        }

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

        public IPrincipal? User
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User;
            }
        }

        public bool IsInRole(string role)
        {
            if (!IsAuthenticated)
                return false;

            return _httpContextAccessor.HttpContext?
                .User?
                .IsInRole(role) ?? false;
        }

        public string GetValidatedUserId()
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated.");

            if (string.IsNullOrEmpty(UserId))
                throw new UnauthorizedAccessException("User ID could not be determined.");

            if (!int.TryParse(UserId, out _))
                throw new UnauthorizedAccessException("User ID is not valid.");

            return UserId;
        }

        public string GetValidatedUserName()
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated.");

            if (string.IsNullOrEmpty(UserName))
                throw new UnauthorizedAccessException("User name could not be determined.");

            return UserName;
        }

        public int GetValidatedCompanyId()
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated.");

            if (!CompanyId.HasValue || CompanyId <= 0)
                throw new UnauthorizedAccessException("Company ID could not be determined.");

            return CompanyId.Value;
        }

        public string GetValidatedCompanyName()
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated.");

            if (string.IsNullOrWhiteSpace(CompanyName))
                throw new UnauthorizedAccessException("Company name could not be determined.");

            return CompanyName;
        }

        public string GetUserId() => GetValidatedUserId();
        public int GetCompanyId() => GetValidatedCompanyId();
        public string GetCompanyName() => GetValidatedCompanyName();

        public Task<string> GetUserIdAsync()
        {
            return Task.FromResult(GetValidatedUserId());
        }

        public Task<string> GetUserNameAsync()
        {
            return Task.FromResult(GetValidatedUserName());
        }
    }
}
