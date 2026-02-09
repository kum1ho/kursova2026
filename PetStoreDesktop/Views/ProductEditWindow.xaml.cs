using Microsoft.EntityFrameworkCore;
using PetStoreDesktop.Data;
using PetStoreDesktop.Models;
using PetStoreDesktop.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace PetStoreDesktop.Views
{
    public partial class ProductEditWindow : Window
    {
        public ProductEditWindow()
        {
            InitializeComponent();
        }

        public ProductEditWindow(ApplicationDbContext context, Product product, 
            ObservableCollection<Category> categories, ObservableCollection<Supplier> suppliers)
        {
            InitializeComponent();
            
            var viewModel = new ProductEditViewModel(context, product, categories, suppliers);
            DataContext = viewModel;
        }
    }
}
