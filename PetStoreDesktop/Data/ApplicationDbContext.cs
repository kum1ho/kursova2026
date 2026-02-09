using Microsoft.EntityFrameworkCore;
using PetStoreDesktop.Models;

namespace PetStoreDesktop.Data
{
    /// <summary>
    /// Контекст бази даних для десктопного додатку
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PetStoreDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true");
            }
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Animal> Animals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Налаштування для Products
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductID);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Weight).HasColumnType("decimal(8,3)");
                entity.Property(e => e.Unit).HasMaxLength(50);
                entity.Property(e => e.Dimensions).HasMaxLength(100);
                entity.Property(e => e.SKU).HasMaxLength(100);
                entity.Property(e => e.DateAdded).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.SKU).IsUnique();

                // Внешні ключі
                entity.HasOne(d => d.Category)
                    .WithMany()
                    .HasForeignKey(d => d.CategoryID)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Supplier)
                    .WithMany()
                    .HasForeignKey(d => d.SupplierID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Налаштування для Categories
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryID);
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.CategoryName).IsUnique();
            });

            // Налаштування для Suppliers
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.SupplierID);
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.ContactPerson).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(300);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.Country).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Налаштування для Orders
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderID);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.CustomerPhone).HasMaxLength(20);
                entity.Property(e => e.CustomerEmail).HasMaxLength(100);
                entity.Property(e => e.CustomerAddress).HasMaxLength(500);
                entity.Property(e => e.OrderDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
            });

            // Налаштування для OrderDetails
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.OrderDetailID);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Discount).HasColumnType("decimal(5,2)");

                entity.HasOne(d => d.Order)
                    .WithMany(o => o.OrderDetails)
                    .HasForeignKey(d => d.OrderID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductID)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Animal)
                    .WithMany()
                    .HasForeignKey(d => d.AnimalID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Налаштування для Animals
            modelBuilder.Entity<Animal>(entity =>
            {
                entity.HasKey(e => e.AnimalID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Species).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Breed).HasMaxLength(100);
                entity.Property(e => e.Gender).HasMaxLength(10);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.HealthStatus).HasMaxLength(500);
                entity.Property(e => e.DateAdded).HasDefaultValueSql("GETDATE()");

                entity.HasOne(d => d.Category)
                    .WithMany()
                    .HasForeignKey(d => d.CategoryID)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Supplier)
                    .WithMany()
                    .HasForeignKey(d => d.SupplierID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Додавання початкових даних
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Початкові категорії
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryID = 1, CategoryName = "Собаки", Description = "Все для собак: корм, іграшки, аксесуари" },
                new Category { CategoryID = 2, CategoryName = "Коти", Description = "Товари для котів та кошенят" },
                new Category { CategoryID = 3, CategoryName = "Птахи", Description = "Все для домашніх птахів" },
                new Category { CategoryID = 4, CategoryName = "Рибки", Description = "Акваріуми, рибки та аксесуари" },
                new Category { CategoryID = 5, CategoryName = "Гризуни", Description = "Товари для хом'яків, кроликів та інших гризунів" },
                new Category { CategoryID = 6, CategoryName = "Рептилії", Description = "Товари для рептилій та амфібій" },
                new Category { CategoryID = 7, CategoryName = "Живі тварини", Description = "Продаж живих тварин" },
                new Category { CategoryID = 8, CategoryName = "Корм", Description = "Корм для всіх видів домашніх тварин" },
                new Category { CategoryID = 9, CategoryName = "Іграшки", Description = "Іграшки та розважальні аксесуари" },
                new Category { CategoryID = 10, CategoryName = "Медицина", Description = "Ветеринарні препарати та засоби гігієни" }
            );

            // Початкові постачальники
            modelBuilder.Entity<Supplier>().HasData(
                new Supplier { SupplierID = 1, CompanyName = "PetFood Ltd", ContactPerson = "Іван Петренко", Phone = "+380441234567", Email = "info@petfood.com", City = "Київ", Country = "Україна" },
                new Supplier { SupplierID = 2, CompanyName = "Animal World", ContactPerson = "Марія Ковальчук", Phone = "+380507654321", Email = "sales@animalworld.com", City = "Київ", Country = "Україна" },
                new Supplier { SupplierID = 3, CompanyName = "Bird Paradise", ContactPerson = "Олександр Сидоренко", Phone = "+380678901234", Email = "contact@birdparadice.com", City = "Львів", Country = "Україна" }
            );

            // Початкові товари
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductID = 1, ProductName = "Сухий корм для собак", Description = "Преміум корм для дорослих собак великих порід", Price = 450.00m, QuantityInStock = 50, Unit = "кг", Weight = 15.000m, SKU = "DOG-FOOD-001", CategoryID = 8, SupplierID = 1 },
                new Product { ProductID = 2, ProductName = "Сухий корм для котів", Description = "Балансований корм для дорослих котів", Price = 380.00m, QuantityInStock = 75, Unit = "кг", Weight = 10.000m, SKU = "CAT-FOOD-001", CategoryID = 8, SupplierID = 1 },
                new Product { ProductID = 3, ProductName = "М'ячик для собак", Description = "Резиновий м'ячик для гри", Price = 120.00m, QuantityInStock = 100, Unit = "шт", Weight = 0.150m, SKU = "DOG-TOY-001", CategoryID = 9, SupplierID = 3 },
                new Product { ProductID = 4, ProductName = "Лазерна указка для котів", Description = "Інтерактивна іграшка для котів", Price = 250.00m, QuantityInStock = 60, Unit = "шт", Weight = 0.050m, SKU = "CAT-TOY-001", CategoryID = 9, SupplierID = 3 },
                new Product { ProductID = 5, ProductName = "Клітка для птахів", Description = "Металева клітка для хвилястих папуг", Price = 1200.00m, QuantityInStock = 20, Unit = "шт", Weight = 5.500m, SKU = "BIRD-CAGE-001", CategoryID = 3, SupplierID = 3 },
                new Product { ProductID = 6, ProductName = "Акваріум 50л", Description = "Скляний акваріум з підсвіткою", Price = 2500.00m, QuantityInStock = 15, Unit = "шт", Weight = 25.000m, SKU = "AQUA-50L-001", CategoryID = 4, SupplierID = 3 }
            );
        }
    }
}
