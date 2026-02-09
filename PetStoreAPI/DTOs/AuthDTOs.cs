using System.ComponentModel.DataAnnotations;

namespace PetStoreAPI.DTOs
{
    /// <summary>
    /// DTO для реєстрації користувача
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Логін користувача
        /// </summary>
        [Required(ErrorMessage = "Логін є обов'язковим")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Логін повинен містити від 3 до 50 символів")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Логін може містити лише літери, цифри та підкреслення")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email користувача
        /// </summary>
        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Некоректний формат email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "Пароль є обов'язковим")]
        [StringLength(128, MinimumLength = 6, ErrorMessage = "Пароль повинен містити від 6 до 128 символів")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Підтвердження пароля
        /// </summary>
        [Required(ErrorMessage = "Підтвердження пароля є обов'язковим")]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Повне ім'я
        /// </summary>
        [StringLength(100, ErrorMessage = "Ім'я не може перевищувати 100 символів")]
        public string? FullName { get; set; }
    }

    /// <summary>
    /// DTO для входу користувача
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Логін або email
        /// </summary>
        [Required(ErrorMessage = "Логін є обов'язковим")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "Пароль є обов'язковим")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO для зміни пароля
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// Поточний пароль
        /// </summary>
        [Required(ErrorMessage = "Поточний пароль є обов'язковим")]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// Новий пароль
        /// </summary>
        [Required(ErrorMessage = "Новий пароль є обов'язковим")]
        [StringLength(128, MinimumLength = 6, ErrorMessage = "Пароль повинен містити від 6 до 128 символів")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Підтвердження нового пароля
        /// </summary>
        [Required(ErrorMessage = "Підтвердження пароля є обов'язковим")]
        [Compare("NewPassword", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO для оновлення токена
    /// </summary>
    public class RefreshTokenDto
    {
        /// <summary>
        /// Refresh токен
        /// </summary>
        [Required(ErrorMessage = "Refresh токен є обов'язковим")]
        public string RefreshToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO для відповіді автентифікації
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// JWT токен
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Refresh токен
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Інформація про користувача
        /// </summary>
        public UserDto User { get; set; } = null!;

        /// <summary>
        /// Час закінчення токена
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Час до закінчення токена в секундах
        /// </summary>
        public long ExpiresIn => (long)(ExpiresAt - DateTime.UtcNow).TotalSeconds;
    }

    /// <summary>
    /// DTO для користувача
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// ID користувача
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Логін користувача
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email користувача
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Повне ім'я
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Ролі користувача
        /// </summary>
        public List<string> Roles { get; set; } = new();

        /// <summary>
        /// Дата останнього входу
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Чи активний користувач
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата реєстрації
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO для відповіді з помилкою
    /// </summary>
    public class ErrorResponseDto
    {
        /// <summary>
        /// Повідомлення про помилку
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Деталі помилки
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Час помилки
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
