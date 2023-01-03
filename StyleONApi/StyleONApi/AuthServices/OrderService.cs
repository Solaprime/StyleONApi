using Shared;
using StyleONApi.Context;
using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.AuthServices
{
    public class OrderService : IOrderService
    {
        private readonly StyleONContext _context;
        public OrderService(StyleONContext context)
        {
            _context = context;
        }

        public  async Task<UserManagerResponse> CreateOrder(Order order)
        {
            // Receive the Order 

            //Check Null 
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            // Create Order

            //Create total Price for Order
    
            foreach (var orderprice in order.OrderItems)
            {
                order.TotalPrice += orderprice.ProductPrice;
            }
            await _context.Orders.AddAsync(order);
             await _context.SaveChangesAsync();
            return new UserManagerResponse { IsSuccess = true, Message = "Order Created Sucessfully" };

            // 
        }
    }
}
