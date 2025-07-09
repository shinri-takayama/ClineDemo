using ECShop.API.Models;

namespace ECShop.API.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product?> UpdateProductAsync(int id, Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<Product>> SearchProductsAsync(string? search, decimal? minPrice, decimal? maxPrice);
        Task<bool> IsStockAvailableAsync(int productId, int quantity);
        Task<bool> UpdateStockAsync(int productId, int quantity);
    }
}
