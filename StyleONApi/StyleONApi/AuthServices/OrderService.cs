using Microsoft.EntityFrameworkCore;
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

        public  async Task<Product> CheckProduct(Guid sellerId, Guid productId)
        {
            if (sellerId== null)
            {
                throw new ArgumentNullException(nameof(sellerId));
            }
            var confirmedProduct =  await _context.Products.
                Where(s => s.ProductId == productId && s.SellerId == sellerId).FirstOrDefaultAsync();
            if (confirmedProduct != null)
            {
                return confirmedProduct;
            }

            return null;
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

            //foreach (var orderprice in order.OrderItems)
            //{

            //    order.TotalPrice += orderprice.ProductPrice *  orderprice.Quantity;
            //}

            // I need the Price, Quantity

            foreach (var singleorder in order.OrderItems)
            {
                ///Retrieve the Price of that order
                ///Multiply by quantity
                ///add for every Iteration
                var priceOfEachItem =  await _context.Products
                  .Where(s => s.ProductId == singleorder
               .ProductId && s.SellerId == singleorder.SellerId).Select(s => new Pricedemo(s.Price)).FirstOrDefaultAsync();

                var singleorderPrice = priceOfEachItem.Price * singleorder.Quantity;
                order.TotalPrice += singleorderPrice; 
            }
            await _context.Orders.AddAsync(order); 
             await _context.SaveChangesAsync();
            return new UserManagerResponse { IsSuccess = true, Message = "Order Created Sucessfully",
                Id = order.OrderId.ToString()
             };

            //  //Meand to reafcto Price individaul Price by Quantity, Price of Product multiplied by quantity

            //Check if sellerId amd ProductId are Marching 
            // a custom method To retrive a product a product and check it Price 

            //Cherck if other prop like ProductPrice are matchin




            // For performance sake, anytime i retireve a product i only
           // need the Price of the Product, the Quantiy comes from the OrderITem
        }


        //Make sure the orderItem has iNfo of the Product return
        public async  Task<Order> GetOrder(Guid orderId)
        {
            if (orderId == null)
            {
                throw new ArgumentNullException(nameof(orderId));
            }

            var order = await  _context.Orders.Include(s=> s.OrderItems)
                .FirstOrDefaultAsync(s => s.OrderId == orderId);
            return order;
        }

        //I want the Order to retireve the Details of the Product of the Product 
        public async Task<Order> GetOrderForUser(Guid orderId)
        {
            if (orderId == null)
            {
                throw new ArgumentNullException(nameof(orderId));
            }

            var order = await _context.Orders.Include(s => s.OrderItems).ThenInclude(s=> s.OrderProducts)
                .FirstOrDefaultAsync(s => s.OrderId == orderId);
            return order;
        }
    }




     public struct Pricedemo
    {
        public Pricedemo( Double price)
        {
            Price = price;
        }
        public Double Price;

    }

  

}

// Work with the Dispatch Order Flow, Only a registrrd dispatch can Pick up ann order
//An admin will get all Pending order, assigne all pending order to a Dispatch
//the state of orderstatus changes to enrout,
//if dispatch delivers it the state of the Order becomes deliveres
// and the Use drop a Reivew of the Product 
//Write a Get method to get all method in the dB AND include sellerId, ProductID, TO MAKE Displaying Easy 
//If seller Order is complete update the Seller nUMBER OF cOMPLETED sALES 