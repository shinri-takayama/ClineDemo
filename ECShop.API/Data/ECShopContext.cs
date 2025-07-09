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
        public DbSet<User> Users { get; set; }

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

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Seed data
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "iPhone 15 Pro",
                    Description = "最新のiPhone 15 Pro。高性能なA17 Proチップを搭載し、プロレベルの写真撮影が可能です。",
                    Price = 159800m,
                    ImageUrl = "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/iphone-15-pro-naturaltitanium-select?wid=470&hei=556&fmt=png-alpha&.v=1693009279145",
                    Stock = 50,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 2,
                    Name = "MacBook Air M3",
                    Description = "薄くて軽い、パワフルなMacBook Air。M3チップで驚異的なパフォーマンスを実現。",
                    Price = 164800m,
                    ImageUrl = "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/macbook-air-midnight-select-20220606?wid=904&hei=840&fmt=jpeg&qlt=90&.v=1653084303665",
                    Stock = 30,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 3,
                    Name = "AirPods Pro",
                    Description = "アクティブノイズキャンセリング機能付きの完全ワイヤレスイヤホン。",
                    Price = 39800m,
                    ImageUrl = "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/MQD83?wid=572&hei=572&fmt=jpeg&qlt=95&.v=1660803972361",
                    Stock = 100,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 6,
                    Name = "Magic Keyboard",
                    Description = "iPadとMacに対応する高品質なワイヤレスキーボード。",
                    Price = 19800m,
                    ImageUrl = "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/MK2A3?wid=572&hei=572&fmt=jpeg&qlt=95&.v=1628010471000",
                    Stock = 60,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
