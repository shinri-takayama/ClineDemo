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
    public class AdminController : ControllerBase
    {
        private readonly ECShopContext _context;

        public AdminController(ECShopContext context)
        {
            _context = context;
        }

        // GET: api/admin/users
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            if (!await IsCurrentUserAdmin())
            {
                return Forbid("管理者権限が必要です。");
            }

            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.CreatedAt,
                    u.IsAdmin
                })
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();

            return Ok(users);
        }

        // PUT: api/admin/users/{id}/admin-status
        [HttpPut("users/{id}/admin-status")]
        public async Task<IActionResult> UpdateAdminStatus(int id, [FromBody] AdminStatusRequest request)
        {
            if (!await IsCurrentUserAdmin())
            {
                return Forbid("管理者権限が必要です。");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("ユーザーが見つかりません。");
            }

            // Prevent removing admin status from the last admin
            if (!request.IsAdmin && user.IsAdmin)
            {
                var adminCount = await _context.Users.CountAsync(u => u.IsAdmin);
                if (adminCount <= 1)
                {
                    return BadRequest("最後の管理者の権限を削除することはできません。");
                }
            }

            user.IsAdmin = request.IsAdmin;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"ユーザー '{user.Username}' の管理者権限を更新しました。" });
        }

        // GET: api/admin/orders
        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? status = null)
        {
            if (!await IsCurrentUserAdmin())
            {
                return Forbid("管理者権限が必要です。");
            }

            var query = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => (int)o.Status == status.Value);
            }

            var totalCount = await query.CountAsync();
            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new
                {
                    o.Id,
                    o.UserId,
                    o.OrderDate,
                    o.Status,
                    StatusText = GetStatusText((int)o.Status),
                    o.TotalAmount,
                    o.ShippingName,
                    ShippingEmail = "", // Not available in current model
                    ItemCount = o.OrderItems.Count,
                    Items = o.OrderItems.Select(oi => new
                    {
                        oi.Id,
                        oi.ProductId,
                        ProductName = oi.Product != null ? oi.Product.Name : "商品が見つかりません",
                        oi.Quantity,
                        oi.Price
                    })
                })
                .ToListAsync();

            return Ok(new
            {
                orders,
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }

        // PUT: api/admin/orders/{id}/status
        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatusRequest request)
        {
            if (!await IsCurrentUserAdmin())
            {
                return Forbid("管理者権限が必要です。");
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound("注文が見つかりません。");
            }

            order.Status = (OrderStatus)request.Status;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"注文 #{order.Id} のステータスを更新しました。" });
        }

        // GET: api/admin/products
        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            if (!await IsCurrentUserAdmin())
            {
                return Forbid("管理者権限が必要です。");
            }

            var products = await _context.Products
                .OrderBy(p => p.Name)
                .ToListAsync();

            return Ok(products);
        }

        // POST: api/admin/products
        [HttpPost("products")]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if (!await IsCurrentUserAdmin())
            {
                return Forbid("管理者権限が必要です。");
            }

            product.CreatedAt = DateTime.UtcNow;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllProducts), new { id = product.Id }, product);
        }

        // PUT: api/admin/products/{id}
        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (!await IsCurrentUserAdmin())
            {
                return Forbid("管理者権限が必要です。");
            }

            if (id != product.Id)
            {
                return BadRequest("IDが一致しません。");
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound("商品が見つかりません。");
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.ImageUrl = product.ImageUrl;

            await _context.SaveChangesAsync();

            return Ok(new { message = $"商品 '{existingProduct.Name}' を更新しました。" });
        }

        // DELETE: api/admin/products/{id}
        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!await IsCurrentUserAdmin())
            {
                return Forbid("管理者権限が必要です。");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("商品が見つかりません。");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"商品 '{product.Name}' を削除しました。" });
        }

        // GET: api/admin/dashboard
        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboard()
        {
            if (!await IsCurrentUserAdmin())
            {
                return Forbid("管理者権限が必要です。");
            }

            var totalUsers = await _context.Users.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var orders = await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled) // Exclude cancelled orders
                .ToListAsync();
            var totalRevenue = orders.Sum(o => o.TotalAmount);

            var recentOrders = await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.TotalAmount,
                    o.ShippingName,
                    StatusText = GetStatusText((int)o.Status),
                    ItemCount = o.OrderItems.Count
                })
                .ToListAsync();

            var lowStockProducts = await _context.Products
                .Where(p => p.Stock <= 5)
                .OrderBy(p => p.Stock)
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                summary = new
                {
                    totalUsers,
                    totalProducts,
                    totalOrders,
                    totalRevenue
                },
                recentOrders,
                lowStockProducts
            });
        }

        private async Task<bool> IsCurrentUserAdmin()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return false;
            }

            var user = await _context.Users.FindAsync(userId);
            return user?.IsAdmin == true;
        }

        private static string GetStatusText(int status)
        {
            return status switch
            {
                0 => "注文確認中",
                1 => "注文確定",
                2 => "処理中",
                3 => "発送済み",
                4 => "配送完了",
                5 => "キャンセル",
                _ => "不明"
            };
        }
    }

    public class AdminStatusRequest
    {
        public bool IsAdmin { get; set; }
    }

    public class OrderStatusRequest
    {
        public int Status { get; set; }
    }
}
