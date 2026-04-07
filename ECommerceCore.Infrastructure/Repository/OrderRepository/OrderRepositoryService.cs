using ECommerceCore.Application.Interfaces.OrderIterface;
using ECommerceCore.Domain.Enities;
using ECommerceCore.Infrastructure.Persistance.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Infrastructure.Repository.OrderRepository
{
    public class OrderRepositoryService : IOrderRepository
    {

        private readonly AppDbContext _db; 

        public OrderRepositoryService (AppDbContext db)
        {
            _db = db;
        }

       public async Task<Order> CreateOrderAsync(Order order)
        {
            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();
            return order;

        }
       public async Task<Order?> GetOrderByIdAsync(int orderId, int customerId) 
        {
            return await _db.Orders.
                Include(x => x.OrderItems).
                ThenInclude(x => x.Product).
                FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId);

        }
      public async  Task<Order?> GetOrderByIdForAdminAsync(int orderId)
        {

            return await _db.Orders.Include(o => o.OrderItems).ThenInclude(o => o.Product).FirstOrDefaultAsync(o => o.Id == orderId);

        }
      public async  Task<IEnumerable<Order>> GetMyOrderAsync(int customerId)
        {
            return await _db.Orders.Include(o => o.OrderItems).ThenInclude(o => o.Product).Where(o => o.CustomerId == customerId).OrderByDescending(o => o.CreatedAt).ToListAsync();

        }
       public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _db.Orders.Include(o => o.Customer).Include(o => o.OrderItems).ThenInclude( i => i.Product).OrderByDescending(o => o.CreatedAt).ToListAsync();

        }
      public async  Task<Order> UpdateOrderAsync(Order order)
        {

             _db.Orders.Update(order);
            await _db.SaveChangesAsync();
            return order;

        }
    }
}
