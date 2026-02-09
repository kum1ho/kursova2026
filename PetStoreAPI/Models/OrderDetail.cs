using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetStoreAPI.Models
{
    /// <summary>
    /// Деталі замовлення (позиції в замовленні)
    /// </summary>
    public class OrderDetail
    {
        /// <summary>
        /// Унікальний ідентифікатор позиції замовлення
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailID { get; set; }

        /// <summary>
        /// Посилання на замовлення
        /// </summary>
        [Required]
        public int OrderID { get; set; }

        /// <summary>
        /// Посилання на товар (може бути NULL, якщо замовляється тварина)
        /// </summary>
        public int? ProductID { get; set; }

        /// <summary>
        /// Посилання на тварину (може бути NULL, якщо замовляється товар)
        /// </summary>
        public int? AnimalID { get; set; }

        /// <summary>
        /// Кількість
        /// </summary>
        [Required]
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Ціна за одиницю на момент замовлення
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Знижка в %
        /// </summary>
        [Column(TypeName = "decimal(5,2)")]
        public decimal Discount { get; set; } = 0;

        /// <summary>
        /// Загальна сума по позиції (обчислюване поле)
        /// </summary>
        [NotMapped]
        public decimal LineTotal => Quantity * UnitPrice * (1 - Discount / 100);

        // Навігаційні властивості
        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("ProductID")]
        public virtual Product? Product { get; set; }

        [ForeignKey("AnimalID")]
        public virtual Animal? Animal { get; set; }
    }
}
