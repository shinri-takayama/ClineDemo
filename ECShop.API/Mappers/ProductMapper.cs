using ECShop.API.Models;
using ECShop.API.DTOs;

namespace ECShop.API.Mappers
{
    public static class ProductMapper
    {
        // Model to DTO mappings
        public static ProductResponse ToResponse(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Stock = product.Stock,
                CreatedAt = product.CreatedAt
            };
        }

        public static ProductSummaryResponse ToSummaryResponse(Product product)
        {
            return new ProductSummaryResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                IsInStock = product.Stock > 0
            };
        }

        public static IEnumerable<ProductResponse> ToResponseList(IEnumerable<Product> products)
        {
            return products.Select(ToResponse);
        }

        public static IEnumerable<ProductSummaryResponse> ToSummaryResponseList(IEnumerable<Product> products)
        {
            return products.Select(ToSummaryResponse);
        }

        public static ProductListResponse ToListResponse(IEnumerable<Product> products, int totalCount, int page, int pageSize)
        {
            return new ProductListResponse
            {
                Products = ToResponseList(products),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // DTO to Model mappings
        public static Product ToModel(CreateProductRequest request)
        {
            return new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                Stock = request.Stock,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static Product ToModel(UpdateProductRequest request, int id)
        {
            return new Product
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                Stock = request.Stock
            };
        }

        public static void UpdateModel(Product product, UpdateProductRequest request)
        {
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.ImageUrl = request.ImageUrl;
            product.Stock = request.Stock;
        }
    }
}
