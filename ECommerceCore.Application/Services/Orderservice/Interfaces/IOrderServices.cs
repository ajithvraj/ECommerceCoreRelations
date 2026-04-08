using ECommerceCore.Application.DTOs.OrderDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.Services.Orderservice.Interfaces
{
   public interface  IOrderServices
    {
        Task<OrderResponseDto> PlaceOrderAsync(int customerId, PlaceOrderDto request);
        Task<OrderResponseDto> GetOrderByIdAsync(int customerId, int orderId);
        Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(int customerId);
        Task<bool> CancelOrderAsync(int customerId, int orderId);
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
        Task<OrderResponseDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto request);



    }
}
