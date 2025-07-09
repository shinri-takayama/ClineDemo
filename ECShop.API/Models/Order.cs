using System.ComponentModel.DataAnnotations;

namespace ECShop.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; }
        
        [Required]
        public OrderStatus Status { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string ShippingName { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string ShippingPostalCode { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string ShippingPrefecture { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string ShippingCity { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string ShippingAddressLine { get; set; }
        
        [MaxLength(20)]
        public string? ShippingPhone { get; set; }
        
        [MaxLength(500)]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public User User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
    
    public enum OrderStatus
    {
        Pending = 0,      // 注文確認中
        Confirmed = 1,    // 注文確定
        Processing = 2,   // 処理中
        Shipped = 3,      // 発送済み
        Delivered = 4,    // 配送完了
        Cancelled = 5     // キャンセル
    }
}
