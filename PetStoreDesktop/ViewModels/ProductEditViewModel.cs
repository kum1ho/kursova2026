using Microsoft.EntityFrameworkCore;
using PetStoreDesktop.Data;
using PetStoreDesktop.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PetStoreDesktop.ViewModels
{
    /// <summary>
    /// ViewModel для вікна редагування товару
    /// </summary>
    public class ProductEditViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private Product _product;
        private ObservableCollection<Category> _categories;
        private ObservableCollection<Supplier> _suppliers;
        private string _errorMessage = string.Empty;

        public Product Product
        {
            get => _product;
            set => SetProperty(ref _product, value);
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

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        // Команди
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public ProductEditViewModel(ApplicationDbContext context, Product product, 
            ObservableCollection<Category> categories, ObservableCollection<Supplier> suppliers)
        {
            _context = context;
            _product = product;
            _categories = categories;
            _suppliers = suppliers;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool CanSave()
        {
            return ValidateProduct();
        }

        private void Save()
        {
            try
            {
                if (!ValidateProduct())
                    return;

                if (_product.ProductID == 0)
                {
                    // Новий товар
                    _context.Products.Add(_product);
                }
                else
                {
                    // Існуючий товар
                    _context.Entry(_product).State = EntityState.Modified;
                }

                _context.SaveChanges();

                // Закриваємо вікно з результатом true
                if (System.Windows.Application.Current.Windows.OfType<Views.ProductEditWindow>().FirstOrDefault() is Views.ProductEditWindow window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Помилка збереження: {ex.Message}";
            }
        }

        private void Cancel()
        {
            // Закриваємо вікно з результатом false
            if (System.Windows.Application.Current.Windows.OfType<Views.ProductEditWindow>().FirstOrDefault() is Views.ProductEditWindow window)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

        private bool ValidateProduct()
        {
            var validationContext = new ValidationContext(_product);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(_product, validationContext, validationResults, true);

            if (!isValid)
            {
                ErrorMessage = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                return false;
            }

            // Додаткові перевірки
            if (_product.Price <= 0)
            {
                ErrorMessage = "Ціна повинна бути більше 0";
                return false;
            }

            if (_product.QuantityInStock < 0)
            {
                ErrorMessage = "Кількість не може бути від'ємною";
                return false;
            }

            // Перевірка унікальності SKU (якщо вказано)
            if (!string.IsNullOrWhiteSpace(_product.SKU))
            {
                var existingProduct = _context.Products
                    .FirstOrDefault(p => p.SKU == _product.SKU && p.ProductID != _product.ProductID);
                
                if (existingProduct != null)
                {
                    ErrorMessage = "Товар з таким SKU вже існує";
                    return false;
                }
            }

            ErrorMessage = string.Empty;
            return true;
        }
    }
}
