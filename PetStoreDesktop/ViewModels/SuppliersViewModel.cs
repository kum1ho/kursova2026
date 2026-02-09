using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PetStoreDesktop.Data;
using PetStoreDesktop.Models;
using PetStoreDesktop.Views;

namespace PetStoreDesktop.ViewModels
{
    public class SuppliersViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private ObservableCollection<Supplier> _suppliers;
        private Supplier? _selectedSupplier;
        private string _searchText = string.Empty;
        private bool _isLoading;

        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set
            {
                _suppliers = value;
                OnPropertyChanged();
            }
        }

        public Supplier? SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                _selectedSupplier = value;
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
                LoadSuppliers();
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

        public ICommand RefreshCommand { get; }
        public ICommand AddSupplierCommand { get; }
        public ICommand EditSupplierCommand { get; }
        public ICommand DeleteSupplierCommand { get; }

        public SuppliersViewModel()
        {
            _context = new ApplicationDbContext();
            _suppliers = new ObservableCollection<Supplier>();

            RefreshCommand = new RelayCommand(_ => LoadSuppliers());
            AddSupplierCommand = new RelayCommand(_ => AddSupplier());
            EditSupplierCommand = new RelayCommand(_ => EditSupplier(), _ => SelectedSupplier != null);
            DeleteSupplierCommand = new RelayCommand(_ => DeleteSupplier(), _ => SelectedSupplier != null);

            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            try
            {
                IsLoading = true;
                var query = _context.Suppliers.AsQueryable();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(s => s.CompanyName.Contains(SearchText) ||
                                           (s.ContactPerson != null && s.ContactPerson.Contains(SearchText)));
                }

                Suppliers = new ObservableCollection<Supplier>(query.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження постачальників: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddSupplier()
        {
            try
            {
                var newSupplier = new Supplier
                {
                    CompanyName = "Нова компанія",
                    ContactPerson = "Контактна особа",
                    Phone = "+380000000000",
                    Country = "Україна"
                };

                var editWindow = new SupplierEditWindow(newSupplier);
                var result = editWindow.ShowDialog();

                if (result == true)
                {
                    _context.Suppliers.Add(newSupplier);
                    _context.SaveChanges();
                    LoadSuppliers();
                    MessageBox.Show("Постачальника додано успішно!", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}\n\nStack trace: {ex.StackTrace}", "Помилка відкриття вікна",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditSupplier()
        {
            if (SelectedSupplier == null) return;

            try
            {
                var editWindow = new SupplierEditWindow(SelectedSupplier);
                var result = editWindow.ShowDialog();

                if (result == true)
                {
                    _context.SaveChanges();
                    LoadSuppliers();
                    MessageBox.Show("Зміни збережено успішно!", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}\n\nStack trace: {ex.StackTrace}", "Помилка відкриття вікна",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteSupplier()
        {
            if (SelectedSupplier == null) return;

            var result = MessageBox.Show(
                $"Ви впевнені, що хочете видалити постачальника '{SelectedSupplier.CompanyName}'?",
                "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Suppliers.Remove(SelectedSupplier);
                    _context.SaveChanges();
                    LoadSuppliers();
                    MessageBox.Show("Постачальника видалено успішно!", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка видалення постачальника: {ex.Message}", "Помилка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
