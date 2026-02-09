using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PetStoreDesktop.Data;
using PetStoreDesktop.Models;
using PetStoreDesktop.ViewModels;

namespace PetStoreDesktop.Views
{
    public partial class AnimalEditWindow : Window
    {
        private readonly ApplicationDbContext _context;
        public Animal Animal { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Supplier> Suppliers { get; set; }
        public bool IsSaved { get; private set; }

        public AnimalEditWindow(Animal? animal = null)
        {
            InitializeComponent();

            _context = new ApplicationDbContext();
            Animal = animal ?? new Animal();

            // Завантаження категорій та постачальників
            Categories = new ObservableCollection<Category>(_context.Categories.ToList());
            Suppliers = new ObservableCollection<Supplier>(_context.Suppliers.ToList());

            DataContext = this;

            if (animal?.AnimalID > 0)
            {
                Title = $"Редагування тварини: {animal.Name}";
            }
            else
            {
                Title = "Додавання нової тварини";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валідація
            if (string.IsNullOrWhiteSpace(Animal.Name))
            {
                MessageBox.Show("Введіть ім'я тварини!", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Animal.Species))
            {
                MessageBox.Show("Оберіть вид тварини!", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Animal.Price <= 0)
            {
                MessageBox.Show("Введіть правильну ціну!", "Помилка",
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
