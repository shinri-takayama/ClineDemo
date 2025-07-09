using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ECShop.API.Data;
using ECShop.API.Models;
using System.Security.Claims;

namespace ECShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ECShopContext _context;

        public OrdersController(ECShopContext context)
        {
            _context = context;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderSummaryResponse>>> GetOrders()
        {
            var userId = GetCurrentUserId();
            
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderSummaryResponse
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    StatusText = GetStatusText(o.Status),
                    TotalAmount = o.TotalAmount,
                    ItemCount = o.OrderItems.Count
                })
                .ToListAsync();

            return Ok(orders);
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponse>> GetOrder(int id)
        {
            var userId = GetCurrentUserId();
            
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            var response = new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                StatusText = GetStatusText(order.Status),
                TotalAmount = order.TotalAmount,
                ShippingName = order.ShippingName,
                ShippingPostalCode = order.ShippingPostalCode,
                ShippingPrefecture = order.ShippingPrefecture,
                ShippingCity = order.ShippingCity,
                ShippingAddressLine = order.ShippingAddressLine,
                ShippingPhone = order.ShippingPhone,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemResponse
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    ProductDescription = oi.ProductDescription,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    Subtotal = oi.Price * oi.Quantity
                }).ToList()
            };

            return Ok(response);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<OrderResponse>> CreateOrder(CreateOrderRequest request)
        {
            var userId = GetCurrentUserId();

            // Validate products and calculate total
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in request.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    return BadRequest($"Product with ID {item.ProductId} not found.");
                }

                if (product.Stock < item.Quantity)
                {
                    return BadRequest($"Insufficient stock for product {product.Name}. Available: {product.Stock}, Requested: {item.Quantity}");
                }

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price,
                    ProductName = product.Name,
                    ProductDescription = product.Description
                };

                orderItems.Add(orderItem);
                totalAmount += product.Price * item.Quantity;

                // Update product stock
                product.Stock -= item.Quantity;
            }

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = totalAmount,
                ShippingName = request.ShippingName,
                ShippingPostalCode = request.ShippingPostalCode,
                ShippingPrefecture = request.ShippingPrefecture,
                ShippingCity = request.ShippingCity,
                ShippingAddressLine = request.ShippingAddressLine,
                ShippingPhone = request.ShippingPhone,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Return created order
            var response = new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                StatusText = GetStatusText(order.Status),
                TotalAmount = order.TotalAmount,
                ShippingName = order.ShippingName,
                ShippingPostalCode = order.ShippingPostalCode,
                ShippingPrefecture = order.ShippingPrefecture,
                ShippingCity = order.ShippingCity,
                ShippingAddressLine = order.ShippingAddressLine,
                ShippingPhone = order.ShippingPhone,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemResponse
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    ProductDescription = oi.ProductDescription,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    Subtotal = oi.Price * oi.Quantity
                }).ToList()
            };

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, response);
        }

        // PUT: api/orders/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusRequest request)
        {
            var userId = GetCurrentUserId();
            
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = request.Status;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        private static string GetStatusText(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "注文確認中",
                OrderStatus.Confirmed => "注文確定",
                OrderStatus.Processing => "処理中",
                OrderStatus.Shipped => "発送済み",
                OrderStatus.Delivered => "配送完了",
                OrderStatus.Cancelled => "キャンセル",
                _ => "不明"
            };
        }
    }
}
