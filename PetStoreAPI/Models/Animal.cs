using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetStoreAPI.Models
{
    /// <summary>
    /// Жива тварина, що продається в магазині
    /// </summary>
    public class Animal
    {
        /// <summary>
        /// Унікальний ідентифікатор тварини
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnimalID { get; set; }

        /// <summary>
        /// Кличка або назва тварини
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Вид тварини
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string Species { get; set; } = string.Empty;

        /// <summary>
        /// Порода
        /// </summary>
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string? Breed { get; set; }

        /// <summary>
        /// Вік у місяцях
        /// </summary>
        public int? AgeMonths { get; set; }

        /// <summary>
        /// Стать
        /// </summary>
        [MaxLength(10)]
        [Column(TypeName = "nvarchar(10)")]
        public string? Gender { get; set; }

        /// <summary>
        /// Колір
        /// </summary>
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? Color { get; set; }

        /// <summary>
        /// Вага в кг
        /// </summary>
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Weight { get; set; }

        /// <summary>
        /// Ціна
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Дата народження
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Доступність для продажу
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Посилання на категорію
        /// </summary>
        public int? CategoryID { get; set; }

        /// <summary>
        /// Посилання на постачальника
        /// </summary>
        public int? SupplierID { get; set; }

        /// <summary>
        /// Дата додавання в магазин
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime DateAdded { get; set; } = DateTime.Now;

        /// <summary>
        /// Додатковий опис
        /// </summary>
        [MaxLength(1000)]
        [Column(TypeName = "nvarchar(1000)")]
        public string? Description { get; set; }

        // Навігаційні властивості
        [ForeignKey("CategoryID")]
        public virtual Category? Category { get; set; }

        [ForeignKey("SupplierID")]
        public virtual Supplier? Supplier { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
