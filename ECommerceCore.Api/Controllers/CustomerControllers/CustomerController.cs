using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerceCore.Application.Services.CustomerServices.Interfaces;
using Microsoft.OpenApi.Writers;
using ECommerceCore.Application.DTOs.CustomerDTO;
using Microsoft.AspNetCore.Http.HttpResults;
using ECommerceCore.Application.Common;

namespace ECommerceCore.Api.Controllers.CustomerControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private readonly ICustomerServices _custumer; 

        public CustomerController (ICustomerServices customer)
        {
            _custumer = customer;

        }

        [HttpPost("Register")] 

        public async Task<IActionResult>CreateAccount(CreateCustomerDto dto)
        {
            var acccount = await _custumer.AddCustomerAsync(dto);

            return Ok(ApiResponse<CustomerResponseDto>.SuccessResult(acccount));



        }


        [HttpPost("Login")] 
        public async Task<IActionResult>Login(LoginCustomerDto log)
        {

            var login = await _custumer.CustomerLoginAsync(log);

            return Ok(ApiResponse<CustomerResponseDto>.SuccessResult(login));

        }
           


    }
}
