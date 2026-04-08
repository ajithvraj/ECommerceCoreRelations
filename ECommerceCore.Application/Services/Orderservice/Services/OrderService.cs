using ECommerceCore.Application.DTOs.OrderDTO;
using ECommerceCore.Application.Interfaces.CartInterface;
using ECommerceCore.Application.Interfaces.OrderIterface;
using ECommerceCore.Application.Interfaces.ProductInterface;
using ECommerceCore.Application.Services.Orderservice.Interfaces;
using ECommerceCore.Domain.Enities;
using ECommerceCore.Application.Exceptions;
using System;
using ECommerceCore.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.Services.Orderservice.Services
{
    public class OrderService : IOrderServices
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo; 

        public OrderService (IOrderRepository orderRepo , ICartRepository cartRepo, IProductRepository productRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            
        }



      public async  Task<OrderResponseDto> PlaceOrderAsync(int customerId, PlaceOrderDto request)
        {

            List<CartItem>itemsToOrder;  

            if(request.CartItemId.HasValue)
            {

                ///checkout single item
                var singleItem = await _cartRepo.GetCartItemByIdAsync(
                    request.CartItemId.Value, customerId);

                if (singleItem == null) throw new NotFoundException("Cart item not found");

                itemsToOrder = new List<CartItem> { singleItem };




            }
            else
            {

                //check out full cart items  

                var cartItems = await _cartRepo.GetCartItemsAsync(customerId);
                itemsToOrder = cartItems.ToList();

            }

            if (!itemsToOrder.Any()) throw new BadRequestException("No items to order");

            //stock validation  

            foreach(var item in itemsToOrder)
            {
                var product = await _productRepo.GetProductByIdAsync(item.ProductId);
                if (product == null) throw new NotFoundException($"Product {item.Product} not found");
                if (product.Stock < item.Quantity) throw new BadRequestException($"Insufficient stock for{product.Name}, Available: {product.Stock} ");

            } 

            //create order and deduct stock 

            var orderItems = new List<OrderItem>(); 

            foreach (var item in itemsToOrder)
            {
                var product = await _productRepo.GetProductByIdAsync(item.ProductId); 
                
                product!.Stock -= item.Quantity;
                await _productRepo.UpdateProductAsync(product);

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price

                });

            }

            //create order 


            var order = new Order
            {
                CustomerId = customerId,
                Status = OrderStatus.Pending,
                TotalAmount = orderItems.Sum(i => i.Price * i.Quantity),
                OrderItems = orderItems,
                CreatedAt = DateTime.Now,


            };

            var created = await _orderRepo.CreateOrderAsync(order);

            //remove ordered item from the cart 

            foreach (var item in orderItems)
            {
                await _cartRepo.RemoveCartItemAsync(item.Id, customerId);

            }

            return MapToResponse(created);


        }
        public async Task<OrderResponseDto>GetOrderByIdAsync(int customerId, int orderId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId, customerId);
            if (order == null)
                throw new NotFoundException("Order not found");
            return MapToResponse(order);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(int customerId)
        {
            var orders = await _orderRepo.GetMyOrderAsync(customerId);
            return orders.Select(MapToResponse);
        }

        public async Task<bool> CancelOrderAsync(int customerId, int orderId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId, customerId);
            if (order == null)
                throw new NotFoundException("Order not found");

            if (order.Status != OrderStatus.Pending)
                throw new BadRequestException(
                    "Only pending orders can be cancelled");

            // restore stock
            foreach (var item in order.OrderItems)
            {
                var product = await _productRepo.GetProductByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                    await _productRepo.UpdateProductAsync(product);
                }
            }

            order.Status = OrderStatus.Cancelled;
            await _orderRepo.UpdateOrderAsync(order);
            return true;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepo.GetAllOrdersAsync();
            return orders.Select(MapToResponse);
        }

        public async Task<OrderResponseDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto request)
        {
            var order = await _orderRepo.GetOrderByIdForAdminAsync(orderId);
            if (order == null)
                throw new NotFoundException("Order not found");

            // prevent invalid status transitions
            if (order.Status == OrderStatus.Cancelled)
                throw new BadRequestException("Cancelled orders cannot be updated");

            if (order.Status == OrderStatus.Completed)
                throw new BadRequestException("Completed orders cannot be updated");

            order.Status = request.Status;
            var updated = await _orderRepo.UpdateOrderAsync(order);
            return MapToResponse(updated);
        }

        private OrderResponseDto MapToResponse(Order order) => new OrderResponseDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            OrderItems = order.OrderItems.Select(i => new OrderItemResponseDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name?? string.Empty,
                ProductImage = i.Product?.ImageUrl?? string.Empty,
                Quantity = i.Quantity,
                Price = i.Price,
                TotalPrice = i.Price * i.Quantity

            }).ToList(),


        };



    }
}
