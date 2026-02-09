using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetStoreDesktop.Models
{
    public class Animal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnimalID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Species { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Breed { get; set; }

        public int? AgeMonths { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? HealthStatus { get; set; }

        public bool IsVaccinated { get; set; }

        public int? CategoryID { get; set; }
        public Category? Category { get; set; }

        public int? SupplierID { get; set; }
        public Supplier? Supplier { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
