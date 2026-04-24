using ECommerceCore.Application.DTOs.CustomerDTO;
using ECommerceCore.Application.Interfaces.CustomerInterface;
using ECommerceCore.Application.Services.CustomerServices.Interfaces;
using ECommerceCore.Domain.Enities;
using System;
using ECommerceCore.Application.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceCore.Application.Common;
namespace ECommerceCore.Application.Services.CustomerServices.Services
{
    public class CustomerServices : ICustomerServices
    {
        private readonly ICustomerRepository _repo;
        private readonly JwtService _jwtService;

        public CustomerServices(ICustomerRepository repo, JwtService jwtService)
        {
            _repo = repo;
            _jwtService = jwtService;
        }

        public async Task<CustomerResponseDto> AddCustomerAsync(CreateCustomerDto request)
        {
            var exist = await _repo.GetCustomerByEmailAsync(request.Email);
            if (exist != null)
                throw new BadRequestException("Email already exists");

            var customer = new Customer
            {
                Name = request.Name,
                Email = request.Email,
                Role = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            var created = await _repo.AddCustomerAsync(customer);
            return new CustomerResponseDto
            {
                Id = created.Id,
                Name = created.Name,
                Email = created.Email,
                Role = created.Role,
            };
        }

        public async Task<CustomerResponseDto> CustomerLoginAsync(LoginCustomerDto login)
        {
            var existing = await _repo.GetCustomerByEmailAsync(login.Email);
            if (existing == null)
                throw new NotFoundException("Account not found");

            if (!BCrypt.Net.BCrypt.Verify(login.Password, existing.PasswordHash))
                throw new BadRequestException("Invalid credentials");

            //  generate both tokens
            var accessToken = _jwtService.GenerateToken(existing);
            var refreshToken = _jwtService.GeneraterefreshToken();

            // save refresh token to database
            existing.RefreshToken = refreshToken;
            existing.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _repo.UpdateCustomerAsync(existing);

            return new CustomerResponseDto
            {
                Id = existing.Id,
                Name = existing.Name,
                Email = existing.Email,
                Role = existing.Role,
                Token = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = existing.RefreshTokenExpiry.Value
            };
        }

        public async Task<CustomerResponseDto> RefreshTokenAsync(RefreshTokenDto request)
        {
            var customer = await _repo.GetCustomerByRefreshTokenAsync(request.RefreshToken);
            if (customer == null)
                throw new BadRequestException("Invalid refresh token");

            if (customer.RefreshTokenExpiry < DateTime.UtcNow)
                throw new BadRequestException("Refresh token has expired, please login again");

            var newAccessToken = _jwtService.GenerateToken(customer);
            var newRefreshToken = _jwtService.GeneraterefreshToken();

            customer.RefreshToken = newRefreshToken;
            customer.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _repo.UpdateCustomerAsync(customer);

            return new CustomerResponseDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Role = customer.Role,
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiry = customer.RefreshTokenExpiry.Value
            };
        }

        public async Task<bool> RevokeTokenAsync(int customerId)
        {
            var customer = await _repo.GetCustomerByIdAsync(customerId);
            if (customer == null)
                throw new NotFoundException("Customer not found");

            customer.RefreshToken = null;
            customer.RefreshTokenExpiry = null;
            await _repo.UpdateCustomerAsync(customer);
            return true;
        }

        public async Task<CustomerProfileResponseDto> GetProfileAsync(int customerId)
        {
            var customer = await _repo.GetCustomerByIdAsync(customerId);
            if (customer == null)
                throw new NotFoundException("Customer not found");
            return MapToProfile(customer);
        }

        public async Task<CustomerProfileResponseDto> UpdateProfileAsync(int customerId, UpdateProfileDto request)
        {
            var customer = await _repo.GetCustomerByIdAsync(customerId);
            if (customer == null)
                throw new NotFoundException("Customer not found");

            if (customer.Email != request.Email)
            {
                var emailExists = await _repo.GetCustomerByEmailAsync(request.Email);
                if (emailExists != null)
                    throw new BadRequestException("Email already in use");
            }

            customer.Name = request.Name;
            customer.Email = request.Email;

            var updated = await _repo.UpdateCustomerAsync(customer);
            return MapToProfile(updated);
        }

        public async Task<bool> ChangePasswordAsync(int customerId, ChangePasswordDto request)
        {
            var customer = await _repo.GetCustomerByIdAsync(customerId);
            if (customer == null)
                throw new NotFoundException("Customer not found");

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, customer.PasswordHash))
                throw new BadRequestException("Current password is incorrect");

            if (request.NewPassword != request.ConfirmPassword)
                throw new BadRequestException("New passwords do not match");

            customer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _repo.UpdateCustomerAsync(customer);
            return true;
        }

        public async Task<bool> DeleteAccountAsync(int customerId)
        {
            var customer = await _repo.GetCustomerByIdAsync(customerId);
            if (customer == null)
                throw new NotFoundException("Customer not found");
            return await _repo.DeleteCustomerAsync(customerId);
        }

        public async Task<IEnumerable<CustomerProfileResponseDto>> GetAllCustomersAsync()
        {
            var customers = await _repo.GetAllCustomersAsync();
            return customers.Select(MapToProfile);
        }

        private CustomerProfileResponseDto MapToProfile(Customer customer) => new CustomerProfileResponseDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Role = customer.Role,
            CreatedAt = customer.CreatedAt
        };
    }
}