using Microsoft.EntityFrameworkCore;
using PetStoreDesktop.Data;
using PetStoreDesktop.Models;
using System.IO;
using System.Text;

namespace PetStoreDesktop.Services
{
    /// <summary>
    /// Сервіс для створення звітів про продажі
    /// </summary>
    public class ReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримати топ-N найбільш продаваних товарів за період
        /// </summary>
        public async Task<List<SalesReport>> GetTopSellingProductsAsync(ReportParameters parameters)
        {
            try
            {
                var query = from od in _context.OrderDetails
                            join o in _context.Orders on od.OrderID equals o.OrderID
                            join p in _context.Products on od.ProductID equals p.ProductID
                            join c in _context.Categories on p.CategoryID equals c.CategoryID into categoryGroup
                            from c in categoryGroup.DefaultIfEmpty()
                            join s in _context.Suppliers on p.SupplierID equals s.SupplierID into supplierGroup
                            from s in supplierGroup.DefaultIfEmpty()
                            where o.OrderDate >= parameters.StartDate
                                  && o.OrderDate <= parameters.EndDate
                                  && od.ProductID.HasValue
                            group new { od, p, c, s, o } by new { p.ProductID, p.ProductName, p.Description, CategoryName = c.CategoryName, SupplierName = s.CompanyName } into g
                            select new SalesReport
                            {
                                ProductID = g.Key.ProductID,
                                ProductName = g.Key.ProductName,
                                Description = g.Key.Description,
                                CategoryName = g.Key.CategoryName ?? "Без категорії",
                                SupplierName = g.Key.SupplierName ?? "Невідомий постачальник",
                                TotalQuantitySold = g.Sum(x => x.od.Quantity),
                                TotalRevenue = g.Sum(x => x.od.Quantity * x.od.UnitPrice * (1 - x.od.Discount / 100)),
                                AveragePrice = g.Average(x => x.od.UnitPrice),
                                OrderCount = g.Count(),
                                FirstSaleDate = g.Min(x => x.o.OrderDate),
                                LastSaleDate = g.Max(x => x.o.OrderDate)
                            };

                // Застосування фільтрів
                if (parameters.CategoryId.HasValue)
                {
                    query = query.Where(r => _context.Products.Any(p => p.ProductID == r.ProductID && p.CategoryID == parameters.CategoryId));
                }

                if (!parameters.IncludeInactive)
                {
                    query = query.Where(r => _context.Products.Any(p => p.ProductID == r.ProductID && p.IsActive));
                }

                var result = await query
                    .Where(r => r.TotalQuantitySold >= parameters.MinQuantitySold)
                    .OrderByDescending(r => r.TotalRevenue)
                    .Take(parameters.TopCount)
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка отримання звіту про продажі: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Отримати звіт про продажі тварин за період
        /// </summary>
        public async Task<List<AnimalSalesReport>> GetAnimalSalesAsync(ReportParameters parameters)
        {
            try
            {
                var query = from od in _context.OrderDetails
                            join o in _context.Orders on od.OrderID equals o.OrderID
                            join a in _context.Animals on od.AnimalID equals a.AnimalID
                            where o.OrderDate >= parameters.StartDate
                                  && o.OrderDate <= parameters.EndDate
                                  && od.AnimalID.HasValue
                            select new AnimalSalesReport
                            {
                                AnimalID = a.AnimalID,
                                Name = a.Name,
                                Species = a.Species,
                                Breed = a.Breed,
                                SalePrice = od.UnitPrice,
                                SaleDate = o.OrderDate,
                                CustomerName = o.CustomerName,
                                AgeMonths = a.AgeMonths
                            };

                return await query
                    .OrderByDescending(r => r.SaleDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка отримання звіту про продажі тварин: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Експортувати звіт про продажі товарів в CSV
        /// </summary>
        public async Task<string> ExportTopProductsToCsvAsync(ReportParameters parameters)
        {
            try
            {
                var data = await GetTopSellingProductsAsync(parameters);
                return GenerateProductsCsv(data, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка експорту звіту в CSV: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Експортувати звіт про продажі тварин в CSV
        /// </summary>
        public async Task<string> ExportAnimalSalesToCsvAsync(ReportParameters parameters)
        {
            try
            {
                var data = await GetAnimalSalesAsync(parameters);
                return GenerateAnimalsCsv(data, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка експорту звіту про тварин в CSV: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Згенерувати CSV для товарів
        /// </summary>
        private string GenerateProductsCsv(List<SalesReport> data, ReportParameters parameters)
        {
            var csv = new StringBuilder();

            // Заголовок звіту
            csv.AppendLine($"ЗВІТ ПРО НАЙБІЛЬШ ПРОДАВАНІ ТОВАРИ");
            csv.AppendLine($"Період: {parameters.StartDate:dd.MM.yyyy} - {parameters.EndDate:dd.MM.yyyy}");
            csv.AppendLine($"Дата генерації: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            csv.AppendLine($"Топ-{parameters.TopCount} товарів");
            csv.AppendLine();

            // Заголовки таблиці
            csv.AppendLine("ID;Назва товару;Категорія;Постачальник;Кількість продано;Загальний дохід;Середня ціна;Кількість замовлень;Перша покупка;Остання покупка");

            // Дані
            foreach (var item in data)
            {
                csv.AppendLine($"{item.ProductID};\"{item.ProductName}\";\"{item.CategoryName}\";\"{item.SupplierName}\";{item.TotalQuantitySold};{item.TotalRevenue:F2};{item.AveragePrice:F2};{item.OrderCount};{item.FirstSaleDate:dd.MM.yyyy};{item.LastSaleDate:dd.MM.yyyy}");
            }

            // Підсумок
            csv.AppendLine();
            csv.AppendLine("ПІДСУМКИ:");
            csv.AppendLine($"Загальна кількість товарів у звіті: {data.Count}");
            csv.AppendLine($"Загальна кількість проданих одиниць: {data.Sum(r => r.TotalQuantitySold)}");
            csv.AppendLine($"Загальний дохід: {data.Sum(r => r.TotalRevenue):F2} грн");
            csv.AppendLine($"Середній дохід на товар: {data.Average(r => r.TotalRevenue):F2} грн");

            return csv.ToString();
        }

        /// <summary>
        /// Згенерувати CSV для тварин
        /// </summary>
        private string GenerateAnimalsCsv(List<AnimalSalesReport> data, ReportParameters parameters)
        {
            var csv = new StringBuilder();

            // Заголовок звіту
            csv.AppendLine($"ЗВІТ ПРО ПРОДАЖІ ТВАРИН");
            csv.AppendLine($"Період: {parameters.StartDate:dd.MM.yyyy} - {parameters.EndDate:dd.MM.yyyy}");
            csv.AppendLine($"Дата генерації: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            csv.AppendLine();

            // Заголовки таблиці
            csv.AppendLine("ID;Ім'я тварини;Вид;Порода;Ціна;Дата продажу;Клієнт;Вік (місяці)");

            // Дані
            foreach (var item in data)
            {
                csv.AppendLine($"{item.AnimalID};\"{item.Name}\";\"{item.Species}\";\"{item.Breed}\";{item.SalePrice:F2};{item.SaleDate:dd.MM.yyyy HH:mm};\"{item.CustomerName}\";{item.AgeMonths}");
            }

            // Підсумок
            csv.AppendLine();
            csv.AppendLine("ПІДСУМКИ:");
            csv.AppendLine($"Загальна кількість проданих тварин: {data.Count}");
            csv.AppendLine($"Загальний дохід: {data.Sum(r => r.SalePrice):F2} грн");
            csv.AppendLine($"Середня ціна: {data.Average(r => r.SalePrice):F2} грн");

            return csv.ToString();
        }

        /// <summary>
        /// Зберегти CSV у файл
        /// </summary>
        public async Task SaveCsvToFileAsync(string csvContent, string fileName)
        {
            try
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var reportsFolder = Path.Combine(documentsPath, "PetStoreReports");

                if (!Directory.Exists(reportsFolder))
                {
                    Directory.CreateDirectory(reportsFolder);
                }

                var filePath = Path.Combine(reportsFolder, fileName);
                await File.WriteAllTextAsync(filePath, csvContent, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка збереження файлу: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Отримати статистику продажів за період
        /// </summary>
        public async Task<(int totalOrders, decimal totalRevenue, int totalItems)> GetSalesStatisticsAsync(ReportParameters parameters)
        {
            try
            {
                var ordersQuery = _context.Orders
                    .Where(o => o.OrderDate >= parameters.StartDate && o.OrderDate <= parameters.EndDate);

                var totalOrders = await ordersQuery.CountAsync();
                var totalRevenue = await ordersQuery.SumAsync(o => o.TotalAmount);

                var totalItems = await _context.OrderDetails
                    .Join(_context.Orders, od => od.OrderID, o => o.OrderID, (od, o) => new { od, o })
                    .Where(x => x.o.OrderDate >= parameters.StartDate && x.o.OrderDate <= parameters.EndDate)
                    .SumAsync(x => x.od.Quantity);

                return (totalOrders, totalRevenue, totalItems);
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка отримання статистики: {ex.Message}", ex);
            }
        }
    }
}
