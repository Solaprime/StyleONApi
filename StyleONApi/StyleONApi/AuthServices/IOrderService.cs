using Shared;
using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.AuthServices
{
    public interface IOrderService
    {
        //Create Order
        //Review Order
        //Pick Up Order
        //Return Order
        Task<UserManagerResponse> CreateOrder(Order order);
        Task<Product> CheckProduct(Guid sellerId, Guid productId);
        //
        //Task<bool> CheckProduct(Guid sellerId, Guid productId);
        Task<Order> GetOrder(Guid orderId);
    }
}
