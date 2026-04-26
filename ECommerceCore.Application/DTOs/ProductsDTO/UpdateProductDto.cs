using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.DTOs.ProductsDTO
{
    public class UpdateProductDto
    {

        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<IFormFile>? Images { get; set; } // optional on update
        public int PrimaryImageIndex { get; set; } = 0;
    }
}
