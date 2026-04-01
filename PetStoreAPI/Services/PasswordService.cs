namespace PetStoreAPI.Services
{
    /// <summary>
    /// Сервіс для роботи з паролями з використанням BCrypt
    /// </summary>
    public class PasswordService
    {
        private const int SaltRounds = 12; // Рівень складності хешування

        /// <summary>
        /// Хешує пароль за допомогою BCrypt
        /// </summary>
        /// <param name="password">Пароль для хешування</param>
        /// <returns>Хеш пароля</returns>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Пароль не може бути порожнім", nameof(password));

            // Перевірка мінімальної довжини пароля
            if (password.Length < 6)
                throw new ArgumentException("Пароль повинен містити щонайменше 6 символів", nameof(password));

            // Хешування пароля з автоматичною генерацією солі
            string hash = BCrypt.Net.BCrypt.HashPassword(password, SaltRounds);

            return hash;
        }

        /// <summary>
        /// Перевіряє, чи відповідає пароль хешу
        /// </summary>
        /// <param name="password">Пароль для перевірки</param>
        /// <param name="hash">Хеш пароля з бази даних</param>
        /// <returns>True, якщо пароль правильний</returns>
        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(hash))
                return false;

            try
            {
                // BCrypt автоматично витягує сіль з хешу та перевіряє пароль
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                // Якщо хеш пошкоджений або має неправильний формат
                return false;
            }
        }

        /// <summary>
        /// Перевіряє складність пароля
        /// </summary>
        /// <param name="password">Пароль для перевірки</param>
        /// <returns>Результат перевірки з повідомленням про помилку</returns>
        public (bool IsValid, string ErrorMessage) ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Пароль є обов'язковим полем");

            if (password.Length < 6)
                return (false, "Пароль повинен містити щонайменше 6 символів");

            if (password.Length > 128)
                return (false, "Пароль не може перевищувати 128 символів");

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

            if (!hasUpper)
                return (false, "Пароль повинен містити хоча б одну велику літеру");

            if (!hasLower)
                return (false, "Пароль повинен містити хоча б одну малу літеру");

            if (!hasDigit)
                return (false, "Пароль повинен містити хоча б одну цифру");

            if (!hasSpecial)
                return (false, "Пароль повинен містити хоча б один спеціальний символ");

            return (true, string.Empty);
        }

        /// <summary>
        /// Генерує тимчасовий пароль
        /// </summary>
        /// <param name="length">Довжина пароля</param>
        /// <returns>Згенерований пароль</returns>
        public string GenerateTemporaryPassword(int length = 12)
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string digitChars = "0123456789";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var random = new Random();
            var password = new char[length];

            // Гарантовано додаємо по одному символу кожного типу
            password[0] = upperChars[random.Next(upperChars.Length)];
            password[1] = lowerChars[random.Next(lowerChars.Length)];
            password[2] = digitChars[random.Next(digitChars.Length)];
            password[3] = specialChars[random.Next(specialChars.Length)];

            // Заповнюємо решту випадковими символами
            string allChars = upperChars + lowerChars + digitChars + specialChars;
            for (int i = 4; i < length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            // Перемішуємо символи
            for (int i = 0; i < length; i++)
            {
                int swapIndex = random.Next(length);
                (password[i], password[swapIndex]) = (password[swapIndex], password[i]);
            }

            return new string(password);
        }

        /// <summary>
        /// Перевіряє, чи потрібно змінити пароль (старіший за 90 днів)
        /// </summary>
        /// <param name="lastPasswordChange">Дата останньої зміни пароля</param>
        /// <returns>True, якщо потрібно змінити</returns>
        public bool ShouldChangePassword(DateTime? lastPasswordChange)
        {
            if (!lastPasswordChange.HasValue)
                return true;

            return DateTime.Now - lastPasswordChange.Value > TimeSpan.FromDays(90);
        }
    }
}
