using BCrypt.Net;
using ECommerceCore.Domain.Enities;
using ECommerceCore.Infrastructure.Persistance.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceCore.Infrastructure.Data
{
    public class DataSeeder
    {
        public static async Task SeedAdminAsync(AppDbContext context)
        {
            //  clear existing data in correct order (respect foreign keys)
            context.OrderItems.RemoveRange(context.OrderItems);
            context.Orders.RemoveRange(context.Orders);
            context.CartItems.RemoveRange(context.CartItems);
            context.Products.RemoveRange(context.Products);
            context.Customers.RemoveRange(context.Customers);
            await context.SaveChangesAsync();

            // ✅ reset identity seeds so IDs start from 1 again
            await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Customers', RESEED, 0)");
            await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Products', RESEED, 0)");
            await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Orders', RESEED, 0)");
            await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('OrderItems', RESEED, 0)");
            await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('CartItems', RESEED, 0)");

            // ✅ seed fresh admin with hashed password
            var admin = new Customer
            {
                Name = "Admin",
                Email = "admin@gmail.com",
                Role = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                CreatedAt = DateTime.UtcNow
            };

            await context.Customers.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}