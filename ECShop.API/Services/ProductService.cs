using ECShop.API.Models;
using ECShop.API.Constants;
using ECShop.API.Repositories;

namespace ECShop.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            try
            {
                return await _productRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all products");
                throw;
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                return await _productRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
                throw;
            }
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            try
            {
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product));
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(product.Name))
                {
                    throw new ArgumentException(ErrorMessages.Validation.REQUIRED_FIELD, nameof(product.Name));
                }

                if (product.Price < 0)
                {
                    throw new ArgumentException(ErrorMessages.Validation.INVALID_RANGE, nameof(product.Price));
                }

                if (product.Stock < 0)
                {
                    throw new ArgumentException(ErrorMessages.Validation.INVALID_RANGE, nameof(product.Stock));
                }

                var createdProduct = await _productRepository.CreateAsync(product);
                _logger.LogInformation("Product created successfully with ID {ProductId}", createdProduct.Id);
                return createdProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                throw;
            }
        }

        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException(ErrorMessages.Validation.INVALID_RANGE, nameof(id));
                }

                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product));
                }

                if (id != product.Id)
                {
                    throw new ArgumentException(ErrorMessages.Validation.ID_MISMATCH);
                }

                // Check if product exists
                if (!await _productRepository.ExistsAsync(id))
                {
                    return null;
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(product.Name))
                {
                    throw new ArgumentException(ErrorMessages.Validation.REQUIRED_FIELD, nameof(product.Name));
                }

                if (product.Price < 0)
                {
                    throw new ArgumentException(ErrorMessages.Validation.INVALID_RANGE, nameof(product.Price));
                }

                if (product.Stock < 0)
                {
                    throw new ArgumentException(ErrorMessages.Validation.INVALID_RANGE, nameof(product.Stock));
                }

                var updatedProduct = await _productRepository.UpdateAsync(product);
                _logger.LogInformation("Product updated successfully with ID {ProductId}", id);
                return updatedProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var deleted = await _productRepository.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Product deleted successfully with ID {ProductId}", id);
                }
                return deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID {ProductId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string? search, decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                var products = await _productRepository.SearchAsync(search, minPrice, maxPrice);
                return products.OrderBy(p => p.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with search='{Search}', minPrice={MinPrice}, maxPrice={MaxPrice}", 
                    search, minPrice, maxPrice);
                throw;
            }
        }

        public async Task<bool> IsStockAvailableAsync(int productId, int quantity)
        {
            try
            {
                return await _productRepository.IsStockAvailableAsync(productId, quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking stock availability for product {ProductId}, quantity {Quantity}", 
                    productId, quantity);
                throw;
            }
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity)
        {
            try
            {
                var updated = await _productRepository.UpdateStockAsync(productId, quantity);
                if (updated)
                {
                    _logger.LogInformation("Stock updated for product {ProductId}, quantity change {Quantity}", 
                        productId, quantity);
                }
                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product {ProductId}, quantity change {Quantity}", 
                    productId, quantity);
                throw;
            }
        }
    }
}
