using Microsoft.AspNetCore.Mvc;
using PetStoreAPI.Models;
using PetStoreAPI.Repositories;

namespace PetStoreAPI.Controllers
{
    /// <summary>
    /// Контролер для роботи з товарами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Отримати всі товари
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        /// <summary>
        /// Отримати товар за ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound($"Товар з ID {id} не знайдено");

            return Ok(product);
        }

        /// <summary>
        /// Отримати товари за категорією
        /// </summary>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetByCategory(int categoryId)
        {
            var products = await _productRepository.GetByCategoryAsync(categoryId);
            return Ok(products);
        }

        /// <summary>
        /// Отримати активні товари
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Product>>> GetActive()
        {
            var products = await _productRepository.GetActiveAsync();
            return Ok(products);
        }

        /// <summary>
        /// Отримати товари з низьким залишком
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<Product>>> GetLowStock()
        {
            var products = await _productRepository.GetLowStockAsync();
            return Ok(products);
        }

        /// <summary>
        /// Отримати товар за SKU
        /// </summary>
        [HttpGet("sku/{sku}")]
        public async Task<ActionResult<Product>> GetBySKU(string sku)
        {
            var product = await _productRepository.GetBySKUAsync(sku);
            if (product == null)
                return NotFound($"Товар з SKU {sku} не знайдено");

            return Ok(product);
        }

        /// <summary>
        /// Додати новий товар
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdProduct = await _productRepository.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.ProductID }, createdProduct);
        }

        /// <summary>
        /// Оновити товар
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> Update(int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != product.ProductID)
                return BadRequest("ID в URL не співпадає з ID товару");

            var exists = await _productRepository.ExistsAsync(id);
            if (!exists)
                return NotFound($"Товар з ID {id} не знайдено");

            var updated = await _productRepository.UpdateAsync(product);
            if (!updated)
                return StatusCode(500, "Помилка при оновленні товару");

            return NoContent();
        }

        /// <summary>
        /// Видалити товар (м'яке видалення)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _productRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound($"Товар з ID {id} не знайдено");

            return NoContent();
        }

        /// <summary>
        /// Оновити кількість товару на складі
        /// </summary>
        [HttpPatch("{id}/stock")]
        public async Task<ActionResult> UpdateStock(int id, [FromBody] int quantityChange)
        {
            var updated = await _productRepository.UpdateStockAsync(id, quantityChange);
            if (!updated)
                return BadRequest($"Не вдалося оновити залишки товару з ID {id}. Перевірте наявність товару та коректність кількості.");

            return Ok(new { Message = "Залишки успішно оновлено", QuantityChange = quantityChange });
        }
    }
}
