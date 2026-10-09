using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface ICurrentUserService
    {
        string? UserId { get; }                          // ✅ int? → string?
        string? UserName { get; }
        string? Email { get; }

        int? CompanyId { get; }                          // int? rakha
        string? CompanyName { get; }

        bool IsAuthenticated { get; }
        bool IsInRole(string role);

        Task<string> GetUserIdAsync();                   // ✅ Task<int> → Task<string>
        Task<string> GetUserNameAsync();
    }
}