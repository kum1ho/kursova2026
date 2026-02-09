using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetStoreAPI.Models
{
    /// <summary>
    /// Категорія товарів зоомагазину
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Унікальний ідентифікатор категорії
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID { get; set; }

        /// <summary>
        /// Назва категорії
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Опис категорії
        /// </summary>
        [MaxLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        public string? Description { get; set; }

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
