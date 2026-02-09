using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetStoreAPI.Models
{
    /// <summary>
    /// Постачальник товарів
    /// </summary>
    public class Supplier
    {
        /// <summary>
        /// Унікальний ідентифікатор постачальника
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplierID { get; set; }

        /// <summary>
        /// Назва компанії постачальника
        /// </summary>
        [Required]
        [MaxLength(150)]
        [Column(TypeName = "nvarchar(150)")]
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// Контактна особа
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string ContactPerson { get; set; } = string.Empty;

        /// <summary>
        /// Телефон постачальника
        /// </summary>
        [MaxLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string? Phone { get; set; }

        /// <summary>
        /// Email постачальника
        /// </summary>
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string? Email { get; set; }

        /// <summary>
        /// Адреса постачальника
        /// </summary>
        [MaxLength(300)]
        [Column(TypeName = "nvarchar(300)")]
        public string? Address { get; set; }

        /// <summary>
        /// Місто
        /// </summary>
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string? City { get; set; }

        /// <summary>
        /// Країна
        /// </summary>
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string? Country { get; set; }

        /// <summary>
        /// Дата створення запису
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Навігаційні властивості
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}
