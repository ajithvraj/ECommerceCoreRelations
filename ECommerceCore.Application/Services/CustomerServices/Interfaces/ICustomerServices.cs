using ECommerceCore.Application.DTOs.CustomerDTO;
using ECommerceCore.Domain.Enities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.Services.CustomerServices.Interfaces
{
    public interface ICustomerServices
    {
        Task<CustomerResponseDto> AddCustomerAsync(CreateCustomerDto request);
        Task<CustomerResponseDto> CustomerLoginAsync(LoginCustomerDto login);
        Task<CustomerResponseDto> RefreshTokenAsync(RefreshTokenDto request);
        Task<bool> RevokeTokenAsync(int customerId);

       
        Task<CustomerProfileResponseDto> GetProfileAsync(int customerId);
        Task<CustomerProfileResponseDto> UpdateProfileAsync(int customerId, UpdateProfileDto request);
        Task<bool> ChangePasswordAsync(int customerId, ChangePasswordDto request);
        Task<bool> DeleteAccountAsync(int customerId);

        Task<IEnumerable<CustomerProfileResponseDto>> GetAllCustomersAsync();
    }



}

