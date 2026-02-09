using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Models;

namespace PetStoreAPI.Data
{
    /// <summary>
    /// Контекст бази даних для зоомагазину
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets для всіх таблиць
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryID)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.SupplierID)
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
                entity.Property(e => e.Color).HasMaxLength(50);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Weight).HasColumnType("decimal(5,2)");
                entity.Property(e => e.DateAdded).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Description).HasMaxLength(1000);

                // Внешні ключі
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Animals)
                    .HasForeignKey(d => d.CategoryID)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Animals)
                    .HasForeignKey(d => d.SupplierID)
                    .OnDelete(DeleteBehavior.SetNull);
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
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(12,2)").HasDefaultValue(0);
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Нове");
                entity.Property(e => e.PaymentMethod).HasMaxLength(50);
                entity.Property(e => e.PaymentStatus).HasMaxLength(50).HasDefaultValue("Не оплачено");
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Налаштування для OrderDetails
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.OrderDetailID);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Discount).HasColumnType("decimal(5,2)").HasDefaultValue(0);

                // Внешні ключі
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductID)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Animal)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.AnimalID)
                    .OnDelete(DeleteBehavior.SetNull);

                // Перевірка: хоча б один з ProductID або AnimalID повинен бути вказаний
                entity.HasCheckConstraint("CHK_OrderDetails_ItemType", 
                    "(ProductID IS NOT NULL AND AnimalID IS NULL) OR (ProductID IS NULL AND AnimalID IS NOT NULL)");
            });

            // Налаштування для Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.PasswordResetToken).HasMaxLength(255);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Налаштування для Roles
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.RoleName).IsUnique();
            });

            // Налаштування для UserRoles
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.UserRoleId);
                entity.Property(e => e.AssignedAt).HasDefaultValueSql("GETDATE()");
                
                // Внешні ключі
                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Унікальність комбінації користувач-роль
                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
            });

            // Налаштування для UserSessions
            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.HasKey(e => e.SessionId);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.RefreshToken).HasMaxLength(500);
                entity.Property(e => e.IpAddress).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.LastActivityAt).HasDefaultValueSql("GETDATE()");
                
                // Внешні ключі
                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserSessions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Додавання початкових даних (seed data)
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Додавання початкових даних
        /// </summary>
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
                new Product { ProductID = 4, ProductName = "Лазерна указка для котів", Description = "Інтерактивна іграшка для котів", Price = 250.00m, QuantityInStock = 60, Unit = "шт", Weight = 0.050m, SKU = "CAT-TOY-001", CategoryID = 9, SupplierID = 3 }
            );

            // Початкові тварини
            modelBuilder.Entity<Animal>().HasData(
                new Animal { AnimalID = 1, Name = "Рекс", Species = "Собака", Breed = "Німецька вівчарка", AgeMonths = 12, Gender = "Самець", Color = "Чорно-рудий", Weight = 25.50m, Price = 15000.00m, DateOfBirth = new DateTime(2024, 1, 20), CategoryID = 7, SupplierID = 1, Description = "Дружня та активна собака, добре дресирована" },
                new Animal { AnimalID = 2, Name = "Мурка", Species = "Кіт", Breed = "Британська короткошерста", AgeMonths = 6, Gender = "Самиця", Color = "Сіра", Weight = 3.20m, Price = 8000.00m, DateOfBirth = new DateTime(2024, 7, 20), CategoryID = 7, SupplierID = 2, Description = "Спокійна кішка, любить гратися" }
            );

            // Початкові ролі
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin", Description = "Адміністратор системи", IsActive = true },
                new Role { RoleId = 2, RoleName = "Manager", Description = "Менеджер магазину", IsActive = true },
                new Role { RoleId = 3, RoleName = "User", Description = "Звичайний користувач", IsActive = true }
            );

            // Початковий адміністратор (пароль: Admin123!)
            var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    UserId = 1, 
                    Username = "admin", 
                    Email = "admin@petstore.com", 
                    PasswordHash = adminPasswordHash,
                    FullName = "Системний Адміністратор", 
                    IsActive = true,
                    CreatedAt = DateTime.Now
                }
            );

            // Призначення ролі адміністратору
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserRoleId = 1, UserId = 1, RoleId = 1, AssignedAt = DateTime.Now }
            );
        }
    }
}
