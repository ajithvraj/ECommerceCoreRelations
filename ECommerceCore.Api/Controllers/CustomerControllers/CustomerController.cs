using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerceCore.Application.Services.CustomerServices.Interfaces;
using ECommerceCore.Application.DTOs.CustomerDTO;
using ECommerceCore.Application.Common;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;

namespace ECommerceCore.Api.Controllers.CustomerControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerServices _custumer;

        public CustomerController(ICustomerServices customer)
        {
            _custumer = customer;
        }

        private int GetCustomerId() =>
            int.Parse(User.FindFirstValue("UserId")
                ?? throw new UnauthorizedAccessException("User not found"));

        // Public
        [HttpPost("Register")]
        [EnableRateLimiting("login")] //max 5/minute
        public async Task<IActionResult> CreateAccount(CreateCustomerDto dto)
        {
            var account = await _custumer.AddCustomerAsync(dto);
            return Ok(ApiResponse<CustomerResponseDto>.SuccessResult(account));
        }

        //  Public
        [HttpPost("Login")]
        [EnableRateLimiting("login")] //max 5/minute
        public async Task<IActionResult> Login(LoginCustomerDto log)
        {
            var login = await _custumer.CustomerLoginAsync(log);
            return Ok(ApiResponse<CustomerResponseDto>.SuccessResult(login));
        }

        // Public
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto request)
        {
            var result = await _custumer.RefreshTokenAsync(request);
            return Ok(ApiResponse<CustomerResponseDto>.SuccessResult(result, "Token refreshed successfully"));
        }

        //  Authorized
        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken()
        {
            var customerId = GetCustomerId();
            var result = await _custumer.RevokeTokenAsync(customerId);
            return Ok(ApiResponse<bool>.SuccessResult(result, "Logged out successfully"));
        }

        // Customer
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var customerId = GetCustomerId();
            var result = await _custumer.GetProfileAsync(customerId);
            return Ok(ApiResponse<CustomerProfileResponseDto>.SuccessResult(result, "Profile retrieved successfully"));
        }

        // Customer
        [Authorize]
        [HttpPut("profile/update")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto request)
        {
            var customerId = GetCustomerId();
            var result = await _custumer.UpdateProfileAsync(customerId, request);
            return Ok(ApiResponse<CustomerProfileResponseDto>.SuccessResult(result, "Profile updated successfully"));
        }

        //  Customer
        [Authorize]
        [HttpPatch("profile/change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto request)
        {
            var customerId = GetCustomerId();
            var result = await _custumer.ChangePasswordAsync(customerId, request);
            return Ok(ApiResponse<bool>.SuccessResult(result, "Password changed successfully"));
        }

        //  Customer
        [Authorize]
        [HttpDelete("profile/delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            var customerId = GetCustomerId();
            var result = await _custumer.DeleteAccountAsync(customerId);
            return Ok(ApiResponse<bool>.SuccessResult(result, "Account deleted successfully"));
        }

        //  Admin
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _custumer.GetAllCustomersAsync();
            return Ok(ApiResponse<IEnumerable<CustomerProfileResponseDto>>.SuccessResult(result, "Customers retrieved successfully"));
        }
    }
}