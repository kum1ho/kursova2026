using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreAPI.Models;

namespace PetStoreAPI.Services
{
    /// <summary>
    /// Сервіс для керування сесіями користувачів
    /// </summary>
    public class SessionService
    {
        private readonly ApplicationDbContext _context;

        public SessionService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Створює нову сесію користувача
        /// </summary>
        public async Task<UserSession> CreateSessionAsync(User user, string token, string refreshToken, string? ipAddress = null, string? userAgent = null)
        {
            var session = new UserSession
            {
                UserId = user.UserId,
                Token = token,
                RefreshToken = refreshToken,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTime.Now,
                LastActivityAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddHours(24),
                IsActive = true
            };

            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();

            return session;
        }

        /// <summary>
        /// Повертає активну сесію за refresh токеном
        /// </summary>
        public async Task<UserSession?> GetActiveSessionByRefreshTokenAsync(string refreshToken)
        {
            return await _context.UserSessions
                .Include(s => s.User)
                .ThenInclude(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && s.IsActive && !s.IsExpired);
        }

        /// <summary>
        /// Деактивує сесію за JWT токеном
        /// </summary>
        public async Task<bool> DeactivateSessionByTokenAsync(string token)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.Token == token && s.IsActive);

            if (session == null)
            {
                return false;
            }

            session.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Оновлює токени існуючої сесії
        /// </summary>
        public async Task UpdateSessionTokensAsync(UserSession session, string token, string refreshToken)
        {
            session.Token = token;
            session.RefreshToken = refreshToken;
            session.LastActivityAt = DateTime.Now;
            session.ExpiresAt = DateTime.Now.AddHours(24);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Деактивує всі активні сесії користувача
        /// </summary>
        public async Task DeactivateAllUserSessionsAsync(int userId)
        {
            var sessions = await _context.UserSessions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync();

            foreach (var session in sessions)
            {
                session.IsActive = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}