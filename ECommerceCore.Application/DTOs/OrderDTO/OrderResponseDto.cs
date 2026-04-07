using ECommerceCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.DTOs.OrderDTO
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty; 
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusMessage => Status.ToString(); 
        public DateTime CreatedAt { get; set; }
        public List<OrderItemResponseDto> OrderItems { get; set; } = new();
    }
}
