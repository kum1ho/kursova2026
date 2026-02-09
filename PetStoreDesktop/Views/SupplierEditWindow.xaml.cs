using System.Windows;
using PetStoreDesktop.Models;

namespace PetStoreDesktop.Views
{
    public partial class SupplierEditWindow : Window
    {
        public Supplier Supplier { get; set; }
        public bool IsSaved { get; private set; }

        public SupplierEditWindow(Supplier? supplier = null)
        {
            InitializeComponent();

            Supplier = supplier ?? new Supplier();
            DataContext = this;

            if (supplier?.SupplierID > 0)
            {
                Title = $"Редагування постачальника: {supplier.CompanyName}";
            }
            else
            {
                Title = "Додавання нового постачальника";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валідація
            if (string.IsNullOrWhiteSpace(Supplier.CompanyName))
            {
                MessageBox.Show("Введіть назву компанії!", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Supplier.ContactPerson))
            {
                MessageBox.Show("Введіть контактну особу!", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsSaved = true;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsSaved = false;
            DialogResult = false;
            Close();
        }
    }
}
