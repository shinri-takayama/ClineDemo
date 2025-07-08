using Microsoft.EntityFrameworkCore;
using ECShop.API.Models;

namespace ECShop.API.Data
{
    public class ECShopContext : DbContext
    {
        public ECShopContext(DbContextOptions<ECShopContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
            });

            // Seed data
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "iPhone 15 Pro",
                    Description = "最新のiPhone 15 Pro。高性能なA17 Proチップを搭載し、プロレベルの写真撮影が可能です。",
                    Price = 159800m,
                    ImageUrl = "https://via.placeholder.com/300x300/007bff/ffffff?text=iPhone+15+Pro",
                    Stock = 50,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 2,
                    Name = "MacBook Air M3",
                    Description = "薄くて軽い、パワフルなMacBook Air。M3チップで驚異的なパフォーマンスを実現。",
                    Price = 164800m,
                    ImageUrl = "https://via.placeholder.com/300x300/28a745/ffffff?text=MacBook+Air",
                    Stock = 30,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 3,
                    Name = "AirPods Pro",
                    Description = "アクティブノイズキャンセリング機能付きの完全ワイヤレスイヤホン。",
                    Price = 39800m,
                    ImageUrl = "https://via.placeholder.com/300x300/dc3545/ffffff?text=AirPods+Pro",
                    Stock = 100,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 4,
                    Name = "iPad Pro 12.9インチ",
                    Description = "プロ仕様のiPad。M2チップ搭載で、クリエイティブな作業に最適。",
                    Price = 172800m,
                    ImageUrl = "https://via.placeholder.com/300x300/ffc107/000000?text=iPad+Pro",
                    Stock = 25,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 5,
                    Name = "Apple Watch Series 9",
                    Description = "健康とフィットネスを追跡する最新のスマートウォッチ。",
                    Price = 59800m,
                    ImageUrl = "https://via.placeholder.com/300x300/6f42c1/ffffff?text=Apple+Watch",
                    Stock = 75,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 6,
                    Name = "Magic Keyboard",
                    Description = "iPadとMacに対応する高品質なワイヤレスキーボード。",
                    Price = 19800m,
                    ImageUrl = "https://via.placeholder.com/300x300/20c997/ffffff?text=Magic+Keyboard",
                    Stock = 60,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
