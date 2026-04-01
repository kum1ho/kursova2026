using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreAPI.Models;

namespace PetStoreAPI.Services
{
    /// <summary>
    /// Сервіс для роботи з користувачами
    /// </summary>
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordService _passwordService;

        public UserService(ApplicationDbContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Реєструє нового користувача
        /// </summary>
        public async Task<(bool Success, string Message, User? User)> RegisterAsync(string username, string email, string password, string? fullName = null)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username || u.Email == email);

            if (existingUser != null)
            {
                if (existingUser.Username == username)
                    return (false, "Користувач з таким логіном вже існує", null);

                if (existingUser.Email == email)
                    return (false, "Користувач з таким email вже існує", null);
            }

            var (isValid, errorMessage) = _passwordService.ValidatePasswordStrength(password);
            if (!isValid)
                return (false, errorMessage, null);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = _passwordService.HashPassword(password),
                FullName = fullName,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);

            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");
            if (defaultRole != null)
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = user.UserId,
                    RoleId = defaultRole.RoleId,
                    AssignedAt = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            return (true, "Користувач успішно зареєстрований", user);
        }

        /// <summary>
        /// Повертає користувача для входу
        /// </summary>
        public async Task<User?> FindForLoginAsync(string username)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);
        }

        /// <summary>
        /// Фіксує невдалу спробу входу
        /// </summary>
        public async Task RecordFailedLoginAsync(User user)
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= 5)
            {
                user.LockedUntil = DateTime.Now.AddMinutes(15);
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Фіксує успішний вхід
        /// </summary>
        public async Task RecordSuccessfulLoginAsync(User user)
        {
            user.FailedLoginAttempts = 0;
            user.LockedUntil = null;
            user.LastLoginAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Змінює пароль користувача
        /// </summary>
        public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return (false, "Користувача не знайдено");

            if (!_passwordService.VerifyPassword(currentPassword, user.PasswordHash))
                return (false, "Поточний пароль невірний");

            var (isValid, errorMessage) = _passwordService.ValidatePasswordStrength(newPassword);
            if (!isValid)
                return (false, errorMessage);

            user.PasswordHash = _passwordService.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return (true, "Пароль успішно змінено");
        }
    }
}