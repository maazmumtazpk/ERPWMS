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
    public class UserSessionService : IUserSessionService
    {
        private readonly AppDbContext _context;

        public UserSessionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserSession>> GetActiveSessionsAsync()
        {
            return await _context.UserSessions
                .Include(s => s.User)
                .Where(s => s.IsActive == true)
                .OrderByDescending(s => s.LastActivity)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserSession>> GetUserSessionsAsync(int userId)
        {
            return await _context.UserSessions
                .Include(s => s.User)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.LoginTime)
                .ToListAsync();
        }

        public async Task<UserSession?> GetSessionByTokenAsync(string token)
        {
            return await _context.UserSessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SessionToken == token && s.IsActive == true);
        }

        public async Task<bool> TerminateSessionAsync(int sessionId)
        {
            var session = await _context.UserSessions.FindAsync(sessionId);
            if (session == null) return false;

            session.IsActive = false;
            session.LogoutTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TerminateAllUserSessionsAsync(int userId)
        {
            var sessions = await _context.UserSessions
                .Where(s => s.UserId == userId && s.IsActive == true)
                .ToListAsync();

            if (!sessions.Any()) return true;

            foreach (var session in sessions)
            {
                session.IsActive = false;
                session.LogoutTime = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateLastActivityAsync(string token)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.SessionToken == token && s.IsActive == true);

            if (session != null)
            {
                session.LastActivity = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetActiveSessionCountAsync()
        {
            return await _context.UserSessions
                .CountAsync(s => s.IsActive == true);
        }
    }
}