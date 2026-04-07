using ECommerceCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.DTOs.OrderDTO
{
    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}
