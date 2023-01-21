using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Model
{
    public class OrderDtoToReturn : OrderDto
    {

        public Guid OrderId { get; set; }
        public Double TotalPrice { get; set; }
        public OrderStatus OrderState { get; set; }

        public Product OrderProducts { get; set; }


        //Find a means to cast orderProduct to ProductDto
    }

}
