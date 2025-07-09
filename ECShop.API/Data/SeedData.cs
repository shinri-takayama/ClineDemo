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

            // データベースが作成されていることを確認
            await context.Database.EnsureCreatedAsync();

            // 既に管理者ユーザーが存在するかチェック
            if (await context.Users.AnyAsync(u => u.IsAdmin))
            {
                return; // 既に管理者が存在する場合は何もしない
            }

            // デフォルト管理者ユーザーを作成
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@ecshop.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // デフォルトパスワード
                FirstName = "管理者",
                LastName = "システム",
                IsAdmin = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();

            Console.WriteLine("デフォルト管理者ユーザーが作成されました:");
            Console.WriteLine($"ユーザー名: {adminUser.Username}");
            Console.WriteLine($"パスワード: admin123");
            Console.WriteLine($"メール: {adminUser.Email}");
            Console.WriteLine("セキュリティのため、初回ログイン後にパスワードを変更してください。");
        }
    }
}
