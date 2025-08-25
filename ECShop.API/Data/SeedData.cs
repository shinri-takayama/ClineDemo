using ECShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ECShop.API.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ECShopContext(
                serviceProvider.GetRequiredService<DbContextOptions<ECShopContext>>());

            // DB 作成（マイグレーションを使っている場合は EnsureCreated でなく Migrate 推奨）
            await context.Database.EnsureCreatedAsync();

            // 1) 管理者ユーザーが無ければ作成（ここで return しない＝以下のシードも継続）
            if (!await context.Users.AnyAsync(u => u.IsAdmin))
            {
                var adminUser = new User
                {
                    
                    Username    = "admin",
                    Email       = "admin@ecshop.com",
                    PasswordHash= BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FirstName   = "管理者",
                    LastName    = "システム",
                    IsAdmin     = true,
                    CreatedAt   = DateTime.UtcNow
                };
                context.Users.Add(adminUser);
                await context.SaveChangesAsync();

                Console.WriteLine("デフォルト管理者ユーザーを作成しました。");
                Console.WriteLine("  ユーザー名: admin / パスワード: admin123（初回変更推奨）");
            }

            // 2) サンプル商品が無ければ投入
            if (!await context.Products.AnyAsync())
            {
                var now = DateTime.UtcNow;
                var products = new List<Product>
                {
                    new Product { Name="AirPods Pro",      Description="ANC イヤホン",           Price=39800m,  Stock=84, ImageUrl="https://example.com/airpods-pro.jpg",      CreatedAt=now },
                    new Product { Name="MacBook Air M3",   Description="13インチ ノートPC",       Price=164800m, Stock=15, ImageUrl="https://example.com/macbook-air-m3.jpg",   CreatedAt=now },
                    new Product { Name="Magic Keyboard",   Description="テンキー付き キーボード", Price=19800m,  Stock=47, ImageUrl="https://example.com/magic-keyboard.jpg",   CreatedAt=now },
                    new Product { Name="iPhone 15 Pro",    Description="A17 Pro チップ",         Price=159800m, Stock=47, ImageUrl="https://example.com/iphone-15-pro.jpg",    CreatedAt=now }
                };
                context.Products.AddRange(products);
                await context.SaveChangesAsync();

                Console.WriteLine("サンプル商品を投入しました。件数: {0}", products.Count);
            }

            // 3) サンプル注文が無ければ投入（ユーザー1名・商品2品以上が前提）
            if (!await context.Orders.AnyAsync())
            {
                var user = await context.Users.OrderBy(u => u.Id).FirstAsync();
                var products = await context.Products.OrderBy(p => p.Id).ToListAsync();
                if (products.Count >= 2)
                {
                    var p1 = products[0];
                    var p2 = products[1];
                    var p3 = products.Count > 2 ? products[2] : products[1];

                    // 注文1：処理中
                    var o1 = new Order
                    {
                        UserId       = user.Id,
                        ShippingName = $"{(string.IsNullOrWhiteSpace(user.FirstName) ? "山田" : user.FirstName)} {(string.IsNullOrWhiteSpace(user.LastName) ? "太郎" : user.LastName)}",
                        OrderDate    = DateTime.UtcNow.AddDays(-2),
                        UpdatedAt    = DateTime.UtcNow.AddDays(-1),
                        Status       = OrderStatus.Processing,
                        OrderItems   = new List<OrderItem>
                        {
                            new OrderItem { ProductId = p1.Id, Quantity = 1, Price = p1.Price },
                            new OrderItem { ProductId = p2.Id, Quantity = 2, Price = p2.Price }
                        }
                    };
                    o1.TotalAmount = o1.OrderItems.Sum(i => i.Price * i.Quantity);

                    // 注文2：発送済み
                    var o2 = new Order
                    {
                        UserId       = user.Id,
                        ShippingName = o1.ShippingName,
                        OrderDate    = DateTime.UtcNow.AddDays(-5),
                        UpdatedAt    = DateTime.UtcNow.AddDays(-3),
                        Status       = OrderStatus.Shipped,
                        OrderItems   = new List<OrderItem>
                        {
                            new OrderItem { ProductId = p3.Id, Quantity = 1, Price = p3.Price }
                        }
                    };
                    o2.TotalAmount = o2.OrderItems.Sum(i => i.Price * i.Quantity);

                    // 注文3：配送完了
                    var o3 = new Order
                    {
                        UserId       = user.Id,
                        ShippingName = o1.ShippingName,
                        OrderDate    = DateTime.UtcNow.AddDays(-9),
                        UpdatedAt    = DateTime.UtcNow.AddDays(-7),
                        Status       = OrderStatus.Delivered,
                        OrderItems   = new List<OrderItem>
                        {
                            new OrderItem { ProductId = p2.Id, Quantity = 1, Price = p2.Price },
                            new OrderItem { ProductId = p1.Id, Quantity = 1, Price = p1.Price }
                        }
                    };
                    o3.TotalAmount = o3.OrderItems.Sum(i => i.Price * i.Quantity);

                    context.Orders.AddRange(o1, o2, o3);
                    await context.SaveChangesAsync();

                    Console.WriteLine("サンプル注文を投入しました。件数: 3");
                }
                else
                {
                    Console.WriteLine("サンプル注文の投入をスキップ：商品が2点未満です。");
                }
            }
        }
    }
}
