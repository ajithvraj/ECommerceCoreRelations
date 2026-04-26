using ECommerceCore.Application.Common;
using ECommerceCore.Application.DTOs.CartDTO;
using ECommerceCore.Application.Services.CartServices.Interfaces;
using ECommerceCore.Application.Services.CartServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace ECommerceCore.Api.Controllers.AddToCartController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("global")]
    public class CartController : ControllerBase
    {

        private readonly ICartServices _cartservice; 

        public CartController (ICartServices cartservice)
        {
            _cartservice = cartservice;
        }

        //gets customer id from jwt token 

        private int GetCustomerId() =>
            int.Parse(User.FindFirstValue("UserId") ?? throw new UnauthorizedAccessException("User not found"));

        [HttpPost("add")] 

        public async Task<IActionResult> AddToCart(AddToCartDto request)
        {
            var customerId = GetCustomerId();

            var result = await _cartservice.AddToCartAsync(customerId, request);
            return Ok(ApiResponse<CartResponseDto>.SuccessResult(result,"Item added to cart"));

        }

        [HttpGet("getCart")] 

        public async Task<IActionResult> GetCart()
        {
            var customerId = GetCustomerId();
            var result = await _cartservice.GetCartAsync(customerId);
            return Ok(ApiResponse<CartResponseDto>.SuccessResult(result, "Cart retrieved successfully"));

        }

        [HttpPut("update/{cartItemId}")] 

        public async Task<IActionResult> UpdatecartItem(int cartaItemId, UpdateCartDto request)
        {
            var customerId = GetCustomerId();
            var result = await _cartservice.UpdateCartItemAsync(customerId, cartaItemId, request);
            return Ok(ApiResponse<CartResponseDto>.SuccessResult(result ,"Cart Updated Successfully"));


        }

        [HttpDelete("remove/{cartItemId}")] 

        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var customerId = GetCustomerId();

            var result = await _cartservice.RemoveCartItemAsync(customerId, cartItemId);
            return Ok(ApiResponse<bool>.SuccessResult(result,"Item removed successfully"));
        }

        [HttpDelete("all")] 

        public async Task<IActionResult> ClearCart()
        {
            var customerId = GetCustomerId();

            var result = await _cartservice.ClearCartAsync(customerId);
            return Ok(ApiResponse<bool>.SuccessResult(result,"Cart Cleared successfully"));


        }

        // increase quantity by 1
        [HttpPatch("increase/{cartItemId}")]
        public async Task<IActionResult> IncreaseQuantity(int cartItemId)
        {
            var customerId = GetCustomerId();
            var result = await _cartservice.IncreaseQuantityAsync(customerId, cartItemId);
            return Ok(ApiResponse<CartResponseDto>.SuccessResult(result, "Quantity increased"));
        }

        //removes item if quantity reaches 0

        [HttpPatch("decrease/{cartItemId}")]
        public async Task<IActionResult> DecreaseQuantity(int cartItemId)
        {
            var customerId = GetCustomerId();
            var result = await _cartservice.DecreaseQuantityAsync(customerId, cartItemId);
            return Ok(ApiResponse<CartResponseDto>.SuccessResult(result, "Quantity decreased"));
        }




    }
}
