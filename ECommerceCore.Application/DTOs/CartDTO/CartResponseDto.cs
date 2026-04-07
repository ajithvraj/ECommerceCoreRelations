using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.DTOs.CartDTO
{
    public class CartResponseDto
    {
        public List<CartItemResponseDto> Items { get; set; } = new List<CartItemResponseDto>();
        public decimal GrandTotal { get; set; }
        public int TotalItems { get; set; }
        public int TotalUniqueProducts { get; set; }
    }
}
