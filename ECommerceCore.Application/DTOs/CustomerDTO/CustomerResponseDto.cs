using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.DTOs.CustomerDTO
{
    public class CustomerResponseDto
    {

        public int Id { get; set; }
        public string Name { get; set; } 
        public string Email { get; set; }
        public string Role { get; set; } 

        public string Token { get; set; }

    }
}
