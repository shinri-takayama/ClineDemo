using Microsoft.AspNetCore.Mvc;
using ECShop.API.Models;
using ECShop.API.Services;
using ECShop.API.Constants;

namespace ECShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
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
            try
            {
                var products = await _productService.SearchProductsAsync(search, minPrice, maxPrice);
                var productList = products.ToList();

                // Sorting (SQLite limitation requires in-memory sorting)
                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy.ToLower())
                    {
                        case "price":
                            productList = sortOrder?.ToLower() == "desc" 
                                ? productList.OrderByDescending(p => p.Price).ToList()
                                : productList.OrderBy(p => p.Price).ToList();
                            break;
                        case "name":
                            productList = sortOrder?.ToLower() == "desc"
                                ? productList.OrderByDescending(p => p.Name).ToList()
                                : productList.OrderBy(p => p.Name).ToList();
                            break;
                        case "date":
                            productList = sortOrder?.ToLower() == "desc"
                                ? productList.OrderByDescending(p => p.CreatedAt).ToList()
                                : productList.OrderBy(p => p.CreatedAt).ToList();
                            break;
                        default:
                            productList = productList.OrderBy(p => p.Id).ToList();
                            break;
                    }
                }

                return Ok(productList);
            }
            catch (ArgumentException)
            {
                return BadRequest(ErrorMessages.General.BAD_REQUEST);
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    return NotFound(ErrorMessages.Product.PRODUCT_NOT_FOUND);
                }

                return Ok(product);
            }
            catch (ArgumentException)
            {
                return BadRequest(ErrorMessages.General.BAD_REQUEST);
            }
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProductAsync(id, product);

                if (updatedProduct == null)
                {
                    return NotFound(ErrorMessages.Product.PRODUCT_NOT_FOUND);
                }

                return NoContent();
            }
            catch (ArgumentException)
            {
                return BadRequest(ErrorMessages.General.BAD_REQUEST);
            }
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            try
            {
                var createdProduct = await _productService.CreateProductAsync(product);
                return CreatedAtAction("GetProduct", new { id = createdProduct.Id }, createdProduct);
            }
            catch (ArgumentNullException)
            {
                return BadRequest(ErrorMessages.General.BAD_REQUEST);
            }
            catch (ArgumentException)
            {
                return BadRequest(ErrorMessages.General.BAD_REQUEST);
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var deleted = await _productService.DeleteProductAsync(id);

                if (!deleted)
                {
                    return NotFound(ErrorMessages.Product.PRODUCT_NOT_FOUND);
                }

                return NoContent();
            }
            catch (ArgumentException)
            {
                return BadRequest(ErrorMessages.General.BAD_REQUEST);
            }
        }
    }
}
