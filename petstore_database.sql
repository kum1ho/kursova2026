-- =============================================
-- База даних зоомагазину
-- Створено: 20.01.2026
-- Призначення: Повна структура бази даних для зоомагазину з тестовими даними
-- =============================================

-- Створення бази даних (якщо вона не існує)
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'PetStoreDB')
BEGIN
    CREATE DATABASE PetStoreDB;
END
GO

USE PetStoreDB;
GO

-- =============================================
-- Таблиця Categories (Категорії товарів)
-- Призначення: Зберігання категорій товарів зоомагазину
-- Кожна категорія має унікальний ідентифікатор та назву
-- =============================================
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,           -- Унікальний ідентифікатор категорії (автоінкремент)
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,         -- Назва категорії (унікальна, обов'язкова)
    Description NVARCHAR(500) NULL,                      -- Опис категорії (необов'язковий)
    CreatedAt DATETIME DEFAULT GETDATE()                 -- Дата створення запису (за замовчуванням поточна дата)
);
GO

-- =============================================
-- Таблиця Suppliers (Постачальники)
-- Призначення: Зберігання інформації про постачальників товарів
-- Містить контактну інформацію та деталі для співпраці
-- =============================================
CREATE TABLE Suppliers (
    SupplierID INT IDENTITY(1,1) PRIMARY KEY,            -- Унікальний ідентифікатор постачальника
    CompanyName NVARCHAR(150) NOT NULL,                  -- Назва компанії постачальника
    ContactPerson NVARCHAR(100) NOT NULL,                -- Контактна особа
    Phone NVARCHAR(20) NULL,                             -- Телефон постачальника
    Email NVARCHAR(100) NULL,                            -- Email постачальника
    Address NVARCHAR(300) NULL,                          -- Адреса постачальника
    City NVARCHAR(100) NULL,                             -- Місто
    Country NVARCHAR(100) NULL,                          -- Країна
    CreatedAt DATETIME DEFAULT GETDATE()                  -- Дата створення запису
);
GO

-- =============================================
-- Таблиця Animals (Тварини)
-- Призначення: Зберігання інформації про живих тварин, що продаються в магазині
-- Містить детальну інформацію про кожну тварину
-- =============================================
CREATE TABLE Animals (
    AnimalID INT IDENTITY(1,1) PRIMARY KEY,              -- Унікальний ідентифікатор тварини
    Name NVARCHAR(100) NOT NULL,                         -- Кличка або назва тварини
    Species NVARCHAR(50) NOT NULL,                       -- Вид тварини (собака, кіт, птах і т.д.)
    Breed NVARCHAR(100) NULL,                            -- Порода
    AgeMonths INT NULL,                                  -- Вік у місяцях
    Gender NVARCHAR(10) NULL CHECK (Gender IN ('Самець', 'Самиця', 'Чоловіча', 'Жіноча', 'Male', 'Female')), -- Стать
    Color NVARCHAR(50) NULL,                             -- Колір
    Weight DECIMAL(5,2) NULL,                            -- Вага в кг
    Price DECIMAL(10,2) NOT NULL,                        -- Ціна
    DateOfBirth DATETIME NULL,                           -- Дата народження
    IsAvailable BIT DEFAULT 1,                           -- Доступність для продажу (1 - доступна, 0 - не доступна)
    CategoryID INT NULL,                                 -- Посилання на категорію
    SupplierID INT NULL,                                 -- Посилання на постачальника
    DateAdded DATETIME DEFAULT GETDATE(),                -- Дата додавання в магазин
    Description NVARCHAR(1000) NULL,                     -- Додатковий опис
    
    -- Внешні ключі
    CONSTRAINT FK_Animals_Categories FOREIGN KEY (CategoryID) 
        REFERENCES Categories(CategoryID) ON DELETE SET NULL,
    CONSTRAINT FK_Animals_Suppliers FOREIGN KEY (SupplierID) 
        REFERENCES Suppliers(SupplierID) ON DELETE SET NULL
);
GO

-- =============================================
-- Таблиця Products (Товари)
-- Призначення: Зберігання інформації про товари зоомагазину (корм, іграшки, аксесуари)
-- Містить інформацію про ціни, залишки та постачальників
-- =============================================
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,             -- Унікальний ідентифікатор товару
    ProductName NVARCHAR(200) NOT NULL,                  -- Назва товару
    Description NVARCHAR(1000) NULL,                     -- Опис товару
    Price DECIMAL(10,2) NOT NULL,                        -- Ціна за одиницю
    QuantityInStock INT NOT NULL DEFAULT 0,             -- Кількість на складі
    ReorderLevel INT DEFAULT 10,                         -- Мінімальний рівень для замовлення
    Unit NVARCHAR(50) NULL,                              -- Одиниця виміру (шт, кг, л і т.д.)
    Weight DECIMAL(8,3) NULL,                            -- Вага товару
    Dimensions NVARCHAR(100) NULL,                        -- Розміри (довжина x ширина x висота)
    SKU NVARCHAR(50) NULL UNIQUE,                        -- Артикул (унікальний)
    CategoryID INT NULL,                                 -- Посилання на категорію
    SupplierID INT NULL,                                 -- Посилання на постачальника
    DateAdded DATETIME DEFAULT GETDATE(),                -- Дата додавання
    IsActive BIT DEFAULT 1,                              -- Чи активний товар (1 - так, 0 - ні)
    
    -- Внешні ключі
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryID) 
        REFERENCES Categories(CategoryID) ON DELETE SET NULL,
    CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierID) 
        REFERENCES Suppliers(SupplierID) ON DELETE SET NULL
);
GO

