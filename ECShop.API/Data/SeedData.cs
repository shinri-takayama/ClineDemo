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

            // 3) サンプル注文が無ければ投入
            if (!await context.Orders.AnyAsync())
            {
                var user = await context.Users.OrderBy(u => u.Id).FirstAsync();
                var products = await context.Products.OrderBy(p => p.Id).ToListAsync();

                if (products.Count >= 3)
                {
                    var now = DateTime.UtcNow;

                    // 注文1: 注文確認中
                    var o1 = new Order
                    {
                        UserId             = user.Id,
                        OrderDate          = now.AddMinutes(-20),
                        UpdatedAt          = now.AddMinutes(-18),
                        Status             = OrderStatus.Pending, // 0 = 注文確認中
                        ShippingName       = "高山 真理",
                        ShippingPostalCode = "2200022",
                        ShippingPrefecture = "神奈川県",
                        ShippingCity       = "横浜市西区",
                        ShippingAddressLine= "花咲町（４〜７丁目）",
                        ShippingPhone      = "12312345678",
                        Notes              = null,
                        CreatedAt          = now,
                        OrderItems = new List<OrderItem>
                        {
                            new OrderItem { ProductId = products[1].Id, Quantity = 2, Price = products[1].Price }, // MacBook Air M3
                            new OrderItem { ProductId = products[0].Id, Quantity = 2, Price = products[0].Price }, // AirPods Pro
                            new OrderItem { ProductId = products[2].Id, Quantity = 1, Price = products[2].Price }  // Magic Keyboard
                        }
                    };
                    o1.TotalAmount = o1.OrderItems.Sum(i => i.Price * i.Quantity);

                    // 注文2: 配送完了
                    var o2 = new Order
                    {
                        UserId             = user.Id,
                        OrderDate          = now.AddMinutes(-40),
                        UpdatedAt          = now.AddMinutes(-30),
                        Status             = OrderStatus.Delivered, // 4 = 配送完了
                        ShippingName       = "高山 真理",
                        ShippingPostalCode = "2200022",
                        ShippingPrefecture = "神奈川県",
                        ShippingCity       = "横浜市西区",
                        ShippingAddressLine= "花咲町（４〜７丁目）",
                        ShippingPhone      = "12312345678",
                        Notes              = null,
                        CreatedAt          = now,
                        OrderItems = new List<OrderItem>
                        {
                            new OrderItem { ProductId = products.Last().Id, Quantity = 1, Price = 31500m, Product = new Product { Name="shinri Pro１", Description="とおれ！！", Price=31500m, Stock=5, CreatedAt=now } }
                        }
                    };
                    o2.TotalAmount = o2.OrderItems.Sum(i => i.Price * i.Quantity);

                    context.Orders.AddRange(o1, o2);
                    await context.SaveChangesAsync();

                    Console.WriteLine("サンプル注文を投入しました。件数: 2");
                }
                else
                {
                    Console.WriteLine("サンプル注文の投入をスキップ：商品が3点未満です。");
                }
            }

        }
    }
}
