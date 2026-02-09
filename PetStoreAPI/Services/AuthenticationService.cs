using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetStoreAPI.Data;
using PetStoreAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PetStoreAPI.Services
{
    /// <summary>
    /// Сервіс для автентифікації та авторизації користувачів
    /// </summary>
    public class AuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly IConfiguration _configuration;

        public AuthenticationService(ApplicationDbContext context, PasswordService passwordService, IConfiguration configuration)
        {
            _context = context;
            _passwordService = passwordService;
            _configuration = configuration;
        }

        /// <summary>
        /// Реєстрація нового користувача
        /// </summary>
        public async Task<(bool Success, string Message, User? User)> RegisterAsync(string username, string email, string password, string? fullName = null)
        {
            try
            {
                // Перевірка, чи існує користувач з таким логіном
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username || u.Email == email);

                if (existingUser != null)
                {
                    if (existingUser.Username == username)
                        return (false, "Користувач з таким логіном вже існує", null);
                    if (existingUser.Email == email)
                        return (false, "Користувач з таким email вже існує", null);
                }

                // Валідація пароля
                var (isValid, errorMessage) = _passwordService.ValidatePasswordStrength(password);
                if (!isValid)
                    return (false, errorMessage, null);

                // Хешування пароля
                string passwordHash = _passwordService.HashPassword(password);

                // Створення нового користувача
                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = passwordHash,
                    FullName = fullName,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);

                // Призначення ролі за замовчуванням (User)
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
            catch (Exception ex)
            {
                return (false, $"Помилка реєстрації: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Вхід користувача в систему
        /// </summary>
        public async Task<(bool Success, string Message, AuthResult? AuthResult)> LoginAsync(string username, string password, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                // Пошук користувача за логіном або email
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);

                if (user == null)
                {
                    return (false, "Невірний логін або пароль", null);
                }

                // Перевірка, чи заблокований користувач
                if (user.IsLocked)
                {
                    return (false, "Акаунт заблоковано. Спробуйте пізніше.", null);
                }

                // Перевірка, чи активний користувач
                if (!user.IsActive)
                {
                    return (false, "Акаунт неактивний", null);
                }

                // Перевірка пароля
                if (!_passwordService.VerifyPassword(password, user.PasswordHash))
                {
                    // Збільшення кількості невдалих спроб
                    user.FailedLoginAttempts++;
                    
                    // Блокування після 5 невдалих спроб на 15 хвилин
                    if (user.FailedLoginAttempts >= 5)
                    {
                        user.LockedUntil = DateTime.Now.AddMinutes(15);
                    }

                    await _context.SaveChangesAsync();
                    return (false, "Невірний логін або пароль", null);
                }

                // Скидання лічильника невдалих спроб
                user.FailedLoginAttempts = 0;
                user.LockedUntil = null;
                user.LastLoginAt = DateTime.Now;

                await _context.SaveChangesAsync();

                // Створення JWT токена
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                // Збереження сесії
                var session = new UserSession
                {
                    UserId = user.UserId,
                    Token = token,
                    RefreshToken = refreshToken,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    CreatedAt = DateTime.Now,
                    LastActivityAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddHours(24), // Токен дійсний 24 години
                    IsActive = true
                };

                _context.UserSessions.Add(session);
                await _context.SaveChangesAsync();

                var authResult = new AuthResult
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    User = new UserDto
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        FullName = user.FullName,
                        Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                        LastLoginAt = user.LastLoginAt
                    },
                    ExpiresAt = session.ExpiresAt
                };

                return (true, "Вхід успішний", authResult);
            }
            catch (Exception ex)
            {
                return (false, $"Помилка входу: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Оновлення токена
        /// </summary>
        public async Task<(bool Success, string Message, AuthResult? AuthResult)> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var session = await _context.UserSessions
                    .Include(s => s.User)
                    .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && s.IsActive && !s.IsExpired);

                if (session == null)
                {
                    return (false, "Недійсний refresh токен", null);
                }

                // Перевірка, чи активний користувач
                if (!session.User.IsActive || session.User.IsLocked)
                {
                    // Деактивуємо сесію
                    session.IsActive = false;
                    await _context.SaveChangesAsync();
                    return (false, "Акаунт неактивний або заблокований", null);
                }

                // Створення нових токенів
                var newToken = GenerateJwtToken(session.User);
                var newRefreshToken = GenerateRefreshToken();

                // Оновлення сесії
                session.Token = newToken;
                session.RefreshToken = newRefreshToken;
                session.LastActivityAt = DateTime.Now;
                session.ExpiresAt = DateTime.Now.AddHours(24);

                await _context.SaveChangesAsync();

                var authResult = new AuthResult
                {
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    User = new UserDto
                    {
                        UserId = session.User.UserId,
                        Username = session.User.Username,
                        Email = session.User.Email,
                        FullName = session.User.FullName,
                        Roles = session.User.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                        LastLoginAt = session.User.LastLoginAt
                    },
                    ExpiresAt = session.ExpiresAt
                };

                return (true, "Токен успішно оновлено", authResult);
            }
            catch (Exception ex)
            {
                return (false, $"Помилка оновлення токена: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Вихід користувача
        /// </summary>
        public async Task<bool> LogoutAsync(string token)
        {
            try
            {
                var session = await _context.UserSessions
                    .FirstOrDefaultAsync(s => s.Token == token && s.IsActive);

                if (session != null)
                {
                    session.IsActive = false;
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Зміна пароля
        /// </summary>
        public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return (false, "Користувача не знайдено");

                // Перевірка поточного пароля
                if (!_passwordService.VerifyPassword(currentPassword, user.PasswordHash))
                    return (false, "Поточний пароль невірний");

                // Валідація нового пароля
                var (isValid, errorMessage) = _passwordService.ValidatePasswordStrength(newPassword);
                if (!isValid)
                    return (false, errorMessage);

                // Оновлення пароля
                user.PasswordHash = _passwordService.HashPassword(newPassword);
                await _context.SaveChangesAsync();

                // Вихід з усіх сесій крім поточної
                await LogoutAllSessionsExceptCurrent(userId);

                return (true, "Пароль успішно змінено");
            }
            catch (Exception ex)
            {
                return (false, $"Помилка зміни пароля: {ex.Message}");
            }
        }

        /// <summary>
        /// Генерація JWT токена
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Додавання ролей
            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Генерація refresh токена
        /// </summary>
        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        /// <summary>
        /// Вихід з усіх сесій крім поточної
        /// </summary>
        private async Task LogoutAllSessionsExceptCurrent(int userId)
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

    /// <summary>
    /// Результат автентифікації
    /// </summary>
    public class AuthResult
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// DTO для користувача
    /// </summary>
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime? LastLoginAt { get; set; }
    }
}
