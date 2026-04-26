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
            // only seed if no admin exists — don't clear data on every restart
            if (await context.Customers.AnyAsync(c => c.Role == "Admin"))
                return;

            // seed fresh admin with hashed password
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