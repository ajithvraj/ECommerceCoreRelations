using ECommerceCore.Application.Common;
using ECommerceCore.Application.DTOs.OrderDTO;
using ECommerceCore.Application.Services.Orderservice.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceCore.Api.Controllers.OrderController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {

        private readonly IOrderServices _orderservice;

        public OrderController(IOrderServices orderservice)
        {
            _orderservice = orderservice;
        }

        private int GetCustomerId() => int.Parse(User.FindFirstValue("User") ?? throw new UnauthorizedAccessException("User not found"));

        [HttpPost("place")]

        public async Task<IActionResult> PlaceOrder(PlaceOrderDto request)
        {
            var customerId = GetCustomerId();
            var result = await _orderservice.PlaceOrderAsync(customerId, request);
            return Ok(ApiResponse<OrderResponseDto>.SuccessResult(result, "Order placed successfully"));

        }

        [HttpGet("my-orders")]

        public async Task<IActionResult> GetOrders()
        {
            int customerId = GetCustomerId();
            var result = await _orderservice.GetMyOrdersAsync(customerId);
            return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.SuccessResult(result, "Orders retrieved successfully"));

        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            int customerId = GetCustomerId();
            var result = await _orderservice.GetOrderByIdAsync(customerId, orderId);
            return Ok(ApiResponse<OrderResponseDto>.SuccessResult(result, "Order retrieved succcessfully"));

        }

        [HttpPatch("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            int customerId = GetCustomerId();
            var result = await _orderservice.CancelOrderAsync(customerId, orderId);
            return Ok(ApiResponse<bool>.SuccessResult(result, "Order cancelled successfully"));

        }

        [Authorize(Roles = "Admin")]

        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderservice.GetAllOrdersAsync();
            return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.SuccessResult(result, "All orders retrieved sucessfully"));

        }

        [Authorize(Roles = "Admin")]

        [HttpPatch("status/{orderId}")] 

        public async Task<IActionResult> UpdateOrderStaus(int orderId ,UpdateOrderStatusDto request)
        {
            var result = await _orderservice.UpdateOrderStatusAsync(orderId, request);
            return Ok(ApiResponse<OrderResponseDto>.SuccessResult(result, "Order status updated"));


        }

        








    }
}
