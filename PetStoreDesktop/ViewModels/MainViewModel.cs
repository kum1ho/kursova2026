using Microsoft.EntityFrameworkCore;
using PetStoreDesktop.Data;
using PetStoreDesktop.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PetStoreDesktop.ViewModels
{
    /// <summary>
    /// ViewModel для головного вікна
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private ObservableCollection<Product> _products;
        private ObservableCollection<Category> _categories;
        private ObservableCollection<Supplier> _suppliers;
        private Product? _selectedProduct;
        private string _searchText = string.Empty;
        private Category? _selectedCategory;
        private bool _isLoading;

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterProducts();
                }
            }
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    FilterProducts();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Команди
        public RelayCommand RefreshCommand { get; }
        public RelayCommand AddProductCommand { get; }
        public RelayCommand EditProductCommand { get; }
        public RelayCommand DeleteProductCommand { get; }
        public RelayCommand ClearFilterCommand { get; }

        public MainViewModel()
        {
            _context = new ApplicationDbContext();
            _products = new ObservableCollection<Product>();
            _categories = new ObservableCollection<Category>();
            _suppliers = new ObservableCollection<Supplier>();

            // Ініціалізація команд
            RefreshCommand = new RelayCommand(async () => await LoadDataAsync());
            AddProductCommand = new RelayCommand(AddProduct);
            EditProductCommand = new RelayCommand(EditProduct, CanEditProduct);
            DeleteProductCommand = new RelayCommand(DeleteProduct, CanDeleteProduct);
            ClearFilterCommand = new RelayCommand(ClearFilter);

            // Завантаження даних
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                // Завантаження категорій та постачальників
                var categories = await _context.Categories.ToListAsync();
                var suppliers = await _context.Suppliers.ToListAsync();

                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }

                // Додаємо "Всі категорії"
                Categories.Insert(0, new Category { CategoryID = 0, CategoryName = "Всі категорії" });

                Suppliers.Clear();
                foreach (var supplier in suppliers)
                {
                    Suppliers.Add(supplier);
                }

                // Завантаження товарів з навігаційними властивостями
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .ToListAsync();

                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних: {ex.Message}", "Помилка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterProducts()
        {
            try
            {
                var filtered = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .AsEnumerable();

                // Фільтрація за назвою
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    filtered = filtered.Where(p => 
                        p.ProductName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        (p.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));
                }

                // Фільтрація за категорією
                if (SelectedCategory != null && SelectedCategory.CategoryID != 0)
                {
                    filtered = filtered.Where(p => p.CategoryID == SelectedCategory.CategoryID);
                }

                Products.Clear();
                foreach (var product in filtered)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка фільтрації: {ex.Message}", "Помилка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddProduct()
        {
            try
            {
                var editWindow = new Views.ProductEditWindow(_context, new Product(), Categories, Suppliers);
                if (editWindow.ShowDialog() == true)
                {
                    _ = LoadDataAsync(); // Перезавантажити дані
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відкриття вікна редагування: {ex.Message}", "Помилка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditProduct()
        {
            if (SelectedProduct == null) return;

            try
            {
                // Створюємо копію для редагування
                var editProduct = new Product
                {
                    ProductID = SelectedProduct.ProductID,
                    ProductName = SelectedProduct.ProductName,
                    Description = SelectedProduct.Description,
                    Price = SelectedProduct.Price,
                    QuantityInStock = SelectedProduct.QuantityInStock,
                    ReorderLevel = SelectedProduct.ReorderLevel,
                    Unit = SelectedProduct.Unit,
                    Weight = SelectedProduct.Weight,
                    Dimensions = SelectedProduct.Dimensions,
                    SKU = SelectedProduct.SKU,
                    CategoryID = SelectedProduct.CategoryID,
                    SupplierID = SelectedProduct.SupplierID,
                    IsActive = SelectedProduct.IsActive
                };

                var editWindow = new Views.ProductEditWindow(_context, editProduct, Categories, Suppliers);
                if (editWindow.ShowDialog() == true)
                {
                    _ = LoadDataAsync(); // Перезавантажити дані
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відкриття вікна редагування: {ex.Message}", "Помилка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteProduct()
        {
            if (SelectedProduct == null) return;

            var result = MessageBox.Show(
                $"Ви впевнені, що хочете видалити товар '{SelectedProduct.ProductName}'?",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var product = _context.Products.Find(SelectedProduct.ProductID);
                    if (product != null)
                    {
                        // М'яке видалення - деактивуємо товар
                        product.IsActive = false;
                        _context.SaveChanges();
                        _ = LoadDataAsync(); // Перезавантажити дані
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка видалення товару: {ex.Message}", "Помилка", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearFilter()
        {
            SearchText = string.Empty;
            SelectedCategory = Categories.FirstOrDefault(); // "Всі категорії"
        }

        private bool CanEditProduct()
        {
            return SelectedProduct != null;
        }

        private bool CanDeleteProduct()
        {
            return SelectedProduct != null;
        }
    }
}
