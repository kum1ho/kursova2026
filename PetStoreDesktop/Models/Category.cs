using System.ComponentModel.DataAnnotations;

namespace PetStoreDesktop.Models
{
    /// <summary>
    /// Модель категорії
    /// </summary>
    public class Category
    {
        public int CategoryID { get; set; }

        private string _categoryName = string.Empty;
        [Required(ErrorMessage = "Назва категорії є обов'язковою")]
        [StringLength(100, ErrorMessage = "Назва не може перевищувати 100 символів")]
        public string CategoryName 
        { 
            get => _categoryName; 
            set => _categoryName = value ?? string.Empty; 
        }

        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
