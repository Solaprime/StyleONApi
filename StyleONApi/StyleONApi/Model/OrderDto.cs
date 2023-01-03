using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Model
{
    public class OrderDto
    {
        public ModeOfPayment PaymentMode { get; set; }

        public ModeOfDelivery DeliveryMode { get; set; }

        public List<OrderItem> OrderItems { get; set; }

      //  public OrderStatus OrderState { get; set; }



    }
}
