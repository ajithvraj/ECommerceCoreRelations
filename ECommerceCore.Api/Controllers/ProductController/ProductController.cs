using ECommerceCore.Application.DTOs.ProductsDTO;
using ECommerceCore.Application.Services.ProductServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerceCore.Application.Common;

namespace ECommerceCore.Api.Controllers.ProductController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService) 
        {
            _productService = productService;
        
        }

        [Authorize (Roles = "Admin")]

        [HttpPost("add")] 

        public async Task<IActionResult> AddProduct([FromForm] CreateProductDto dto)
        {

            var product = await _productService.AddProductAsync(dto);

            return Ok(ApiResponse<ProductResponseDto>.SuccessResult(product, "Product added successfully"));

        }

        [HttpGet("all")] 

        public async Task<IActionResult> GetAllProducts()
        {
           var result =  await _productService.GetAllProductsAsync();

            return Ok(ApiResponse<IEnumerable< ProductResponseDto>>.SuccessResult(result ,"Products fetched successfully"));

        }

        [HttpGet("{id}")] 

        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductById(id);

            return Ok(ApiResponse<ProductResponseDto>.SuccessResult(product, "Product fetched successfully"));

        }

        [Authorize(Roles = "Admin")]

        [HttpPut("update/{id}")]

        public async Task<IActionResult> UpdateProduct(int id ,[FromForm] UpdateProductDto dto)
        {
            var updated = await _productService.UpdateProductAsync(id, dto);

            return Ok(ApiResponse<ProductResponseDto>.SuccessResult(updated, "Product updated successfully"));

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")] 
        public async Task<IActionResult> DeleteProduct(int id)
        {

            var deleted = await _productService.DeleteProductAsync(id);

            return Ok(ApiResponse<bool>.SuccessResult(deleted, "Product deleted successfully"));

        }

        [HttpGet("Search")] 

        public async Task<IActionResult> SearchProducts([FromQuery] string? name, [FromQuery] string? category , [FromQuery] decimal minPrice , [FromQuery] decimal maxPrice)
        {

            var result = await _productService.SearchProductAsync(name, category, minPrice, maxPrice);

            return Ok(ApiResponse<IEnumerable<ProductResponseDto>>.SuccessResult(result,"Search completed successfully"));

        }











    }
}
