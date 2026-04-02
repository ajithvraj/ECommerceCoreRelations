using ECommerceCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Domain.Enities
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string OrderNumber { get; set; } = Guid.NewGuid().ToString();

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
