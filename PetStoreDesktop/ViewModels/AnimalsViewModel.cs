using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using PetStoreDesktop.Data;
using PetStoreDesktop.Models;
using PetStoreDesktop.Views;

namespace PetStoreDesktop.ViewModels
{
    public class AnimalsViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private ObservableCollection<Animal> _animals;
        private Animal? _selectedAnimal;
        private string _searchText = string.Empty;
        private bool _isLoading;

        public ObservableCollection<Animal> Animals
        {
            get => _animals;
            set
            {
                _animals = value;
                OnPropertyChanged();
            }
        }

        public Animal? SelectedAnimal
        {
            get => _selectedAnimal;
            set
            {
                _selectedAnimal = value;
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
                LoadAnimals();
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
        public ICommand AddAnimalCommand { get; }
        public ICommand EditAnimalCommand { get; }
        public ICommand DeleteAnimalCommand { get; }

        public AnimalsViewModel()
        {
            _context = new ApplicationDbContext();
            _animals = new ObservableCollection<Animal>();

            RefreshCommand = new RelayCommand(_ => LoadAnimals(), (Predicate<object?>?)null);
            AddAnimalCommand = new RelayCommand(_ => AddAnimal(), (Predicate<object?>?)null);
            EditAnimalCommand = new RelayCommand(_ => EditAnimal(), (Predicate<object?>?)(_ => SelectedAnimal != null));
            DeleteAnimalCommand = new RelayCommand(_ => DeleteAnimal(), (Predicate<object?>?)(_ => SelectedAnimal != null));

            LoadAnimals();
        }

        private void LoadAnimals()
        {
            try
            {
                IsLoading = true;
                var query = _context.Animals.AsQueryable();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(a => a.Name.Contains(SearchText) ||
                                           a.Species.Contains(SearchText));
                }

                Animals = new ObservableCollection<Animal>(query.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження тварин: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddAnimal()
        {
            try
            {
                var newAnimal = new Animal
                {
                    Name = "Нова тварина",
                    Species = "Кіт",
                    AgeMonths = 12,
                    Price = 1000,
                    DateAdded = DateTime.Now,
                    IsVaccinated = false
                };

                var editWindow = new AnimalEditWindow(newAnimal);
                var result = editWindow.ShowDialog();

                if (result == true)
                {
                    _context.Animals.Add(newAnimal);
                    _context.SaveChanges();
                    LoadAnimals();
                    MessageBox.Show("Тварину додано успішно!", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}\n\nStack trace: {ex.StackTrace}", "Помилка відкриття вікна",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditAnimal()
        {
            if (SelectedAnimal == null) return;

            try
            {
                var editWindow = new AnimalEditWindow(SelectedAnimal);
                var result = editWindow.ShowDialog();

                if (result == true)
                {
                    _context.SaveChanges();
                    LoadAnimals();
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

        private void DeleteAnimal()
        {
            if (SelectedAnimal == null) return;

            var result = MessageBox.Show($"Ви впевнені, що хочете видалити {SelectedAnimal.Name}?",
                "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Animals.Remove(SelectedAnimal);
                    _context.SaveChanges();
                    LoadAnimals();
                    MessageBox.Show("Тварину видалено успішно!", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка видалення тварини: {ex.Message}", "Помилка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
