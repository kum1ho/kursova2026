using System.ComponentModel.DataAnnotations;

namespace PetStoreDesktop.Models
{
    /// <summary>
    /// Модель постачальника
    /// </summary>
    public class Supplier
    {
        public int SupplierID { get; set; }

        private string _companyName = string.Empty;
        [Required(ErrorMessage = "Назва компанії є обов'язковою")]
        [StringLength(150, ErrorMessage = "Назва не може перевищувати 150 символів")]
        public string CompanyName 
        { 
            get => _companyName; 
            set => _companyName = value ?? string.Empty; 
        }

        private string _contactPerson = string.Empty;
        [Required(ErrorMessage = "Контактна особа є обов'язковою")]
        [StringLength(100, ErrorMessage = "Ім'я не може перевищувати 100 символів")]
        public string ContactPerson 
        { 
            get => _contactPerson; 
            set => _contactPerson = value ?? string.Empty; 
        }

        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
