using ECommerceCore.Domain.Enities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceCore.Infrastructure.Persistance.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; } // ✅ added
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customer → Orders
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(m => m.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order → OrderItems
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            // OrderItem → Product
            modelBuilder.Entity<OrderItem>()
                .HasOne(p => p.Product)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(p => p.ProductId);

            // CartItem → Product
            modelBuilder.Entity<CartItem>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // CartItem → Customer
            modelBuilder.Entity<CartItem>()
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // CartItem unique index
            modelBuilder.Entity<CartItem>()
                .HasIndex(c => new { c.CustomerId, c.ProductId })
                .IsUnique();

            // Customer → Addresses
            modelBuilder.Entity<Address>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order → Address
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Address)
                .WithMany()
                .HasForeignKey(o => o.AdressId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product → ProductImages
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}