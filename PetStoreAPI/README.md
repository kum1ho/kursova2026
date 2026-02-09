# PetStoreAPI - API для зоомагазину

## Опис проекту

Це ASP.NET Core Web API проект (.NET 8) для управління базою даних зоомагазину. Проект використовує Entity Framework Core для роботи з SQL Server та реалізує повний CRUD функціонал для товарів та замовлень.

## Структура проекту

```
PetStoreAPI/
├── Controllers/          # API контролери
│   ├── ProductsController.cs
│   └── OrdersController.cs
├── Data/                 # Контекст бази даних
│   └── ApplicationDbContext.cs
├── DTOs/                 # Об'єкти передачі даних
│   └── CreateOrderDTO.cs
├── Models/               # Модлі даних
│   ├── Animal.cs
│   ├── Category.cs
│   ├── Order.cs
│   ├── OrderDetail.cs
│   ├── Product.cs
│   └── Supplier.cs
├── Repositories/         # Репозиторії
│   ├── IOrderRepository.cs
│   ├── IProductRepository.cs
│   ├── OrderRepository.cs
│   └── ProductRepository.cs
├── appsettings.json      # Конфігурація
├── Program.cs           # Налаштування сервісів
└── README.md            # Цей файл
```

## Особливості реалізації

### База даних
- Використовується MS SQL Server
- Налаштовані Primary Keys та Foreign Keys
- Реалізовані обчислювані поля
- Додані початкові дані (seed data)

### Entity Framework Core
- Code First підхід
- Fluent API для налаштування моделей
- Міграції готові до створення
- Включені навігаційні властивості

### Репозиторії
- **ProductRepository**: CRUD операції для товарів
- **OrderRepository**: Управління замовленнями з транзакціями

### Транзакції
- Метод `CreateOrderAsync` виконує операцію в одній транзакції
- Автоматичне оновлення залишків товарів на складі
- Перевірка доступності товарів перед створенням замовлення
- Rollback при помилках

## API Ендпоінти

### Товари (Products)

| Метод | Ендпоінт | Опис |
|------|----------|------|
| GET | `/api/products` | Отримати всі товари |
| GET | `/api/products/{id}` | Отримати товар за ID |
| GET | `/api/products/category/{categoryId}` | Отримати товари за категорією |
| GET | `/api/products/active` | Отримати активні товари |
| GET | `/api/products/low-stock` | Отримати товари з низьким залишком |
| GET | `/api/products/sku/{sku}` | Отримати товар за SKU |
| POST | `/api/products` | Створити новий товар |
| PUT | `/api/products/{id}` | Оновити товар |
| DELETE | `/api/products/{id}` | Видалити товар (м'яке видалення) |
| PATCH | `/api/products/{id}/stock` | Оновити кількість на складі |

### Замовлення (Orders)

| Метод | Ендпоінт | Опис |
|------|----------|------|
| GET | `/api/orders` | Отримати всі замовлення |
| GET | `/api/orders/{id}` | Отримати замовлення за ID |
| GET | `/api/orders/status/{status}` | Отримати замовлення за статусом |
| GET | `/api/orders/date-range` | Отримати замовлення за період |
| POST | `/api/orders` | Створити нове замовлення |
| PATCH | `/api/orders/{id}/status` | Оновити статус замовлення |
| GET | `/api/orders/statuses` | Отримати доступні статуси |

## Приклад використання

### Створення замовлення

```json
POST /api/orders
{
  "customerName": "Іван Петренко",
  "customerPhone": "+380501234567",
  "customerEmail": "ivan@example.com",
  "customerAddress": "вул. Хрещатик, 10, Київ",
  "paymentMethod": "Картка",
  "orderDetails": [
    {
      "itemId": 1,
      "itemType": "Product",
      "quantity": 2,
      "discount": 10
    },
    {
      "itemId": 1,
      "itemType": "Animal",
      "quantity": 1,
      "discount": 0
    }
  ]
}
```

## Налаштування

1. Переконайтесь, що у вас встановлено MS SQL Server
2. Оновіть рядок підключення в `appsettings.json` за потреби
3. Створіть міграцію:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

## Запуск проекту

```bash
dotnet run
```

API буде доступне за адресою: `https://localhost:7xxx/api`

## Технології

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- MS SQL Server
- Repository Pattern
- Dependency Injection
