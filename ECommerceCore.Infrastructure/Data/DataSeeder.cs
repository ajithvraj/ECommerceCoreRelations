using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Infrastructure.Data
{
    using ECommerceCore.Domain.Enities;
    using ECommerceCore.Infrastructure.Persistance.Data;
    using Microsoft.EntityFrameworkCore;

    public class DataSeeder
    {
        public static async Task SeedAdminAsync(AppDbContext context)
        {
            // Check if any customer exists
            if (await context.Customers.AnyAsync())
                return;

            // Create default admin
            var admin = new Customer
            {
                Name = "Admin",
                Email = "admin@gmail.com",
                Role = "Admin"
            };

            await context.Customers.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
