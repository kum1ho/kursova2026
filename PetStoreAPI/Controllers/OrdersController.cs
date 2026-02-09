using Microsoft.AspNetCore.Mvc;
using PetStoreAPI.DTOs;
using PetStoreAPI.Models;
using PetStoreAPI.Repositories;

namespace PetStoreAPI.Controllers
{
    /// <summary>
    /// Контролер для роботи з замовленнями
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrdersController(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Отримати всі замовлення
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var orders = await _orderRepository.GetAllAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Отримати замовлення за ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return NotFound($"Замовлення з ID {id} не знайдено");

            return Ok(order);
        }

        /// <summary>
        /// Отримати замовлення за статусом
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetByStatus(string status)
        {
            var orders = await _orderRepository.GetByStatusAsync(status);
            return Ok(orders);
        }

        /// <summary>
        /// Отримати замовлення за період
        /// </summary>
        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<Order>>> GetByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var orders = await _orderRepository.GetByDateRangeAsync(startDate, endDate);
            return Ok(orders);
        }

        /// <summary>
        /// Створити нове замовлення
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderDTO createOrderDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Створюємо об'єкт замовлення
                var order = new Order
                {
                    CustomerName = createOrderDTO.CustomerName,
                    CustomerPhone = createOrderDTO.CustomerPhone,
                    CustomerEmail = createOrderDTO.CustomerEmail,
                    CustomerAddress = createOrderDTO.CustomerAddress,
                    RequiredDate = createOrderDTO.RequiredDate,
                    PaymentMethod = createOrderDTO.PaymentMethod,
                    Notes = createOrderDTO.Notes
                };

                // Створюємо деталі замовлення
                var orderDetails = new List<OrderDetail>();
                foreach (var detailDto in createOrderDTO.OrderDetails)
                {
                    var orderDetail = new OrderDetail
                    {
                        Quantity = detailDto.Quantity,
                        Discount = detailDto.Discount
                    };

                    // Встановлюємо ProductID або AnimalID залежно від типу
                    if (detailDto.ItemType.Equals("Product", StringComparison.OrdinalIgnoreCase))
                    {
                        orderDetail.ProductID = detailDto.ItemId;
                    }
                    else if (detailDto.ItemType.Equals("Animal", StringComparison.OrdinalIgnoreCase))
                    {
                        orderDetail.AnimalID = detailDto.ItemId;
                    }
                    else
                    {
                        return BadRequest($"Некоректний тип позиції: {detailDto.ItemType}. Очікується 'Product' або 'Animal'");
                    }

                    orderDetails.Add(orderDetail);
                }

                // Створюємо замовлення в транзакції
                var createdOrder = await _orderRepository.CreateOrderAsync(order, orderDetails);
                
                return CreatedAtAction(nameof(GetById), new { id = createdOrder.OrderID }, createdOrder);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутрішня помилка сервера: {ex.Message}");
            }
        }

        /// <summary>
        /// Оновити статус замовлення
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest("Статус є обов'язковим полем");

            var updated = await _orderRepository.UpdateOrderStatusAsync(id, status);
            if (!updated)
                return NotFound($"Замовлення з ID {id} не знайдено");

            return Ok(new { Message = $"Статус замовлення {id} успішно оновлено на '{status}'" });
        }

        /// <summary>
        /// Отримати доступні статуси замовлень
        /// </summary>
        [HttpGet("statuses")]
        public ActionResult<IEnumerable<string>> GetAvailableStatuses()
        {
            var statuses = new[] { "Нове", "В обробці", "Відправлено", "Доставлено", "Скасовано" };
            return Ok(statuses);
        }
    }
}
