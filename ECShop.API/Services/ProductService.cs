using Microsoft.EntityFrameworkCore;
using ECShop.API.Data;
using ECShop.API.Models;
using ECShop.API.Constants;

namespace ECShop.API.Services
{
    public class ProductService : IProductService
    {
        private readonly ECShopContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ECShopContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            try
            {
                return await _context.Products
                    .OrderBy(p => p.Id)
                    .ToListAsync();
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
                if (id <= 0)
                {
                    throw new ArgumentException(ErrorMessages.Validation.INVALID_RANGE, nameof(id));
                }

                return await _context.Products.FindAsync(id);
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

                product.CreatedAt = DateTime.UtcNow;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product created successfully with ID {ProductId}", product.Id);
                return product;
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

                var existingProduct = await _context.Products.FindAsync(id);
                if (existingProduct == null)
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

                // Update properties
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.Stock = product.Stock;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Product updated successfully with ID {ProductId}", id);
                return existingProduct;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating product with ID {ProductId}", id);
                
                if (!await ProductExistsAsync(id))
                {
                    return null;
                }
                throw;
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
                if (id <= 0)
                {
                    throw new ArgumentException(ErrorMessages.Validation.INVALID_RANGE, nameof(id));
                }

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return false;
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product deleted successfully with ID {ProductId}", id);
                return true;
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
                    if (minPrice.Value < 0)
                    {
                        throw new ArgumentException(ErrorMessages.Validation.INVALID_RANGE, nameof(minPrice));
                    }
                    query = query.Where(p => p.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    if (maxPrice.Value < 0)
                    {
                        throw new ArgumentException(ErrorMessages.Validation.INVALID_RANGE, nameof(maxPrice));
                    }
                    query = query.Where(p => p.Price <= maxPrice.Value);
                }

                // Get products first, then sort in memory for price sorting (SQLite limitation)
                var products = await query.ToListAsync();
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
                if (productId <= 0)
                {
                    throw new ArgumentException(ErrorMessages.Validation.INVALID_RANGE, nameof(productId));
                }

                if (quantity <= 0)
                {
                    throw new ArgumentException(ErrorMessages.Validation.INVALID_RANGE, nameof(quantity));
                }

                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    throw new KeyNotFoundException(ErrorMessages.Product.PRODUCT_NOT_FOUND);
                }

                return product.Stock >= quantity;
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
                if (productId <= 0)
                {
                    throw new ArgumentException(ErrorMessages.Validation.INVALID_RANGE, nameof(productId));
                }

                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    throw new KeyNotFoundException(ErrorMessages.Product.PRODUCT_NOT_FOUND);
                }

                var newStock = product.Stock + quantity;
                if (newStock < 0)
                {
                    throw new InvalidOperationException(ErrorMessages.Product.INSUFFICIENT_STOCK);
                }

                product.Stock = newStock;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Stock updated for product {ProductId}: {OldStock} -> {NewStock}", 
                    productId, product.Stock - quantity, product.Stock);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product {ProductId}, quantity change {Quantity}", 
                    productId, quantity);
                throw;
            }
        }

        private async Task<bool> ProductExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(e => e.Id == id);
        }
    }
}
