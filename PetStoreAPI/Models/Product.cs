using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetStoreAPI.Models
{
    /// <summary>
    /// Товар зоомагазину (корм, іграшки, аксесуари)
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Унікальний ідентифікатор товару
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }

        /// <summary>
        /// Назва товару
        /// </summary>
        [Required]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Опис товару
        /// </summary>
        [MaxLength(1000)]
        [Column(TypeName = "nvarchar(1000)")]
        public string? Description { get; set; }

        /// <summary>
        /// Ціна за одиницю
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Кількість на складі
        /// </summary>
        public int QuantityInStock { get; set; } = 0;

        /// <summary>
        /// Мінімальний рівень для замовлення
        /// </summary>
        public int ReorderLevel { get; set; } = 10;

        /// <summary>
        /// Одиниця виміру
        /// </summary>
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? Unit { get; set; }

        /// <summary>
        /// Вага товару
        /// </summary>
        [Column(TypeName = "decimal(8,3)")]
        public decimal? Weight { get; set; }

        /// <summary>
        /// Розміри
        /// </summary>
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string? Dimensions { get; set; }

        /// <summary>
        /// Артикул
        /// </summary>
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? SKU { get; set; }

        /// <summary>
        /// Посилання на категорію
        /// </summary>
        public int? CategoryID { get; set; }

        /// <summary>
        /// Посилання на постачальника
        /// </summary>
        public int? SupplierID { get; set; }

        /// <summary>
        /// Дата додавання
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime DateAdded { get; set; } = DateTime.Now;

        /// <summary>
        /// Чи активний товар
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Навігаційні властивості
        [ForeignKey("CategoryID")]
        public virtual Category? Category { get; set; }

        [ForeignKey("SupplierID")]
        public virtual Supplier? Supplier { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
