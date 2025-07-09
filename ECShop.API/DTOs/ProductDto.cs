using System.ComponentModel.DataAnnotations;

namespace ECShop.API.DTOs
{
    // Request DTOs
    public class CreateProductRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [StringLength(200)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be 0 or greater")]
        public int Stock { get; set; }
    }

    public class UpdateProductRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [StringLength(200)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be 0 or greater")]
        public int Stock { get; set; }
    }

    public class ProductSearchRequest
    {
        public string? Search { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "MinPrice must be 0 or greater")]
        public decimal? MinPrice { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "MaxPrice must be 0 or greater")]
        public decimal? MaxPrice { get; set; }
        
        public string? SortBy { get; set; }
        
        public string SortOrder { get; set; } = "asc";
    }

    // Response DTOs
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsInStock => Stock > 0;
    }

    public class ProductSummaryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsInStock { get; set; }
    }

    public class ProductListResponse
    {
        public IEnumerable<ProductResponse> Products { get; set; } = new List<ProductResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage => (Page * PageSize) < TotalCount;
        public bool HasPreviousPage => Page > 1;
    }
}
