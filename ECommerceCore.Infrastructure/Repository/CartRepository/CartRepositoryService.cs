using ECommerceCore.Application.Interfaces.CartInterface;
using ECommerceCore.Domain.Enities;
using ECommerceCore.Infrastructure.Persistance.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceCore.Infrastructure.Repository.CartRepository
{
    public class CartRepositoryServices : ICartRepository
    {
        private readonly AppDbContext _db;

        public CartRepositoryServices(AppDbContext db)
        {
            _db = db;
        }

        public async Task<CartItem?> GetCartItemAsync(int customerId, int productId)
        {
            return await _db.CartItems
                .FirstOrDefaultAsync(c => c.CustomerId == customerId
                                       && c.ProductId == productId);
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(int customerId)
        {
            return await _db.CartItems
                .Include(c => c.Product) // load product details
                .Where(c => c.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<CartItem> AddCartItemAsync(CartItem item)
        {
            await _db.CartItems.AddAsync(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<CartItem> UpdateCartItemAsync(CartItem item)
        {
            _db.CartItems.Update(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<bool> RemoveCartItemAsync(int cartItemId, int customerId)
        {
            var item = await _db.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId
                                       && c.CustomerId == customerId);
            if (item == null) return false;

            _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(int customerId)
        {
            var items = await _db.CartItems
                .Where(c => c.CustomerId == customerId)
                .ToListAsync();

            if (!items.Any()) return false;

            _db.CartItems.RemoveRange(items);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}