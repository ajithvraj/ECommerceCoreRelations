using ECommerceCore.Application.DTOs.ProductsDTO;

namespace ECommerceCore.Application.Services.ProductServices.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> AddProductAsync(CreateProductDto product);
        Task<ProductResponseDto> GetProductById(int id);
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto> UpdateProductAsync(int id, UpdateProductDto update);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<ProductResponseDto>> SearchProductAsync(string? name, string? category, decimal minPrice, decimal maxPrice);
        Task<IEnumerable<ProductResponseDto>> GetInactiveProductAsync();
        Task<ProductResponseDto> RestoreProductAsync(int id);
    }
}