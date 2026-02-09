using System.ComponentModel.DataAnnotations;

namespace PetStoreAPI.DTOs
{
    /// <summary>
    /// DTO для створення замовлення
    /// </summary>
    public class CreateOrderDTO
    {
        /// <summary>
        /// Ім'я клієнта
        /// </summary>
        [Required(ErrorMessage = "Ім'я клієнта є обов'язковим")]
        [MaxLength(150)]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Телефон клієнта
        /// </summary>
        [MaxLength(20)]
        public string? CustomerPhone { get; set; }

        /// <summary>
        /// Email клієнта
        /// </summary>
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Некоректний формат email")]
        public string? CustomerEmail { get; set; }

        /// <summary>
        /// Адреса доставки
        /// </summary>
        [MaxLength(500)]
        public string? CustomerAddress { get; set; }

        /// <summary>
        /// Бажана дата доставки
        /// </summary>
        public DateTime? RequiredDate { get; set; }

        /// <summary>
        /// Спосіб оплати
        /// </summary>
        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// Примітки до замовлення
        /// </summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Деталі замовлення
        /// </summary>
        [Required(ErrorMessage = "Деталі замовлення є обов'язковими")]
        [MinLength(1, ErrorMessage = "Має бути хоча б одна позиція в замовленні")]
        public List<CreateOrderDetailDTO> OrderDetails { get; set; } = new();
    }

    /// <summary>
    /// DTO для створення позиції замовлення
    /// </summary>
    public class CreateOrderDetailDTO
    {
        /// <summary>
        /// ID товару (або AnimalID для тварин)
        /// </summary>
        [Required(ErrorMessage = "ID товару/тварини є обов'язковим")]
        public int ItemId { get; set; }

        /// <summary>
        /// Тип позиції: Product або Animal
        /// </summary>
        [Required(ErrorMessage = "Тип позиції є обов'язковим")]
        public string ItemType { get; set; } = string.Empty; // "Product" або "Animal"

        /// <summary>
        /// Кількість
        /// </summary>
        [Required(ErrorMessage = "Кількість є обов'язковою")]
        [Range(1, int.MaxValue, ErrorMessage = "Кількість повинна бути більше 0")]
        public int Quantity { get; set; }

        /// <summary>
        /// Знижка в %
        /// </summary>
        [Range(0, 100, ErrorMessage = "Знижка повинна бути в діапазоні від 0 до 100")]
        public decimal Discount { get; set; } = 0;
    }
}
