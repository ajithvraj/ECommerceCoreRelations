using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Domain.Enities
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; } 

        public int ProductId { get; set; }
        public Product? Product { get; set; } 

        public int Quantity { get; set; }


    }
}