-- =============================================
-- Таблиця Orders (Замовлення)
-- Призначення: Зберігання інформації про замовлення клієнтів
-- Містить дані про клієнта, дату замовлення та загальну суму
-- =============================================
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,              -- Унікальний ідентифікатор замовлення
    CustomerName NVARCHAR(150) NOT NULL,                -- Ім'я клієнта
    CustomerPhone NVARCHAR(20) NULL,                     -- Телефон клієнта
    CustomerEmail NVARCHAR(100) NULL,                    -- Email клієнта
    CustomerAddress NVARCHAR(500) NULL,                  -- Адреса доставки
    OrderDate DATETIME DEFAULT GETDATE(),                -- Дата замовлення
    RequiredDate DATETIME NULL,                           -- Бажана дата доставки
    ShippedDate DATETIME NULL,                            -- Дата відвантаження
    TotalAmount DECIMAL(12,2) NOT NULL DEFAULT 0,       -- Загальна сума замовлення
    Status NVARCHAR(50) DEFAULT 'Нове' CHECK (Status IN ('Нове', 'В обробці', 'Відправлено', 'Доставлено', 'Скасовано')), -- Статус замовлення
    PaymentMethod NVARCHAR(50) NULL,                     -- Спосіб оплати
    PaymentStatus NVARCHAR(50) DEFAULT 'Не оплачено' CHECK (PaymentStatus IN ('Не оплачено', 'Оплачено', 'Частково оплачено')), -- Статус оплати
    Notes NVARCHAR(1000) NULL,                           -- Примітки до замовлення
    CreatedAt DATETIME DEFAULT GETDATE()                 -- Дата створення запису
);
GO

