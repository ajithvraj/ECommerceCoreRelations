using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.DTOs.OrderDTO
{
    public class OrderItemResponseDto
    {

        public int Id { get; set; }
        public int ProductId { get; set; } 
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage {  get; set; } = string.Empty; 
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
