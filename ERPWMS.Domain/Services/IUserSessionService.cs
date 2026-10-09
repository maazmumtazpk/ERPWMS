using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IUserSessionService
    {
        Task<IEnumerable<UserSession>> GetActiveSessionsAsync();
        Task<IEnumerable<UserSession>> GetUserSessionsAsync(int userId);
        Task<UserSession?> GetSessionByTokenAsync(string token);
        Task<bool> TerminateSessionAsync(int sessionId);
        Task<bool> TerminateAllUserSessionsAsync(int userId);
        Task UpdateLastActivityAsync(string token);
        Task<int> GetActiveSessionCountAsync();
    }
}