-- =============================================
-- Таблиця OrderDetails (Деталі замовлення)
-- Призначення: Зв'язуюча таблиця між замовленнями та товарами/тваринами
-- Зберігає інформацію про кожну позицію в замовленні
-- =============================================
CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,         -- Унікальний ідентифікатор позиції замовлення
    OrderID INT NOT NULL,                                -- Посилання на замовлення
    ProductID INT NULL,                                  -- Посилання на товар (може бути NULL, якщо замовляється тварина)
    AnimalID INT NULL,                                   -- Посилання на тварину (може бути NULL, якщо замовляється товар)
    Quantity INT NOT NULL DEFAULT 1,                     -- Кількість
    UnitPrice DECIMAL(10,2) NOT NULL,                    -- Ціна за одиницю на момент замовлення
    Discount DECIMAL(5,2) DEFAULT 0,                     -- Знижка в %
    LineTotal AS (Quantity * UnitPrice * (1 - Discount/100)) PERSISTED, -- Загальна сума по позиції (обчислюване поле)
    
    -- Внешні ключі
    CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderID) 
        REFERENCES Orders(OrderID) ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductID) 
        REFERENCES Products(ProductID) ON DELETE SET NULL,
    CONSTRAINT FK_OrderDetails_Animals FOREIGN KEY (AnimalID) 
        REFERENCES Animals(AnimalID) ON DELETE SET NULL,
    
    -- Перевірка: хоча б один з ProductID або AnimalID повинен бути вказаний
    CONSTRAINT CHK_OrderDetails_ItemType CHECK (
        (ProductID IS NOT NULL AND AnimalID IS NULL) OR 
        (ProductID IS NULL AND AnimalID IS NOT NULL)
    )
);
GO

-- =============================================
-- Вставка тестових даних
-- =============================================

-- Тестові дані для Categories
INSERT INTO Categories (CategoryName, Description) VALUES
('Собаки', 'Все для собак: корм, іграшки, аксесуари'),
('Коти', 'Товари для котів та кошенят'),
('Птахи', 'Все для домашніх птахів'),
('Рибки', 'Акваріуми, рибки та аксесуари'),
('Гризуни', 'Товари для хом''яків, кроликів та інших гризунів'),
('Рептилії', 'Товари для рептилій та амфібій'),
('Живі тварини', 'Продаж живих тварин'),
('Корм', 'Корм для всіх видів домашніх тварин'),
('Іграшки', 'Іграшки та розважальні аксесуари'),
('Медицина', 'Ветеринарні препарати та засоби гігієни');
GO

-- Тестові дані для Suppliers
INSERT INTO Suppliers (CompanyName, ContactPerson, Phone, Email, Address, City, Country) VALUES
('PetFood Ltd', 'Іван Петренко', '+380441234567', 'info@petfood.com', 'вул. Хрещатик, 1', 'Київ', 'Україна'),
('Animal World', 'Марія Ковальчук', '+380507654321', 'sales@animalworld.com', 'вул. Сагайдачного, 15', 'Київ', 'Україна'),
('Bird Paradise', 'Олександр Сидоренко', '+380678901234', 'contact@birdparadise.com', 'вул. Шевченка, 25', 'Львів', 'Україна'),
('Aqua Life', 'Наталія Мельник', '+380991112233', 'info@aqualife.com', 'вул. Франка, 10', 'Львів', 'Україна'),
('Rodent House', 'Сергій Ткаченко', '+380634445566', 'sales@rodenthouse.com', 'вул. Незалежності, 5', 'Харків', 'Україна'),
('Reptile Zone', 'Андрій Бондаренко', '+380977778899', 'info@reptilezone.com', 'вул. Центральна, 20', 'Одеса', 'Україна'),
('Happy Pets', 'Олена Козак', '+380682223344', 'contact@happypets.com', 'вул. Леніна, 30', 'Дніпро', 'Україна'),
('Vet Supply', 'Дмитро Павленко', '+380935556677', 'orders@vetsupply.com', 'вул. Пушкіна, 12', 'Запоріжжя', 'Україна'),
('Toy Factory', 'Ірина Григоренко', '+380639998877', 'sales@toysforpets.com', 'вул. Гагаріна, 8', 'Кривий Ріг', 'Україна'),
('Premium Feed', 'Володимир Мороз', '+380966667788', 'info@premiumfeed.com', 'вул. Миру, 18', 'Вінниця', 'Україна');
GO

