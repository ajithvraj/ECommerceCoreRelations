using ECommerceCore.Domain.Enities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.Interfaces.CartInterface
{
    public interface ICartRepository
    {
        Task<CartItem?> GetCartItemAsync(int customerId, int productId);
        Task<IEnumerable<CartItem>> GetCartItemsAsync(int customerId);
        Task<CartItem> AddCartItemAsync(CartItem item);
        Task<CartItem> UpdateCartItemAsync(CartItem item);
        Task<bool> RemoveCartItemAsync(int cartItemId, int customerId);
        Task<bool> ClearCartAsync(int customerId);
    }
}
