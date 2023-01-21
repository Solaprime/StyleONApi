using Microsoft.EntityFrameworkCore;
using Shared;
using StyleONApi.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.AuthServices
{
    public class DispatchOrderService : IDispatchService
    {
        private readonly StyleONContext _context;
        public DispatchOrderService(StyleONContext context)
        {
            _context = context;
        }
        public async  Task<UserManagerResponse> PickOrder(Guid orderId)
        {

            if (orderId == null)
            {
                throw new ArgumentNullException();
            }
            //Check if Order Exsit
            var order =  await _context.Orders.FirstOrDefaultAsync( s=> s.OrderId == orderId);
            if (order == null)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Order with these Id can not be found"
                };
            }

            //Check state of order 
            if (order.OrderState == Entities.OrderStatus.OrderMade)
            {
                return new UserManagerResponse
                {
                    IsSuccess = true,
                    Message = "Order ready for Pick up"
                };
            }


            return new UserManagerResponse { IsSuccess = false, Message = "Some error occired" };
           
        }
    }
}