-- Тестові дані для Animals
INSERT INTO Animals (Name, Species, Breed, AgeMonths, Gender, Color, Weight, Price, DateOfBirth, CategoryID, SupplierID, Description) VALUES
('Рекс', 'Собака', 'Німецька вівчарка', 12, 'Самець', 'Чорно-рудий', 25.50, 15000.00, '2024-01-20', 7, 1, 'Дружня та активна собака, добре дресирована'),
('Мурка', 'Кіт', 'Британська короткошерста', 6, 'Самиця', 'Сіра', 3.20, 8000.00, '2024-07-20', 7, 2, 'Спокійна кішка, любить гратися'),
('Кеша', 'Папуга', 'Хвилястий', 3, 'Самець', 'Зелено-жовтий', 0.04, 500.00, '2024-10-20', 7, 3, 'Говорить кілька слів, дуже соціальний'),
('Немо', 'Рибка', 'Клоун', 2, 'Чоловіча', 'Оранжевий з білим', 0.02, 150.00, '2024-11-20', 7, 4, 'Красива рибка для акваріума'),
('Пух', 'Хом''як', 'Сирійський', 4, 'Самець', 'Золотистий', 0.12, 200.00, '2024-09-20', 7, 5, 'Миленький хом''як, любить насіння'),
('Гео', 'Черепаха', 'Червоновуха', 24, 'Самиця', 'Зелено-коричнева', 1.50, 1200.00, '2022-01-20', 7, 6, 'Довговічна черепаха, потребує акватераріуму'),
('Барон', 'Собака', 'Лабрадор', 8, 'Самець', 'Коричневий', 18.00, 12000.00, '2024-05-20', 7, 1, 'Дуже доброзичливий, любить дітей'),
('Луна', 'Кіт', 'Мейн-кун', 10, 'Самиця', 'Сріблясто-чорна', 4.80, 18000.00, '2024-03-20', 7, 2, 'Велика та красива кішка, добра до людей'),
('Кікі', 'Папуга', 'Корелла', 6, 'Самиця', 'Сіро-біла', 0.08, 800.00, '2024-07-20', 7, 3, 'Співає мелодії, дуже розумна'),
('Сніжок', 'Кролик', 'Великий білий', 5, 'Самець', 'Білий', 2.30, 600.00, '2024-08-20', 7, 5, 'Миленький кролик, їсть моркву');
GO

-- Тестові дані для Products
INSERT INTO Products (ProductName, Description, Price, QuantityInStock, Unit, Weight, SKU, CategoryID, SupplierID) VALUES
('Сухий корм для собак', 'Преміум корм для дорослих собак великих порід', 450.00, 50, 'кг', 15.000, 'DOG-FOOD-001', 8, 1),
('Сухий корм для котів', 'Балансований корм для дорослих котів', 380.00, 75, 'кг', 10.000, 'CAT-FOOD-001', 8, 1),
('М''ячик для собак', 'Резиновий м''ячик для гри', 120.00, 100, 'шт', 0.150, 'DOG-TOY-001', 9, 9),
('Лазерна указка для котів', 'Інтерактивна іграшка для котів', 250.00, 60, 'шт', 0.050, 'CAT-TOY-001', 9, 9),
('Клітка для птахів', 'Металева клітка для хвилястих папуг', 1200.00, 20, 'шт', 5.500, 'BIRD-CAGE-001', 3, 3),
('Акваріум 50л', 'Скляний акваріум з підсвіткою', 2500.00, 15, 'шт', 25.000, 'AQUA-50L-001', 4, 4),
('Клітка для хом''яків', 'Пластикова клітка з колесом', 450.00, 40, 'шт', 2.000, 'HAM-CAGE-001', 5, 5),
('Тераріум для рептилій', 'Скляний тераріум з обігрівом', 3500.00, 10, 'шт', 30.000, 'REP-TERR-001', 6, 6),
('Шампунь для собак', 'Гіпоалергенний шампунь', 180.00, 80, 'шт', 0.500, 'DOG-SHAM-001', 10, 8),
('Нашийник для котів', 'Шкіряний нашийник з дзвіночком', 150.00, 120, 'шт', 0.030, 'CAT-COLL-001', 2, 7);
GO

