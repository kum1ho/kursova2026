using PetStoreAPI.DTOs;
using PetStoreAPI.Models;

namespace PetStoreAPI.Services
{
    /// <summary>
    /// Сервіс для автентифікації та авторизації користувачів
    /// </summary>
    public class AuthenticationService
    {
        private readonly PasswordService _passwordService;
        private readonly UserService _userService;
        private readonly SessionService _sessionService;
        private readonly JwtService _jwtService;

        public AuthenticationService(
            PasswordService passwordService,
            UserService userService,
            SessionService sessionService,
            JwtService jwtService)
        {
            _passwordService = passwordService;
            _userService = userService;
            _sessionService = sessionService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Реєстрація нового користувача
        /// </summary>
        public Task<(bool Success, string Message, User? User)> RegisterAsync(string username, string email, string password, string? fullName = null)
        {
            return _userService.RegisterAsync(username, email, password, fullName);
        }

        /// <summary>
        /// Вхід користувача в систему
        /// </summary>
        public async Task<(bool Success, string Message, AuthResult? AuthResult)> LoginAsync(string username, string password, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                var user = await _userService.FindForLoginAsync(username);

                if (user == null)
                {
                    return (false, "Невірний логін або пароль", null);
                }

                if (user.IsLocked)
                {
                    return (false, "Акаунт заблоковано. Спробуйте пізніше.", null);
                }

                if (!user.IsActive)
                {
                    return (false, "Акаунт неактивний", null);
                }

                if (!_passwordService.VerifyPassword(password, user.PasswordHash))
                {
                    await _userService.RecordFailedLoginAsync(user);
                    return (false, "Невірний логін або пароль", null);
                }

                await _userService.RecordSuccessfulLoginAsync(user);

                var token = _jwtService.GenerateJwtToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();
                var session = await _sessionService.CreateSessionAsync(user, token, refreshToken, ipAddress, userAgent);

                return (true, "Вхід успішний", BuildAuthResult(user, session, token, refreshToken));
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
                var session = await _sessionService.GetActiveSessionByRefreshTokenAsync(refreshToken);

                if (session == null)
                {
                    return (false, "Недійсний refresh токен", null);
                }

                if (!session.User.IsActive || session.User.IsLocked)
                {
                    await _sessionService.DeactivateSessionByTokenAsync(session.Token);
                    return (false, "Акаунт неактивний або заблокований", null);
                }

                var newToken = _jwtService.GenerateJwtToken(session.User);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                await _sessionService.UpdateSessionTokensAsync(session, newToken, newRefreshToken);

                return (true, "Токен успішно оновлено", BuildAuthResult(session.User, session, newToken, newRefreshToken));
            }
            catch (Exception ex)
            {
                return (false, $"Помилка оновлення токена: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Вихід користувача
        /// </summary>
        public Task<bool> LogoutAsync(string token)
        {
            return _sessionService.DeactivateSessionByTokenAsync(token);
        }

        /// <summary>
        /// Зміна пароля
        /// </summary>
        public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var result = await _userService.ChangePasswordAsync(userId, currentPassword, newPassword);

                if (!result.Success)
                {
                    return result;
                }

                await _sessionService.DeactivateAllUserSessionsAsync(userId);

                return result;
            }
            catch (Exception ex)
            {
                return (false, $"Помилка зміни пароля: {ex.Message}");
            }
        }

        private static AuthResult BuildAuthResult(User user, UserSession session, string token, string refreshToken)
        {
            return new AuthResult
            {
                Token = token,
                RefreshToken = refreshToken,
                User = BuildUserDto(user),
                ExpiresAt = session.ExpiresAt
            };
        }

        private static UserDto BuildUserDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}