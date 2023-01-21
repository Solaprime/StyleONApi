using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Entities
{

     public class Cart
    {
        public List<OrderItem> OrderItems { get; set; }
        public Double  ProductTotalPrice { get; set; }
        [ForeignKey("Id")]
        public ApplicationUser User { get; set; }

    }
    public class Order
    {
        [Key]
        [Required]
        public Guid OrderId { get; set; }
      
        
      //  public ApplicationUser User { get; set; } 
      //  public Guid DispatchRiderId { get; set; }
      //  public List<OrderItem> OrderItems { get; set; }
        public ModeOfPayment PaymentMode { get; set; }

        public ModeOfDelivery DeliveryMode { get; set; }

        public Double  OrderPrice { get; set; }
        [Required]
        public Double TotalPrice{ get; set; }

        [Required]
        public List<OrderItem> OrderItems { get; set; }
        //U need to asign a application userId
      
      //  public Guid ApplicationUserId { get; set; }

        public OrderStatus OrderState { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser userFlow { get; set; }




    }

    //
      public class OrderItem
      {

        public int OrderItemId { get; set; }
        public Guid SellerId { get; set; }
        public Guid ProductId { get; set; }
      //  public double ProductPrice { get; set; }
        public int Quantity { get; set; }
        public Product OrderProducts { get; set; }

    }


    //I dont need the ProductPrice to be sent to the Db 
    //Whe you query the Producct rhe Prie shoul be Returned 
    //So order item should only contain,

    //selleriD, ProductID and Quantity

    //Meand to reafcto Price individaul Price by Quantity, Price of Product multiplied by quantity

    

    //public Specification poductSpecification { get; set; }
    //public ModeOfDelivery DeliveryMode { get; set; }



    public class Specification
    {
        public string Color { get; set; }
        public Size ClothSize { get; set; }
    }

    public enum Size
    {
        Small,
        Medium,
        Large,
        ExtraLarge

    }
    public enum ModeOfPayment
    {
        PaymentOnDelivery,
        PaymentOnOrder,
        PaymentOnPickup,
        PreOrder,
        
    }

    public enum ModeOfDelivery
    {
         AddressDelivery,
         PickupStation

    }

    public enum OrderStatus
    {
        OrderMade,
        OrderAssignedToDispatch,
        OrderEnroute,
        OrderDelivered,
        OrderReturned

    }

}