-- Тестові дані для Orders
INSERT INTO Orders (CustomerName, CustomerPhone, CustomerEmail, CustomerAddress, OrderDate, RequiredDate, TotalAmount, Status, PaymentMethod, PaymentStatus, Notes) VALUES
('Іван Іваненко', '+380501234567', 'ivan.ivanenko@email.com', 'вул. Хрещатик, 10, кв. 5, Київ', '2025-01-15 10:30:00', '2025-01-18', 570.00, 'Доставлено', 'Картка', 'Оплачено', 'Доставити після 18:00'),
('Марія Петренко', '+380669876543', 'maria.p@email.com', 'вул. Леніна, 25, кв. 12, Львів', '2025-01-16 14:20:00', '2025-01-19', 1450.00, 'Відправлено', 'Наложений платіж', 'Не оплачено', 'Обережно з склом'),
('Олександр Коваль', '+380991112233', 'olex.k@email.com', 'вул. Шевченка, 8, кв. 3, Харків', '2025-01-17 09:15:00', '2025-01-20', 380.00, 'В обробці', 'Картка', 'Оплачено', 'Потрібна консультація з доглядом'),
('Наталія Сидоренко', '+380634445566', 'nataly.s@email.com', 'вул. Франка, 15, кв. 7, Одеса', '2025-01-17 16:45:00', '2025-01-21', 2500.00, 'Нове', 'Готівка', 'Не оплачено', 'Доставка до під''їзду'),
('Сергій Мельник', '+380687778899', 'serg.m@email.com', 'вул. Незалежності, 30, кв. 9, Дніпро', '2025-01-18 11:00:00', '2025-01-22', 180.00, 'Нове', 'Картка', 'Не оплачено', 'Подарункова упаковка'),
('Тетяна Бондар', '+380972223344', 'tetiana.b@email.com', 'вул. Пушкіна, 5, кв. 2, Запоріжжя', '2025-01-18 13:30:00', '2025-01-23', 1200.00, 'В обробці', 'Наложений платіж', 'Не оплачено', 'Перевірити наявність'),
('Андрій Григоренко', '+380639998877', 'andriy.g@email.com', 'вул. Гагаріна, 12, кв. 11, Кривий Ріг', '2025-01-19 10:00:00', '2025-01-24', 450.00, 'Нове', 'Картка', 'Не оплачено', 'Замінити на інший колір'),
('Олена Мороз', '+380966667788', 'olena.m@email.com', 'вул. Миру, 18, кв. 4, Вінниця', '2025-01-19 15:20:00', '2025-01-25', 150.00, 'Нове', 'Готівка', 'Не оплачено', 'Доставка в офіс'),
('Дмитро Козак', '+380505556677', 'dmytro.k@email.com', 'вул. Центральна, 22, кв. 6, Полтава', '2025-01-20 09:45:00', '2025-01-26', 800.00, 'Нове', 'Картка', 'Не оплачено', 'Терміново'),
('Ірина Павленко', '+380684445566', 'iryna.p@email.com', 'вул. Лесі Українки, 7, кв. 8, Черкаси', '2025-01-20 14:10:00', '2025-01-27', 600.00, 'Нове', 'Наложений платіж', 'Не оплачено', 'Передзвонити за годину');
GO

-- Тестові дані для OrderDetails
INSERT INTO OrderDetails (OrderID, ProductID, AnimalID, Quantity, UnitPrice, Discount) VALUES
-- Замовлення 1 (Іван Іваненко) - купив корм та іграшку для собаки
(1, 1, NULL, 1, 450.00, 0),
(1, 3, NULL, 1, 120.00, 0),

-- Замовлення 2 (Марія Петренко) - купила акваріум та клітку для птахів
(2, 5, NULL, 1, 1200.00, 0),
(2, 6, NULL, 1, 250.00, 0),

