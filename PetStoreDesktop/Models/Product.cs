using System.ComponentModel.DataAnnotations;

namespace PetStoreDesktop.Models
{
    /// <summary>
    /// Модель товару для десктопного додатку
    /// </summary>
    public class Product
    {
        public int ProductID { get; set; }

        private string _productName = string.Empty;
        [Required(ErrorMessage = "Назва товару є обов'язковою")]
        [StringLength(200, ErrorMessage = "Назва не може перевищувати 200 символів")]
        public string ProductName 
        { 
            get => _productName; 
            set => _productName = value ?? string.Empty; 
        }

        public string? Description { get; set; }

        private decimal _price;
        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна повинна бути більше 0")]
        public decimal Price 
        { 
            get => _price; 
            set => _price = value; 
        }

        private int _quantityInStock;
        [Range(0, int.MaxValue, ErrorMessage = "Кількість не може бути від'ємною")]
        public int QuantityInStock 
        { 
            get => _quantityInStock; 
            set => _quantityInStock = value; 
        }

        public int ReorderLevel { get; set; } = 10;
        public string? Unit { get; set; }
        public decimal? Weight { get; set; }
        public string? Dimensions { get; set; }
        public string? SKU { get; set; }
        public int? CategoryID { get; set; }
        public int? SupplierID { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Навігаційні властивості
        public Category? Category { get; set; }
        public Supplier? Supplier { get; set; }

        // Властивості для відображення
        public string CategoryName => Category?.CategoryName ?? "Немає категорії";
        public string SupplierName => Supplier?.CompanyName ?? "Немає постачальника";
        public string DisplayPrice => $"{Price:F2} грн";
        public string StockStatus => QuantityInStock <= ReorderLevel ? "Низький залишок" : "В наявності";
    }
}
