using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreAPI.Models;

namespace PetStoreAPI.Repositories
{
    /// <summary>
    /// Реалізація репозиторію для роботи з замовленнями
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepository;

        public OrderRepository(ApplicationDbContext context, IProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Animal)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Animal)
                .FirstOrDefaultAsync(o => o.OrderID == id);
        }

        public async Task<Order?> CreateOrderAsync(Order order, List<OrderDetail> orderDetails)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Перевіряємо наявність товарів та достатню кількість на складі
                foreach (var detail in orderDetails)
                {
                    if (detail.ProductID.HasValue)
                    {
                        var product = await _productRepository.GetByIdAsync(detail.ProductID.Value);
                        if (product == null)
                        {
                            await transaction.RollbackAsync();
                            throw new InvalidOperationException($"Товар з ID {detail.ProductID} не знайдено");
                        }

                        if (product.QuantityInStock < detail.Quantity)
                        {
                            await transaction.RollbackAsync();
                            throw new InvalidOperationException($"Недостатня кількість товару '{product.ProductName}'. Доступно: {product.QuantityInStock}, потрібно: {detail.Quantity}");
                        }

                        // Встановлюємо ціну на момент замовлення
                        detail.UnitPrice = product.Price;
                    }
                    else if (detail.AnimalID.HasValue)
                    {
                        var animal = await _context.Animals.FindAsync(detail.AnimalID.Value);
                        if (animal == null)
                        {
                            await transaction.RollbackAsync();
                            throw new InvalidOperationException($"Тварина з ID {detail.AnimalID} не знайдена");
                        }

                        if (!animal.IsAvailable)
                        {
                            await transaction.RollbackAsync();
                            throw new InvalidOperationException($"Тварина '{animal.Name}' не доступна для продажу");
                        }

                        // Встановлюємо ціну на момент замовлення
                        detail.UnitPrice = animal.Price;

                        // Позначаємо тварину як не доступну
                        animal.IsAvailable = false;
                        _context.Entry(animal).State = EntityState.Modified;
                    }
                }

                // Розраховуємо загальну суму замовлення
                order.TotalAmount = orderDetails.Sum(od => od.Quantity * od.UnitPrice * (1 - od.Discount / 100));
                order.OrderDate = DateTime.Now;
                order.Status = "Нове";
                order.CreatedAt = DateTime.Now;

                // Додаємо замовлення
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Додаємо деталі замовлення
                foreach (var detail in orderDetails)
                {
                    detail.OrderID = order.OrderID;
                    _context.OrderDetails.Add(detail);
                }

                await _context.SaveChangesAsync();

                // Зменшуємо кількість товарів на складі
                foreach (var detail in orderDetails)
                {
                    if (detail.ProductID.HasValue)
                    {
                        var stockUpdated = await _productRepository.UpdateStockAsync(detail.ProductID.Value, -detail.Quantity);
                        if (!stockUpdated)
                        {
                            await transaction.RollbackAsync();
                            throw new InvalidOperationException($"Помилка оновлення залишків товару з ID {detail.ProductID}");
                        }
                    }
                }

                await transaction.CommitAsync();

                // Повертаємо замовлення з деталями
                return await GetByIdAsync(order.OrderID);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                    return false;

                order.Status = status;
                
                // Якщо статус "Відправлено", встановлюємо дату відвантаження
                if (status == "Відправлено" && !order.ShippedDate.HasValue)
                {
                    order.ShippedDate = DateTime.Now;
                }

                _context.Entry(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Animal)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Animal)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
