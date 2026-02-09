using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStoreAPI.DTOs;
using PetStoreAPI.Services;
using System.Security.Claims;

namespace PetStoreAPI.Controllers
{
    /// <summary>
    /// Контролер для автентифікації та авторизації
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticationService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthenticationService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Реєстрація нового користувача
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var (success, message, user) = await _authService.RegisterAsync(
                    registerDto.Username,
                    registerDto.Email,
                    registerDto.Password,
                    registerDto.FullName);

                if (!success)
                {
                    return BadRequest(new ErrorResponseDto
                    {
                        Message = message,
                        Details = "Перевірте введені дані та спробуйте знову"
                    });
                }

                // Автоматичний вхід після реєстрації
                var (loginSuccess, loginMessage, authResult) = await _authService.LoginAsync(
                    registerDto.Username,
                    registerDto.Password,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString());

                if (!loginSuccess)
                {
                    return BadRequest(new ErrorResponseDto
                    {
                        Message = loginMessage,
                        Details = "Реєстрація успішна, але не вдалося виконати автоматичний вхід"
                    });
                }

                var response = new AuthResponseDto
                {
                    Token = authResult!.Token,
                    RefreshToken = authResult.RefreshToken,
                    User = authResult.User,
                    ExpiresAt = authResult.ExpiresAt
                };

                _logger.LogInformation("Користувач {Username} успішно зареєстрований", registerDto.Username);

                return CreatedAtAction(nameof(GetCurrentUser), new { }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при реєстрації користувача {Username}", registerDto.Username);
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Внутрішня помилка сервера",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Вхід користувача в систему
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var (success, message, authResult) = await _authService.LoginAsync(
                    loginDto.Username,
                    loginDto.Password,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString());

                if (!success)
                {
                    _logger.LogWarning("Невдала спроба входу для користувача {Username}: {Message}", loginDto.Username, message);
                    return Unauthorized(new ErrorResponseDto
                    {
                        Message = message,
                        Details = "Перевірте логін та пароль"
                    });
                }

                var response = new AuthResponseDto
                {
                    Token = authResult!.Token,
                    RefreshToken = authResult.RefreshToken,
                    User = authResult.User,
                    ExpiresAt = authResult.ExpiresAt
                };

                _logger.LogInformation("Користувач {Username} успішно увійшов в систему", loginDto.Username);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при вході користувача {Username}", loginDto.Username);
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Внутрішня помилка сервера",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Оновлення токена
        /// </summary>
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var (success, message, authResult) = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);

                if (!success)
                {
                    return Unauthorized(new ErrorResponseDto
                    {
                        Message = message,
                        Details = "Потрібно повторно увійти в систему"
                    });
                }

                var response = new AuthResponseDto
                {
                    Token = authResult!.Token,
                    RefreshToken = authResult.RefreshToken,
                    User = authResult.User,
                    ExpiresAt = authResult.ExpiresAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні токена");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Внутрішня помилка сервера",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Вихід з системи
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var success = await _authService.LogoutAsync(token);

                if (success)
                {
                    _logger.LogInformation("Користувач успішно вийшов з системи");
                    return Ok(new { Message = "Вихід успішний" });
                }

                return BadRequest(new ErrorResponseDto
                {
                    Message = "Не вдалося виконати вихід",
                    Details = "Сесія не знайдена"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при виході з системи");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Внутрішня помилка сервера",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Отримання інформації про поточного користувача
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public ActionResult<UserDto> GetCurrentUser()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "";
                var email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
                var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

                var userDto = new UserDto
                {
                    UserId = userId,
                    Username = username,
                    Email = email,
                    Roles = roles,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні інформації про користувача");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Внутрішня помилка сервера",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Зміна пароля
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                var (success, message) = await _authService.ChangePasswordAsync(
                    userId,
                    changePasswordDto.CurrentPassword,
                    changePasswordDto.NewPassword);

                if (!success)
                {
                    return BadRequest(new ErrorResponseDto
                    {
                        Message = message,
                        Details = "Перевірте поточний пароль та спробуйте знову"
                    });
                }

                _logger.LogInformation("Користувач {UserId} успішно змінив пароль", userId);

                return Ok(new { Message = "Пароль успішно змінено" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при зміні пароля користувача");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Внутрішня помилка сервера",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Перевірка валідності токена
        /// </summary>
        [HttpGet("validate")]
        [Authorize]
        public ActionResult<bool> ValidateToken()
        {
            return Ok(true);
        }
    }
}
