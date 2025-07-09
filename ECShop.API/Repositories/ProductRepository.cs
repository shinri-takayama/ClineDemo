using Microsoft.EntityFrameworkCore;
using ECShop.API.Data;
using ECShop.API.Models;
using ECShop.API.Constants;

namespace ECShop.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ECShopContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ECShopContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                return await _context.Products
                    .OrderBy(p => p.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all products from repository");
                throw;
            }
        }

        public async Task<Product?> GetByIdAsync(int id)
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
                _logger.LogError(ex, "Error retrieving product with ID {ProductId} from repository", id);
                throw;
            }
        }

        public async Task<Product> CreateAsync(Product product)
        {
            try
            {
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product));
                }

                product.CreatedAt = DateTime.UtcNow;
                _context.Products.Add(product);
                await SaveChangesAsync();

                _logger.LogInformation("Product created in repository with ID {ProductId}", product.Id);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product in repository");
                throw;
            }
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            try
            {
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product));
                }

                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct == null)
                {
                    throw new KeyNotFoundException(ErrorMessages.Product.PRODUCT_NOT_FOUND);
                }

                // Update properties
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.Stock = product.Stock;

                _context.Products.Update(existingProduct);
                await SaveChangesAsync();

                _logger.LogInformation("Product updated in repository with ID {ProductId}", product.Id);
                return existingProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product in repository with ID {ProductId}", product?.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
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
                await SaveChangesAsync();

                _logger.LogInformation("Product deleted from repository with ID {ProductId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product from repository with ID {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return false;
                }

                return await _context.Products.AnyAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if product exists in repository with ID {ProductId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> SearchAsync(string? search, decimal? minPrice, decimal? maxPrice)
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

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products in repository with search='{Search}', minPrice={MinPrice}, maxPrice={MaxPrice}", 
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
                _logger.LogError(ex, "Error checking stock availability in repository for product {ProductId}, quantity {Quantity}", 
                    productId, quantity);
                throw;
            }
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantityChange)
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

                var newStock = product.Stock + quantityChange;
                if (newStock < 0)
                {
                    throw new InvalidOperationException(ErrorMessages.Product.INSUFFICIENT_STOCK);
                }

                product.Stock = newStock;
                await SaveChangesAsync();

                _logger.LogInformation("Stock updated in repository for product {ProductId}: {OldStock} -> {NewStock}", 
                    productId, product.Stock - quantityChange, product.Stock);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock in repository for product {ProductId}, quantity change {QuantityChange}", 
                    productId, quantityChange);
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes in repository");
                throw;
            }
        }
    }
}
