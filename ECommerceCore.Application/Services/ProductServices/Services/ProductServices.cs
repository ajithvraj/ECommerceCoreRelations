using ECommerceCore.Application.Common;
using ECommerceCore.Application.DTOs.ProductsDTO;
using ECommerceCore.Application.Interfaces.ProductInterface;
using ECommerceCore.Application.Services.ProductServices.Interfaces;
using ECommerceCore.Domain.Enities;
using System;
using System.Collections.Generic;
using ECommerceCore.Application.Exceptions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ECommerceCore.Application.Services.ProductServices.Services
{
    public class ProductServices : IProductService
    {

        private readonly IProductRepository _repo;
        private readonly CloudinaryService _cloudinary; 

        public ProductServices  ( IProductRepository repo , CloudinaryService cloudinary )
        {
            _repo = repo;
            _cloudinary = cloudinary;




        }

       public async Task<ProductResponseDto> AddProductAsync(CreateProductDto product)
        {
            var imageUrl = await _cloudinary.UploadImageAsync(product.Image);

            var item = new Product
            {
               Name = product.Name,
               Price= product.Price,
               Description = product.Description,
               Stock = product.Stock,
               Category = product.Category,
               ImageUrl = imageUrl,
               IsActive = true,


            };

            var created = await _repo.AddProductAsync(item); 
            return MapToResponse(created);




        }
       public async Task<ProductResponseDto> GetProductById(int id)
        {

            var product = await _repo.GetProductByIdAsync(id); 

            if(product == null)
            {
                throw new NotFoundException("Product not found");
            }

            return MapToResponse(product);

        }
       public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {

            var products = await _repo.GetAllProductAsync();
            return products.Select(MapToResponse);


        }
       public async Task<ProductResponseDto> UpdateProductAsync(int id, UpdateProductDto update)
        {

            var product = await _repo.GetProductByIdAsync(id);

            if (product == null) throw new NotFoundException("Product not found"); 

            product.Name = update.Name;
            product.Price = update.Price;
            product.Description = update.Description;
            product.Stock = update.Stock;
            product.Category = update.Category; 

            if(update.Image != null)
            {
                await _cloudinary.DeleteImageAsync(product.ImageUrl);
                product.ImageUrl = await _cloudinary.UploadImageAsync(update.Image);
            }

            var updated = await _repo.UpdateProductAsync(product); 

            return MapToResponse(updated);





        }
       public async Task<bool> DeleteProductAsync(int id)
        {
            var exist = await _repo.GetProductByIdAsync(id);
            if (exist == null) throw new NotFoundException("Product not found");

            //await _cloudinary.DeleteImageAsync(exist.ImageUrl);
          return  await _repo.DeleteProductAsync(id);



        }
       public async Task<IEnumerable<ProductResponseDto>> SearchProductAsync(string? name, string? category, decimal minPrice, decimal maxPrice)
        {

            var result = await _repo.SearchProductAsync(name, category, minPrice, maxPrice);
            return result.Select(MapToResponse);


        }

        private ProductResponseDto MapToResponse(Product product) => new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Stock = product.Stock,
            Category = product.Category,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,

        };
    }
}
