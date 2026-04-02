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

        public CustomerServices(ICustomerRepository repo , JwtService jwtService)
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

            if (existing.Name != login.Name)
                throw new BadRequestException("Invalid credentials");
            var token = _jwtService.GenerateToken(existing); 

            return new CustomerResponseDto
            {
                Id = existing.Id,
                Name = existing.Name,
                Email = existing.Email,
                Role = existing.Role,
                Token = token


            };
        }





    }


    
}
