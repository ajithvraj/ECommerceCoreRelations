using ECommerceCore.Application.DTOs.CartDTO;
using ECommerceCore.Application.Exceptions;
using ECommerceCore.Application.Interfaces.CartInterface;
using ECommerceCore.Application.Interfaces.ProductInterface;
using ECommerceCore.Application.Services.CartServices.Interfaces;
using ECommerceCore.Domain.Enities;

namespace ECommerceCore.Application.Services.CartServices.Services
{
    public class CartServices : ICartServices
    {
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;

        public CartServices(ICartRepository cartRepo, IProductRepository productRepo)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
        }

        public async Task<CartResponseDto> AddToCartAsync(int customerId, AddToCartDto request)
        {
            // check product exists and is active
            var product = await _productRepo.GetProductByIdAsync(request.ProductId);
            if (product == null)
                throw new NotFoundException("Product not found");

            // check stock
            if (product.Stock < request.Quantity)
                throw new BadRequestException($"Only {product.Stock} items available in stock");

            // check if item already in cart
            var existingItem = await _cartRepo.GetCartItemAsync(customerId, request.ProductId);
            if (existingItem != null)
            {
                // update quantity instead of adding duplicate
                existingItem.Quantity += request.Quantity;

                // recheck stock with updated quantity
                if (product.Stock < existingItem.Quantity)
                    throw new BadRequestException($"Only {product.Stock} items available in stock");

                await _cartRepo.UpdateCartItemAsync(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CustomerId = customerId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };
                await _cartRepo.AddCartItemAsync(cartItem);
            }

            return await GetCartAsync(customerId);
        }

        public async Task<CartResponseDto> GetCartAsync(int customerId)
        {
            var items = await _cartRepo.GetCartItemsAsync(customerId);

            var cartItems = items.Select(item => new CartItemResponseDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? string.Empty,
                ProductImage = item.Product?.ImageUrl ?? string.Empty,
                Price = item.Product?.Price ?? 0,
                Quantity = item.Quantity,
                TotalPrice = (item.Product?.Price ?? 0) * item.Quantity
            }).ToList();

            return new CartResponseDto
            {
                Items = cartItems,
                GrandTotal = cartItems.Sum(i => i.TotalPrice),
                TotalItems = cartItems.Sum(i => i.Quantity)
            };
        }

        public async Task<CartResponseDto> UpdateCartItemAsync(int customerId, int cartItemId, UpdateCartDto request)
        {
            var cartItems = await _cartRepo.GetCartItemsAsync(customerId);
            var item = cartItems.FirstOrDefault(i => i.Id == cartItemId);

            if (item == null)
                throw new NotFoundException("Cart item not found");

            // check stock
            var product = await _productRepo.GetProductByIdAsync(item.ProductId);
            if (product == null)
                throw new NotFoundException("Product not found");

            if (product.Stock < request.Quantity)
                throw new BadRequestException($"Only {product.Stock} items available in stock");

            item.Quantity = request.Quantity;
            await _cartRepo.UpdateCartItemAsync(item);

            return await GetCartAsync(customerId);
        }

        public async Task<bool> RemoveCartItemAsync(int customerId, int cartItemId)
        {
            return await _cartRepo.RemoveCartItemAsync(cartItemId, customerId);
        }

        public async Task<bool> ClearCartAsync(int customerId)
        {
            return await _cartRepo.ClearCartAsync(customerId);
        }
        public async Task<CartResponseDto> IncreaseQuantityAsync(int customerId, int cartItemId)
        {
            var items = await _cartRepo.GetCartItemsAsync(customerId);
            var item = items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null)
                throw new NotFoundException("Cart item not found");

            var product = await _productRepo.GetProductByIdAsync(item.ProductId);
            if (product == null)
                throw new NotFoundException("Product not found");

            if (item.Quantity >= product.Stock)
                throw new BadRequestException($"Only {product.Stock} items available in stock");

            item.Quantity += 1;
            await _cartRepo.UpdateCartItemAsync(item);
            return await GetCartAsync(customerId);
        }

        public async Task<CartResponseDto> DecreaseQuantityAsync(int customerId, int cartItemId)
        {
            var items = await _cartRepo.GetCartItemsAsync(customerId);
            var item = items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null)
                throw new NotFoundException("Cart item not found");

            if (item.Quantity <= 1)
            {
                // auto remove if quantity reaches 0
                await _cartRepo.RemoveCartItemAsync(cartItemId, customerId);
            }
            else
            {
                item.Quantity -= 1;
                await _cartRepo.UpdateCartItemAsync(item);
            }

            return await GetCartAsync(customerId);
        }
    }
}