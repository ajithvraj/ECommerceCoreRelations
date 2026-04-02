using ECommerceCore.Application.DTOs.CustomerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.Services.CustomerServices.Interfaces
{
    public interface ICustomerServices
    {
        Task<CustomerResponseDto>AddCustomerAsync(CreateCustomerDto request);
        Task<CustomerResponseDto> CustomerLoginAsync(LoginCustomerDto login);
    


    }
}
