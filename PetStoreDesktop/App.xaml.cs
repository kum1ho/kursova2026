using Microsoft.EntityFrameworkCore;
using PetStoreDesktop.Data;
using System.IO;
using System.Windows;

namespace PetStoreDesktop
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Глобальна обробка помилок
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var ex = (Exception)args.ExceptionObject;
                File.WriteAllText("error.log", $"{DateTime.Now}\n{ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Критична помилка: {ex.Message}\n\nДеталі в файлі error.log",
                    "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            // Ініціалізація бази даних
            try
            {
                using var context = new ApplicationDbContext();
                context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                File.WriteAllText("db_error.log", $"{DateTime.Now}\n{ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Помилка підключення до бази даних: {ex.Message}\n\nДеталі в файлі db_error.log",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}

