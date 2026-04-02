using ECommerceCore.Application.DTOs.ProductsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.Services.ProductServices.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> AddProductAsync(CreateProductDto product);
        Task<ProductResponseDto> GetProductById(int id); 
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto> UpdateProductAsync(int id, UpdateProductDto update);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<ProductResponseDto>> SearchProductAsync(string? name , string? category,decimal minPrice, decimal maxPrice);
    }
}
