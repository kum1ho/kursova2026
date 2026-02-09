using PetStoreAPI.Models;

namespace PetStoreAPI.Repositories
{
    /// <summary>
    /// Інтерфейс репозиторію для роботи з товарами
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Отримати всі товари
        /// </summary>
        Task<IEnumerable<Product>> GetAllAsync();

        /// <summary>
        /// Отримати товар за ID
        /// </summary>
        Task<Product?> GetByIdAsync(int id);

        /// <summary>
        /// Отримати товари за категорією
        /// </summary>
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);

        /// <summary>
        /// Отримати активні товари
        /// </summary>
        Task<IEnumerable<Product>> GetActiveAsync();

        /// <summary>
        /// Отримати товари з низьким залишком
        /// </summary>
        Task<IEnumerable<Product>> GetLowStockAsync();

        /// <summary>
        /// Додати новий товар
        /// </summary>
        Task<Product> AddAsync(Product product);

        /// <summary>
        /// Оновити існуючий товар
        /// </summary>
        Task<bool> UpdateAsync(Product product);

        /// <summary>
        /// Видалити товар
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Перевірити наявність товару
        /// </summary>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Оновити кількість товару на складі
        /// </summary>
        Task<bool> UpdateStockAsync(int productId, int quantityChange);

        /// <summary>
        /// Отримати товар за SKU
        /// </summary>
        Task<Product?> GetBySKUAsync(string sku);
    }
}
