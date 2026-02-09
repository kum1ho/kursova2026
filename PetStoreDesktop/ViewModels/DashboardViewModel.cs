using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using PetStoreDesktop.Data;
using PetStoreDesktop.Models;

namespace PetStoreDesktop.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private int _totalProducts;
        private int _totalAnimals;
        private int _totalOrders;
        private decimal _totalRevenue;
        private int _lowStockCount;
        private string _topProduct = string.Empty;
        private ObservableCollection<Product> _lowStockProducts;
        private ObservableCollection<Order> _recentOrders;

        public int TotalProducts
        {
            get => _totalProducts;
            set
            {
                _totalProducts = value;
                OnPropertyChanged();
            }
        }

        public int TotalAnimals
        {
            get => _totalAnimals;
            set
            {
                _totalAnimals = value;
                OnPropertyChanged();
            }
        }

        public int TotalOrders
        {
            get => _totalOrders;
            set
            {
                _totalOrders = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set
            {
                _totalRevenue = value;
                OnPropertyChanged();
            }
        }

        public int LowStockCount
        {
            get => _lowStockCount;
            set
            {
                _lowStockCount = value;
                OnPropertyChanged();
            }
        }

        public string TopProduct
        {
            get => _topProduct;
            set
            {
                _topProduct = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Product> LowStockProducts
        {
            get => _lowStockProducts;
            set
            {
                _lowStockProducts = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Order> RecentOrders
        {
            get => _recentOrders;
            set
            {
                _recentOrders = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }

        public DashboardViewModel()
        {
            _context = new ApplicationDbContext();
            _lowStockProducts = new ObservableCollection<Product>();
            _recentOrders = new ObservableCollection<Order>();

            RefreshCommand = new RelayCommand(_ => LoadDashboardData());

            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                // Загальна статистика
                TotalProducts = _context.Products.Count();
                TotalAnimals = _context.Animals.Count();
                TotalOrders = _context.Orders.Count();
                TotalRevenue = _context.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0;

                // Товари з низьким залишком
                var lowStock = _context.Products
                    .Where(p => p.QuantityInStock < 10)
                    .OrderBy(p => p.QuantityInStock)
                    .Take(5)
                    .ToList();
                LowStockProducts = new ObservableCollection<Product>(lowStock);
                LowStockCount = lowStock.Count;

                // Останні замовлення
                var recent = _context.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToList();
                RecentOrders = new ObservableCollection<Order>(recent);

                // Найпопулярніший товар
                var topProductData = _context.OrderDetails
                    .GroupBy(od => od.Product!.ProductName)
                    .Select(g => new { ProductName = g.Key, TotalSold = g.Sum(od => od.Quantity) })
                    .OrderByDescending(x => x.TotalSold)
                    .FirstOrDefault();

                TopProduct = topProductData != null
                    ? $"{topProductData.ProductName} ({topProductData.TotalSold} шт.)"
                    : "Немає даних";
            }
            catch (Exception)
            {
                // Ігноруємо помилки при завантаженні дашборду
            }
        }
    }
}