-- Замовлення 3 (Олександр Коваль) - купив корм для котів
(3, 2, NULL, 1, 380.00, 0),

-- Замовлення 4 (Наталія Сидоренко) - купила тераріум
(4, 8, NULL, 1, 2500.00, 0),

-- Замовлення 5 (Сергій Мельник) - купив шампунь для собак
(5, 9, NULL, 1, 180.00, 0),

-- Замовлення 6 (Тетяна Бондар) - купила клітку для хом''яків
(6, 7, NULL, 1, 450.00, 0),

-- Замовлення 7 (Андрій Григоренко) - купив корм для собак
(7, 1, NULL, 1, 450.00, 0),

-- Замовлення 8 (Олена Мороз) - купила нашийник для котів
(8, 10, NULL, 1, 150.00, 0),

-- Замовлення 9 (Дмитро Козак) - купив іграшку для котів та корм для собак
(9, 4, NULL, 1, 250.00, 0),
(9, 1, NULL, 1, 450.00, 10), -- зі знижкою 10%

-- Замовлення 10 (Ірина Павленко) - купила клітку для хом''яків та іграшку для собак
(10, 7, NULL, 1, 450.00, 0),
(10, 3, NULL, 1, 120.00, 5); -- зі знижкою 5%
GO

-- =============================================
-- Оновлення загальних сум замовлень
-- =============================================
UPDATE o
SET TotalAmount = (
    SELECT SUM(LineTotal) 
    FROM OrderDetails od 
    WHERE od.OrderID = o.OrderID
)
FROM Orders o;
GO

-- =============================================
-- Перевірка даних
-- =============================================
PRINT 'База даних PetStoreDB успішно створена!';
PRINT 'Кількість категорій: ' + CAST((SELECT COUNT(*) FROM Categories) AS NVARCHAR(10));
PRINT 'Кількість постачальників: ' + CAST((SELECT COUNT(*) FROM Suppliers) AS NVARCHAR(10));
PRINT 'Кількість тварин: ' + CAST((SELECT COUNT(*) FROM Animals) AS NVARCHAR(10));
PRINT 'Кількість товарів: ' + CAST((SELECT COUNT(*) FROM Products) AS NVARCHAR(10));
PRINT 'Кількість замовлень: ' + CAST((SELECT COUNT(*) FROM Orders) AS NVARCHAR(10));
PRINT 'Кількість позицій в замовленнях: ' + CAST((SELECT COUNT(*) FROM OrderDetails) AS NVARCHAR(10));
GO

-- =============================================
-- Корисні запити для перевірки
-- =============================================

-- Показати всіх тварин з постачальниками
-- SELECT a.Name, a.Species, a.Breed, a.Price, s.CompanyName 
-- FROM Animals a
-- LEFT JOIN Suppliers s ON a.SupplierID = s.SupplierID;

-- Показати товари з низьким залишком
-- SELECT ProductName, QuantityInStock, ReorderLevel 
-- FROM Products 
-- WHERE QuantityInStock <= ReorderLevel;

-- Показати замовлення з деталями
-- SELECT o.OrderID, o.CustomerName, o.OrderDate, o.TotalAmount, od.ProductName, od.Quantity, od.LineTotal
-- FROM Orders o
-- JOIN (
--     SELECT od.OrderID, p.ProductName, od.Quantity, od.LineTotal
--     FROM OrderDetails od
--     JOIN Products p ON od.ProductID = p.ProductID
--     WHERE od.ProductID IS NOT NULL
--     UNION ALL
--     SELECT od.OrderID, a.Name + ' (' + a.Species + ')' as ProductName, od.Quantity, od.LineTotal
--     FROM OrderDetails od
--     JOIN Animals a ON od.AnimalID = a.AnimalID
--     WHERE od.AnimalID IS NOT NULL
-- ) od ON o.OrderID = od.OrderID
-- ORDER BY o.OrderID, od.LineTotal;
