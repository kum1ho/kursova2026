using PetStoreDesktop.Data;
using PetStoreDesktop.Models;
using PetStoreDesktop.Services;

namespace PetStoreDesktop.Services
{
    /// <summary>
    /// Приклад використання сервісу звітів
    /// </summary>
    public class ReportExample
    {
        /// <summary>
        /// Приклад створення звіту про топ-5 товарів за останній місяць
        /// </summary>
        public static async Task ExampleUsage()
        {
            var context = new ApplicationDbContext();
            var reportService = new ReportService(context);

            // Параметри звіту - останній місяць
            var parameters = new ReportParameters
            {
                StartDate = DateTime.Now.AddMonths(-1),
                EndDate = DateTime.Now,
                TopCount = 5,
                IncludeInactive = false,
                MinQuantitySold = 1
            };

            try
            {
                // Отримання топ-5 товарів
                var topProducts = await reportService.GetTopSellingProductsAsync(parameters);

                Console.WriteLine("ТОП-5 НАЙБІЛЬШ ПРОДАВАНИХ ТОВАРІВ ЗА ОСТАННІЙ МІСЯЦЬ:");
                Console.WriteLine("=" + new string('=', 60));

                for (int i = 0; i < topProducts.Count; i++)
                {
                    var product = topProducts[i];
                    Console.WriteLine($"{i + 1}. {product.ProductName}");
                    Console.WriteLine($"   Категорія: {product.CategoryName}");
                    Console.WriteLine($"   Продано: {product.TotalQuantitySold} шт.");
                    Console.WriteLine($"   Дохід: {product.TotalRevenue:F2} грн");
                    Console.WriteLine($"   Середня ціна: {product.AveragePrice:F2} грн");
                    Console.WriteLine($"   Кількість замовлень: {product.OrderCount}");
                    Console.WriteLine();
                }

                // Експорт в CSV
                var csvContent = await reportService.ExportTopProductsToCsvAsync(parameters);
                await reportService.SaveCsvToFileAsync(csvContent, "Top5Products_LastMonth.csv");

                Console.WriteLine("Звіт збережено у файл Top5Products_LastMonth.csv");

                // Отримання статистики
                var (totalOrders, totalRevenue, totalItems) = await reportService.GetSalesStatisticsAsync(parameters);
                Console.WriteLine($"\nСТАТИСТИКА ЗА ПЕРІОД:");
                Console.WriteLine($"Замовлень: {totalOrders}");
                Console.WriteLine($"Товарів продано: {totalItems}");
                Console.WriteLine($"Загальний дохід: {totalRevenue:F2} грн");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        /// <summary>
        /// Приклад створення кастомного звіту
        /// </summary>
        public static async Task CustomReportExample()
        {
            var context = new ApplicationDbContext();
            var reportService = new ReportService(context);

            // Кастомні параметри - останні 3 місяці, тільки корм для тварин
            var parameters = new ReportParameters
            {
                StartDate = DateTime.Now.AddMonths(-3),
                EndDate = DateTime.Now,
                TopCount = 10,
                CategoryId = 8, // Категорія "Корм"
                IncludeInactive = false,
                MinQuantitySold = 5
            };

            try
            {
                var topFeedProducts = await reportService.GetTopSellingProductsAsync(parameters);

                Console.WriteLine("ТОП-10 НАЙБІЛЬШ ПРОДАВАНИХ ТОВАРІВ КАТЕГОРІЇ 'КОРМ' ЗА 3 МІСЯЦІ:");
                Console.WriteLine("=" + new string('=', 70));

                foreach (var product in topFeedProducts)
                {
                    Console.WriteLine($"{product.ProductName,-40} | {product.TotalQuantitySold,5} шт. | {product.TotalRevenue,10:F2} грн");
                }

                // Експорт звіту
                var csvContent = await reportService.ExportTopProductsToCsvAsync(parameters);
                await reportService.SaveCsvToFileAsync(csvContent, "Top10Feed_3Months.csv");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
    }
}
