using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetStoreAPI.Models
{
    /// <summary>
    /// Модель користувача системи
    /// </summary>
    public class User
    {
        /// <summary>
        /// Унікальний ідентифікатор користувача
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        /// <summary>
        /// Логін користувача (унікальний)
        /// </summary>
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Column(TypeName = "nvarchar(50)")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email користувача (унікальний)
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Хеш пароля користувача (BCrypt)
        /// </summary>
        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Повне ім'я користувача
        /// </summary>
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string? FullName { get; set; }

        /// <summary>
        /// Номер телефону
        /// </summary>
        [StringLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Чи активний користувач
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Дата реєстрації
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Дата останнього входу
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Кількість невдалих спроб входу
        /// </summary>
        public int FailedLoginAttempts { get; set; } = 0;

        /// <summary>
        /// Час блокування (якщо є)
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? LockedUntil { get; set; }

        /// <summary>
        /// Токен для скидання пароля
        /// </summary>
        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string? PasswordResetToken { get; set; }

        /// <summary>
        /// Час закінчення токена скидання пароля
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? PasswordResetTokenExpiry { get; set; }

        // Навігаційні властивості
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();

        // Властивості для відображення
        public bool IsLocked => LockedUntil.HasValue && LockedUntil.Value > DateTime.Now;
        public string DisplayName => string.IsNullOrEmpty(FullName) ? Username : FullName;
    }

    /// <summary>
    /// Модель ролі користувача
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Унікальний ідентифікатор ролі
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        /// <summary>
        /// Назва ролі (унікальна)
        /// </summary>
        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Опис ролі
        /// </summary>
        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string? Description { get; set; }

        /// <summary>
        /// Чи активна роль
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Дата створення ролі
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Навігаційні властивості
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    /// <summary>
    /// Зв'язуюча таблиця між користувачами та ролями
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// Унікальний ідентифікатор
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserRoleId { get; set; }

        /// <summary>
        /// ID користувача
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// ID ролі
        /// </summary>
        [Required]
        public int RoleId { get; set; }

        /// <summary>
        /// Дата призначення ролі
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime AssignedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Хто призначив роль
        /// </summary>
        public int? AssignedBy { get; set; }

        // Навігаційні властивості
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;
    }

    /// <summary>
    /// Модель сесії користувача
    /// </summary>
    public class UserSession
    {
        /// <summary>
        /// Унікальний ідентифікатор сесії
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SessionId { get; set; }

        /// <summary>
        /// ID користувача
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// JWT токен
        /// </summary>
        [Required]
        [StringLength(1000)]
        [Column(TypeName = "nvarchar(1000)")]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Refresh токен
        /// </summary>
        [StringLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// IP адреса
        /// </summary>
        [StringLength(45)]
        [Column(TypeName = "nvarchar(45)")]
        public string? IpAddress { get; set; }

        /// <summary>
        /// User Agent
        /// </summary>
        [StringLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        public string? UserAgent { get; set; }

        /// <summary>
        /// Час створення сесії
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Час останньої активності
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime LastActivityAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Час закінчення токена
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Чи активна сесія
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Навігаційні властивості
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        // Властивості для відображення
        public bool IsExpired => ExpiresAt < DateTime.Now;
        public TimeSpan TimeUntilExpiry => ExpiresAt - DateTime.Now;
    }
}
