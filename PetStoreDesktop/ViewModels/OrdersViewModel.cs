using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using PetStoreDesktop.Data;
using PetStoreDesktop.Models;

namespace PetStoreDesktop.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private ObservableCollection<Order> _orders;
        private Order? _selectedOrder;
        private string _searchText = string.Empty;
        private bool _isLoading;
        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged();
            }
        }

        public Order? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                LoadOrders();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
                LoadOrders();
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
                LoadOrders();
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand UpdateStatusCommand { get; }

        public OrdersViewModel()
        {
            _context = new ApplicationDbContext();
            _orders = new ObservableCollection<Order>();

            RefreshCommand = new RelayCommand(_ => LoadOrders());
            ViewDetailsCommand = new RelayCommand(_ => ViewOrderDetails(), _ => SelectedOrder != null);
            UpdateStatusCommand = new RelayCommand(_ => UpdateOrderStatus(), _ => SelectedOrder != null);

            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                IsLoading = true;
                var query = _context.Orders
                    .Include(o => o.OrderDetails)
                    .Where(o => o.OrderDate >= StartDate && o.OrderDate <= EndDate)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    if (int.TryParse(SearchText, out int orderId))
                    {
                        query = query.Where(o => o.OrderID == orderId);
                    }
                    else
                    {
                        query = query.Where(o => o.Status.Contains(SearchText));
                    }
                }

                Orders = new ObservableCollection<Order>(query.OrderByDescending(o => o.OrderDate).ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження замовлень: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ViewOrderDetails()
        {
            if (SelectedOrder == null) return;

            var details = _context.OrderDetails
                .Where(od => od.OrderID == SelectedOrder.OrderID)
                .Include(od => od.Product)
                .ToList();

            var message = $"Замовлення #{SelectedOrder.OrderID}\n" +
                         $"Дата: {SelectedOrder.OrderDate:dd.MM.yyyy HH:mm}\n" +
                         $"Статус: {SelectedOrder.Status}\n" +
                         $"Сума: {SelectedOrder.TotalAmount:C}\n\n" +
                         $"Товари:\n" +
                         string.Join("\n", details.Select(d =>
                             $"- {d.Product?.ProductName} x{d.Quantity} = {(d.UnitPrice * d.Quantity * (1 - d.Discount)):C}"));

            MessageBox.Show(message, "Деталі замовлення",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateOrderStatus()
        {
            if (SelectedOrder == null) return;

            var statuses = new[] { "Нове", "Обробляється", "Готове", "Виконано", "Скасовано" };
            var currentIndex = Array.IndexOf(statuses, SelectedOrder.Status);
            var nextStatus = statuses[(currentIndex + 1) % statuses.Length];

            var result = MessageBox.Show(
                $"Змінити статус замовлення #{SelectedOrder.OrderID} на '{nextStatus}'?",
                "Оновлення статусу", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedOrder.Status = nextStatus;
                    _context.SaveChanges();
                    LoadOrders();
                    MessageBox.Show("Статус оновлено успішно!", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка оновлення статусу: {ex.Message}", "Помилка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
