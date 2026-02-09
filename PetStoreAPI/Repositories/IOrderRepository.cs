using PetStoreAPI.Models;

namespace PetStoreAPI.Repositories
{
    /// <summary>
    /// Інтерфейс репозиторію для роботи з замовленнями
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Отримати всі замовлення
        /// </summary>
        Task<IEnumerable<Order>> GetAllAsync();

        /// <summary>
        /// Отримати замовлення за ID
        /// </summary>
        Task<Order?> GetByIdAsync(int id);

        /// <summary>
        /// Створити нове замовлення з деталями в транзакції
        /// </summary>
        Task<Order?> CreateOrderAsync(Order order, List<OrderDetail> orderDetails);

        /// <summary>
        /// Оновити статус замовлення
        /// </summary>
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);

        /// <summary>
        /// Отримати замовлення за статусом
        /// </summary>
        Task<IEnumerable<Order>> GetByStatusAsync(string status);

        /// <summary>
        /// Отримати замовлення за період
        /// </summary>
        Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
