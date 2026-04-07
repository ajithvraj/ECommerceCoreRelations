using ECommerceCore.Application.DTOs.CartDTO;

namespace ECommerceCore.Application.Services.CartServices.Interfaces
{
    public interface ICartServices
    {
        Task<CartResponseDto> AddToCartAsync(int customerId, AddToCartDto request);
        Task<CartResponseDto> GetCartAsync(int customerId);
        Task<CartResponseDto> UpdateCartItemAsync(int customerId, int cartItemId, UpdateCartDto request);
        Task<bool> RemoveCartItemAsync(int customerId, int cartItemId);
        Task<bool> ClearCartAsync(int customerId);
        Task<CartResponseDto> IncreaseQuantityAsync(int customerId, int cartItemId);
        Task<CartResponseDto> DecreaseQuantityAsync(int customerId , int cartItemId);
    }
}