using System.Security.Principal;

namespace ERPWMS.Application.Common
{
    public interface ICurrentUserService
    {
        string? UserId { get; }                          // ✅ int? → string?
        string? UserName { get; }
        string? Email { get; }

        int? CompanyId { get; }                          // int? rakha
        string? CompanyName { get; }

        string[]? Roles { get; }

        bool IsAuthenticated { get; }

        bool IsInRole(string role);

        IPrincipal? User { get; }

        string GetValidatedUserId();                     // ✅ int → string
        string GetValidatedUserName();

        int GetCompanyId();
        string GetCompanyName();

        string GetUserId();                              // ✅ int → string
    }
}