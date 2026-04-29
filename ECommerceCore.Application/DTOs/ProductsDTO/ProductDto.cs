using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.DTOs.ProductsDTO
{
    // Application/DTOs/ProductDto.cs
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
        public string? ThumbnailUrl => Images.FirstOrDefault(i => i.IsDefault)?.ImageUrl
                                    ?? Images.FirstOrDefault()?.ImageUrl;
    }
}
