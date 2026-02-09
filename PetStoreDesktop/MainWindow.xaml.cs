using System;
using System.Windows;
using PetStoreDesktop.Views;
using PetStoreDesktop.ViewModels;

namespace PetStoreDesktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Ініціалізація ViewModels для кожної вкладки
            try
            {
                DashboardTab.DataContext = new DashboardViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації Дашборду: {ex.Message}",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            try
            {
                ProductsTab.DataContext = new MainViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації Товарів: {ex.Message}",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            try
            {
                AnimalsTab.DataContext = new AnimalsViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації Тварин: {ex.Message}",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            try
            {
                OrdersTab.DataContext = new OrdersViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації Замовлень: {ex.Message}",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            try
            {
                SuppliersTab.DataContext = new SuppliersViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації Постачальників: {ex.Message}",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenReports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reportsWindow = new ReportsWindow();
                reportsWindow.Owner = this;
                reportsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відкриття звітів: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}