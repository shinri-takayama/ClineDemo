using ECShop.API.Models;

namespace ECShop.API.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Product>> SearchAsync(string? search, decimal? minPrice, decimal? maxPrice);
        Task<bool> IsStockAvailableAsync(int productId, int quantity);
        Task<bool> UpdateStockAsync(int productId, int quantityChange);
        Task SaveChangesAsync();
    }
}
