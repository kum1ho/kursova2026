using System.Collections.ObjectModel;
using System.Linq;
using PetStoreDesktop.Data;
using PetStoreDesktop.Models;

namespace PetStoreDesktop.ViewModels
{
    public class AnimalEditViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private Animal _animal;

        public Animal Animal
        {
            get => _animal;
            set
            {
                _animal = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Supplier> Suppliers { get; set; }

        public AnimalEditViewModel(Animal? animal = null)
        {
            _context = new ApplicationDbContext();
            _animal = animal ?? new Animal();

            Categories = new ObservableCollection<Category>(_context.Categories.ToList());
            Suppliers = new ObservableCollection<Supplier>(_context.Suppliers.ToList());
        }
    }
}
