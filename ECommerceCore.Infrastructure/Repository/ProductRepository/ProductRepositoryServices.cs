using ECommerceCore.Application.Interfaces.ProductInterface;
using ECommerceCore.Domain.Enities;
using ECommerceCore.Infrastructure.Persistance.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Infrastructure.Repository.ProductRepository
{
    public class ProductRepositoryServices : IProductRepository
    {

        private readonly AppDbContext _db;

        public ProductRepositoryServices(AppDbContext db) 
        { 
            _db = db;
        }

       public async Task<Product> AddProductAsync(Product product)
        {
             await _db.Products.AddAsync(product);

            await _db.SaveChangesAsync();

            return product;





        }
      public async  Task<Product?> GetProductByIdAsync(int id)
        {

            return await _db.Products.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true );

        }

       public async Task<IEnumerable<Product>> GetAllProductAsync()
        {
         return   await _db.Products.Where(x => x.IsActive).ToListAsync();


        }
       public async Task<Product> UpdateProductAsync(Product product)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync(); 
            return product;

        }
       public async Task<bool> DeleteProductAsync(int id)
        {

            var product = await _db.Products.FirstOrDefaultAsync(x => x.Id ==id);
            if (product == null)
            {
                return false;
                
            }

            product.IsActive = false; 
            await _db.SaveChangesAsync();
            return true;

        }

       public async Task<IEnumerable<Product>> SearchProductAsync(string? name, string? category, decimal? minPrice, decimal? maxPrice)
        {
            var exist = _db.Products.Where(p => p.IsActive).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                exist = exist.Where(x => x.Name.Contains(name));
                
            }

            if(!string.IsNullOrWhiteSpace(category)) exist = exist.Where(c => c.Category.Contains(category)); 

            if(minPrice.HasValue) exist = exist.Where(p => p.Price >=  minPrice.Value);
            if(maxPrice.HasValue) exist = exist.Where(p => p.Price <= maxPrice.Value); 

            return await exist.ToListAsync();



        }


    }
}
