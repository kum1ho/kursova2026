using Microsoft.EntityFrameworkCore;
using PetStoreDesktop.Data;
using PetStoreDesktop.Models;
using PetStoreDesktop.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;

namespace PetStoreDesktop.ViewModels
{
    /// <summary>
    /// ViewModel для вікна звітів
    /// </summary>
    public class ReportsViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ReportService _reportService;

        private ObservableCollection<SalesReport> _topProducts;
        private ObservableCollection<AnimalSalesReport> _animalSales;
        private ObservableCollection<Category> _categories;
        private ReportParameters _parameters;
        private bool _isLoading;
        private string _statusMessage = string.Empty;
        private string _statisticsText = string.Empty;

        public ObservableCollection<SalesReport> TopProducts
        {
            get => _topProducts;
            set => SetProperty(ref _topProducts, value);
        }

        public ObservableCollection<AnimalSalesReport> AnimalSales
        {
            get => _animalSales;
            set => SetProperty(ref _animalSales, value);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ReportParameters Parameters
        {
            get => _parameters;
            set => SetProperty(ref _parameters, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public string StatisticsText
        {
            get => _statisticsText;
            set => SetProperty(ref _statisticsText, value);
        }

        // Команди
        public RelayCommand GenerateReportCommand { get; }
        public RelayCommand ExportProductsCsvCommand { get; }
        public RelayCommand ExportAnimalsCsvCommand { get; }
        public RelayCommand PrintReportCommand { get; }
        public RelayCommand RefreshCommand { get; }
        public RelayCommand ResetParametersCommand { get; }

        public ReportsViewModel()
        {
            _context = new ApplicationDbContext();
            _reportService = new ReportService(_context);

            _topProducts = new ObservableCollection<SalesReport>();
            _animalSales = new ObservableCollection<AnimalSalesReport>();
            _categories = new ObservableCollection<Category>();
            _parameters = new ReportParameters();

            // Ініціалізація команд
            GenerateReportCommand = new RelayCommand(async () => await GenerateReportAsync());
            ExportProductsCsvCommand = new RelayCommand(async () => await ExportProductsCsvAsync(), CanExport);
            ExportAnimalsCsvCommand = new RelayCommand(async () => await ExportAnimalsCsvAsync(), CanExport);
            PrintReportCommand = new RelayCommand(PrintReport, CanPrint);
            RefreshCommand = new RelayCommand(async () => await LoadDataAsync());
            ResetParametersCommand = new RelayCommand(ResetParameters);

            // Завантаження даних
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Завантаження даних...";

                // Завантаження категорій
                var categories = await _context.Categories.ToListAsync();
                Categories.Clear();
                Categories.Add(new Category { CategoryID = 0, CategoryName = "Всі категорії" });
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }

                // Автоматична генерація звіту за замовчуванням
                await GenerateReportAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Помилка завантаження: {ex.Message}";
                MessageBox.Show($"Помилка завантаження даних: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task GenerateReportAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Генерація звіту...";

                // Генерація звіту про топ товари
                var topProducts = await _reportService.GetTopSellingProductsAsync(Parameters);
                TopProducts.Clear();
                foreach (var product in topProducts)
                {
                    TopProducts.Add(product);
                }

                // Генерація звіту про продажі тварин
                var animalSales = await _reportService.GetAnimalSalesAsync(Parameters);
                AnimalSales.Clear();
                foreach (var animal in animalSales)
                {
                    AnimalSales.Add(animal);
                }

                // Отримання статистики
                var (totalOrders, totalRevenue, totalItems) = await _reportService.GetSalesStatisticsAsync(Parameters);
                StatisticsText = $"Період: {Parameters.StartDate:dd.MM.yyyy} - {Parameters.EndDate:dd.MM.yyyy}\n" +
                                $"Замовлень: {totalOrders} | Товарів: {totalItems} | Дохід: {totalRevenue:F2} грн";

                StatusMessage = $"Звіт успішно згенеровано. Топ товарів: {TopProducts.Count}, Продано тварин: {AnimalSales.Count}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Помилка генерації звіту: {ex.Message}";
                MessageBox.Show($"Помилка генерації звіту: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExportProductsCsvAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Експорт звіту про товари...";

                var csvContent = await _reportService.ExportTopProductsToCsvAsync(Parameters);
                var fileName = $"TopProducts_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                await _reportService.SaveCsvToFileAsync(csvContent, fileName);

                StatusMessage = $"Звіт збережено: {fileName}";
                MessageBox.Show($"Звіт успішно збережено у папці Документи\\PetStoreReports\\{fileName}",
                    "Експорт завершено", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Помилка експорту: {ex.Message}";
                MessageBox.Show($"Помилка експорту звіту: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExportAnimalsCsvAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Експорт звіту про тварин...";

                var csvContent = await _reportService.ExportAnimalSalesToCsvAsync(Parameters);
                var fileName = $"AnimalSales_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                await _reportService.SaveCsvToFileAsync(csvContent, fileName);

                StatusMessage = $"Звіт збережено: {fileName}";
                MessageBox.Show($"Звіт успішно збережено у папці Документи\\PetStoreReports\\{fileName}",
                    "Експорт завершено", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Помилка експорту: {ex.Message}";
                MessageBox.Show($"Помилка експорту звіту: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void PrintReport()
        {
            try
            {
                // Створення текстового звіту для друку
                var reportText = GeneratePrintableReport();

                // Збереження у тимчасовий файл
                var tempFile = Path.GetTempFileName();
                File.WriteAllText(tempFile, reportText, Encoding.UTF8);

                // Відкриття файлу для друку
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = tempFile,
                        UseShellExecute = true
                    }
                };
                process.Start();

                StatusMessage = "Звіт відкрито для друку";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Помилка друку: {ex.Message}";
                MessageBox.Show($"Помилка підготовки звіту до друку: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GeneratePrintableReport()
        {
            var report = new StringBuilder();

            report.AppendLine("ЗВІТ ПРО ПРОДАЖІ ЗООМАГАЗИНУ");
            report.AppendLine("=" + new string('=', 50));
            report.AppendLine();
            report.AppendLine($"Період: {Parameters.StartDate:dd.MM.yyyy} - {Parameters.EndDate:dd.MM.yyyy}");
            report.AppendLine($"Дата генерації: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            report.AppendLine();
            report.AppendLine(StatisticsText);
            report.AppendLine();

            // Звіт про топ товари
            report.AppendLine("ТОП-" + Parameters.TopCount + " НАЙБІЛЬШ ПРОДАВАНИХ ТОВАРІВ");
            report.AppendLine("-" + new string('-', 50));
            report.AppendLine();
            report.AppendLine($"{"№",-3} {"Назва",-30} {"Категорія",-15} {"Кількість",-8} {"Дохід",-12} {"Ціна",-10}");
            report.AppendLine(new string('-', 80));

            for (int i = 0; i < TopProducts.Count; i++)
            {
                var item = TopProducts[i];
                report.AppendLine($"{i + 1,-3} {item.ProductName,-30} {item.CategoryName,-15} {item.TotalQuantitySold,-8} {item.TotalRevenue,-12:F2} {item.AveragePrice,-10:F2}");
            }

            // Звіт про продажі тварин
            if (AnimalSales.Any())
            {
                report.AppendLine();
                report.AppendLine("ПРОДАНІ ТВАРИНИ");
                report.AppendLine("-" + new string('-', 50));
                report.AppendLine();
                report.AppendLine($"{"№",-3} {"Ім'я",-15} {"Від",-10} {"Ціна",-10} {"Дата",-12} {"Клієнт",-20}");
                report.AppendLine(new string('-', 70));

                for (int i = 0; i < Math.Min(AnimalSales.Count, 20); i++) // Обмежуємо для друку
                {
                    var item = AnimalSales[i];
                    report.AppendLine($"{i + 1,-3} {item.Name,-15} {item.Species,-10} {item.SalePrice,-10:F2} {item.SaleDate:dd.MM,-12} {item.CustomerName,-20}");
                }

                if (AnimalSales.Count > 20)
                {
                    report.AppendLine($"... та ще {AnimalSales.Count - 20} записів");
                }
            }

            report.AppendLine();
            report.AppendLine("=" + new string('=', 50));
            report.AppendLine("Кінець звіту");

            return report.ToString();
        }

        private void ResetParameters()
        {
            Parameters = new ReportParameters();
            StatusMessage = "Параметри скинуто. Натисніть 'Генерувати' для оновлення звіту.";
        }

        private bool CanExport()
        {
            return TopProducts.Any() || AnimalSales.Any();
        }

        private bool CanPrint()
        {
            return TopProducts.Any() || AnimalSales.Any();
        }
    }
}
