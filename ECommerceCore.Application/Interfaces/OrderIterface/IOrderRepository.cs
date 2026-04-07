using ECommerceCore.Domain.Enities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.Interfaces.OrderIterface
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(int orderId, int customerId);
        Task<Order?> GetOrderByIdForAdminAsync(int orderId);
        Task<IEnumerable<Order>> GetMyOrderAsync(int customerId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> UpdateOrderAsync(Order order);



    }
}
