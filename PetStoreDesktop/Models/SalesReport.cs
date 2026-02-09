using System.ComponentModel.DataAnnotations;

namespace PetStoreDesktop.Models
{
    /// <summary>
    /// Модель для звіту про продажі товарів
    /// </summary>
    public class SalesReport
    {
        /// <summary>
        /// ID товару
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Назва товару
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Опис товару
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Категорія товару
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Постачальник
        /// </summary>
        public string SupplierName { get; set; } = string.Empty;

        /// <summary>
        /// Загальна кількість проданих одиниць
        /// </summary>
        public int TotalQuantitySold { get; set; }

        /// <summary>
        /// Загальна сума продажів
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Середня ціна продажу
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// Кількість замовлень з цим товаром
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// Дата першої покупки в періоді
        /// </summary>
        public DateTime? FirstSaleDate { get; set; }

        /// <summary>
        /// Дата останньої покупки в періоді
        /// </summary>
        public DateTime? LastSaleDate { get; set; }

        // Властивості для відображення
        public string DisplayRevenue => $"{TotalRevenue:F2} грн";
        public string DisplayAveragePrice => $"{AveragePrice:F2} грн";
        public string DisplayPeriod => FirstSaleDate?.ToString("dd.MM.yyyy") + " - " + LastSaleDate?.ToString("dd.MM.yyyy");
    }

    /// <summary>
    /// Модель для звіту про продажі тварин
    /// </summary>
    public class AnimalSalesReport
    {
        /// <summary>
        /// ID тварини
        /// </summary>
        public int AnimalID { get; set; }

        /// <summary>
        /// Ім'я тварини
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Вид тварини
        /// </summary>
        public string Species { get; set; } = string.Empty;

        /// <summary>
        /// Порода
        /// </summary>
        public string? Breed { get; set; }

        /// <summary>
        /// Кількість проданих тварин
        /// </summary>
        public int QuantitySold { get; set; }

        /// <summary>
        /// Загальний дохід від продажу
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Ціна продажу
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// Дата продажу
        /// </summary>
        public DateTime? SaleDate { get; set; }

        /// <summary>
        /// Ім'я клієнта
        /// </summary>
        public string? CustomerName { get; set; }

        /// <summary>
        /// Вік на момент продажу (у місяцях)
        /// </summary>
        public int? AgeMonths { get; set; }

        // Властивості для відображення
        public string DisplayPrice => $"{SalePrice:F2} грн";
        public string DisplayRevenue => $"{TotalRevenue:F2} грн";
        public string DisplaySaleDate => SaleDate?.ToString("dd.MM.yyyy HH:mm") ?? "-";
        public string DisplayAnimalInfo => $"{Species} {(string.IsNullOrEmpty(Breed) ? "" : $"({Breed})")} - {Name}";
    }

    /// <summary>
    /// Параметри для генерації звітів
    /// </summary>
    public class ReportParameters
    {
        /// <summary>
        /// Початкова дата періоду
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Кінцева дата періоду
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Кількість топ товарів для звіту
        /// </summary>
        public int TopCount { get; set; } = 5;

        /// <summary>
        /// Чи включати неактивні товари
        /// </summary>
        public bool IncludeInactive { get; set; } = false;

        /// <summary>
        /// Фільтрація за категорією (null = всі категорії)
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Мінімальна кількість продажів для включення в звіт
        /// </summary>
        public int MinQuantitySold { get; set; } = 1;

        public ReportParameters()
        {
            // За замовчуванням - останній місяць
            EndDate = DateTime.Now;
            StartDate = EndDate.AddMonths(-1);
        }
    }
}
