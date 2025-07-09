using Microsoft.AspNetCore.Mvc;
using ECShop.API.Services;
using ECShop.API.Constants;
using ECShop.API.DTOs;
using ECShop.API.Mappers;

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
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts(
            [FromQuery] ProductSearchRequest searchRequest)
        {
            try
            {
                var products = await _productService.SearchProductsAsync(
                    searchRequest.Search, 
                    searchRequest.MinPrice, 
                    searchRequest.MaxPrice);
                var productList = products.ToList();

                // Sorting (SQLite limitation requires in-memory sorting)
                if (!string.IsNullOrEmpty(searchRequest.SortBy))
                {
                    switch (searchRequest.SortBy.ToLower())
                    {
                        case "price":
                            productList = searchRequest.SortOrder?.ToLower() == "desc" 
                                ? productList.OrderByDescending(p => p.Price).ToList()
                                : productList.OrderBy(p => p.Price).ToList();
                            break;
                        case "name":
                            productList = searchRequest.SortOrder?.ToLower() == "desc"
                                ? productList.OrderByDescending(p => p.Name).ToList()
                                : productList.OrderBy(p => p.Name).ToList();
                            break;
                        case "date":
                            productList = searchRequest.SortOrder?.ToLower() == "desc"
                                ? productList.OrderByDescending(p => p.CreatedAt).ToList()
                                : productList.OrderBy(p => p.CreatedAt).ToList();
                            break;
                        default:
                            productList = productList.OrderBy(p => p.Id).ToList();
                            break;
                    }
                }

                var responseList = ProductMapper.ToResponseList(productList);
                return Ok(responseList);
            }
            catch (ArgumentException)
            {
                return BadRequest(ErrorMessages.General.BAD_REQUEST);
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    return NotFound(ErrorMessages.Product.PRODUCT_NOT_FOUND);
                }

                var response = ProductMapper.ToResponse(product);
                return Ok(response);
            }
            catch (ArgumentException)
            {
                return BadRequest(ErrorMessages.General.BAD_REQUEST);
            }
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, UpdateProductRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = ProductMapper.ToModel(request, id);
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
        public async Task<ActionResult<ProductResponse>> PostProduct(CreateProductRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = ProductMapper.ToModel(request);
                var createdProduct = await _productService.CreateProductAsync(product);
                var response = ProductMapper.ToResponse(createdProduct);
                
                return CreatedAtAction("GetProduct", new { id = createdProduct.Id }, response);
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
