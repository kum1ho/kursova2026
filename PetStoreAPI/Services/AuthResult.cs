using PetStoreAPI.DTOs;

namespace PetStoreAPI.Services
{
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
}