using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetStoreAPI.Models
{
    /// <summary>
    /// Замовлення клієнта
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Унікальний ідентифікатор замовлення
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        /// <summary>
        /// Ім'я клієнта
        /// </summary>
        [Required]
        [MaxLength(150)]
        [Column(TypeName = "nvarchar(150)")]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Телефон клієнта
        /// </summary>
        [MaxLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string? CustomerPhone { get; set; }

        /// <summary>
        /// Email клієнта
        /// </summary>
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string? CustomerEmail { get; set; }

        /// <summary>
        /// Адреса доставки
        /// </summary>
        [MaxLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        public string? CustomerAddress { get; set; }

        /// <summary>
        /// Дата замовлення
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Бажана дата доставки
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? RequiredDate { get; set; }

        /// <summary>
        /// Дата відвантаження
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? ShippedDate { get; set; }

        /// <summary>
        /// Загальна сума замовлення
        /// </summary>
        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalAmount { get; set; } = 0;

        /// <summary>
        /// Статус замовлення
        /// </summary>
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; } = "Нове";

        /// <summary>
        /// Спосіб оплати
        /// </summary>
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// Статус оплати
        /// </summary>
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string PaymentStatus { get; set; } = "Не оплачено";

        /// <summary>
        /// Примітки до замовлення
        /// </summary>
        [MaxLength(1000)]
        [Column(TypeName = "nvarchar(1000)")]
        public string? Notes { get; set; }

        /// <summary>
        /// Дата створення запису
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Навігаційні властивості
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
