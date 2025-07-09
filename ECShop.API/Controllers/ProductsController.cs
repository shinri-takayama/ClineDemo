using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECShop.API.Data;
using ECShop.API.Models;

namespace ECShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ECShopContext _context;

        public ProductsController(ECShopContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
            [FromQuery] string? search = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = "asc")
        {
            var query = _context.Products.AsQueryable();

            // Search by name or description (case-insensitive)
            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower) || 
                                        (p.Description != null && p.Description.ToLower().Contains(searchLower)));
            }

            // Filter by price range
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Get products first, then sort in memory for price sorting (SQLite limitation)
            var products = await query.ToListAsync();

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "price":
                        products = sortOrder?.ToLower() == "desc" 
                            ? products.OrderByDescending(p => p.Price).ToList()
                            : products.OrderBy(p => p.Price).ToList();
                        break;
                    case "name":
                        products = sortOrder?.ToLower() == "desc"
                            ? products.OrderByDescending(p => p.Name).ToList()
                            : products.OrderBy(p => p.Name).ToList();
                        break;
                    case "date":
                        products = sortOrder?.ToLower() == "desc"
                            ? products.OrderByDescending(p => p.CreatedAt).ToList()
                            : products.OrderBy(p => p.CreatedAt).ToList();
                        break;
                    default:
                        products = products.OrderBy(p => p.Id).ToList();
                        break;
                }
            }
            else
            {
                products = products.OrderBy(p => p.Id).ToList();
            }

            return products;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
