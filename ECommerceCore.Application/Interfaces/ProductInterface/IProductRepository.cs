using ECommerceCore.Domain.Enities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.Interfaces.ProductInterface
{
    public interface IProductRepository
    {
        Task<Product>AddProductAsync(Product product);
        Task<Product?>GetProductByIdAsync(int  id);

        Task<IEnumerable<Product>> GetAllProductAsync(); 
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);

        Task<IEnumerable<Product>> SearchProductAsync(string? name,string? category ,  decimal? minPrice, decimal? maxPrice);


    }
}
