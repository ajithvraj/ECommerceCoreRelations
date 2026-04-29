using ECommerceCore.Application.Common;
using ECommerceCore.Application.DTOs.ProductsDTO;
using ECommerceCore.Application.Exceptions;
using ECommerceCore.Application.Interfaces.ProductInterface;
using ECommerceCore.Application.Services.ProductServices.Interfaces;
using ECommerceCore.Domain.Enities;

namespace ECommerceCore.Application.Services.ProductServices.Services
{
    public class ProductServices : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly CloudinaryService _cloudinary;

        public ProductServices(IProductRepository repo, CloudinaryService cloudinary)
        {
            _repo = repo;
            _cloudinary = cloudinary;
        }

        public async Task<ProductResponseDto> AddProductAsync(CreateProductDto product)
        {
            if (product.Images == null || product.Images.Count == 0)
                throw new BadRequestException("At least one image is required");

            //  upload all images
            var imageUrls = await _cloudinary.UploadMultipleImagesAsync(product.Images);

            var item = new Product
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Stock = product.Stock,
                Category = product.Category,
                IsActive = true,
                // create ProductImage objects
                Images = imageUrls.Select((url, index) => new ProductImage
                {
                    ImageUrl = url,
                    IsPrimary = index == product.PrimaryImageIndex
                }).ToList()
            };

            var created = await _repo.AddProductAsync(item);
            return MapToResponse(created);
        }

        public async Task<ProductResponseDto> GetProductById(int id)
        {
            var product = await _repo.GetProductByIdAsync(id);
            if (product == null)
                throw new NotFoundException("Product not found");
            return MapToResponse(product);
        }
        //In your ProductService / Handler


        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _repo.GetAllProductAsync();
            return products.Select(MapToResponse);
        }

        public async Task<ProductResponseDto> UpdateProductAsync(int id, UpdateProductDto update)
        {
            var product = await _repo.GetProductByIdAsync(id);
            if (product == null)
                throw new NotFoundException("Product not found");

            product.Name = update.Name;
            product.Price = update.Price;
            product.Description = update.Description;
            product.Stock = update.Stock;
            product.Category = update.Category;

            // if new images provided delete old and upload new
            if (update.Images != null && update.Images.Count > 0)
            {
                var oldUrls = product.Images.Select(i => i.ImageUrl).ToList();
                await _cloudinary.DeleteMultipleImagesAsync(oldUrls);

                product.Images.Clear();

                var newUrls = await _cloudinary.UploadMultipleImagesAsync(update.Images);
                foreach (var (url, index) in newUrls.Select((url, i) => (url, i)))
                {
                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = url,
                        IsPrimary = index == update.PrimaryImageIndex
                    });
                }
            }

            var updated = await _repo.UpdateProductAsync(product);
            return MapToResponse(updated);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var exist = await _repo.GetProductByIdAsync(id);
            if (exist == null)
                throw new NotFoundException("Product not found");

            return await _repo.DeleteProductAsync(id);
        }

        public async Task<IEnumerable<ProductResponseDto>> SearchProductAsync(string? name, string? category, decimal minPrice, decimal maxPrice)
        {
            var result = await _repo.SearchProductAsync(name, category, minPrice, maxPrice);
            return result.Select(MapToResponse);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetInactiveProductAsync()
        {
            var product = await _repo.GetInactiveProductAsync();
            return product.Select(MapToResponse);
        }

        public async Task<ProductResponseDto> RestoreProductAsync(int id)
        {
            var product = await _repo.RestoreProductAsync(id);
            return MapToResponse(product);
        }

        //  updated MapToResponse
        private ProductResponseDto MapToResponse(Product product) => new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Stock = product.Stock,
            Category = product.Category,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            PrimaryImageUrl = product.Images
                .FirstOrDefault(i => i.IsPrimary)?.ImageUrl ??
                product.Images.FirstOrDefault()?.ImageUrl ?? string.Empty,
            ImageUrls = product.Images.Select(i => i.ImageUrl).ToList()
        };
    }
}