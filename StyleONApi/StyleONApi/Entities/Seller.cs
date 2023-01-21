using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Entities
{
    public class Seller
    {

      
        public Seller()
        {
            Products = new List<Product>();
        }
        [Required]
        [Key]
        public Guid SellerId { get; set; }
        public int NumberOfCompletedSales { get; set; }
        public ICollection<Product> Products { get; set; }
        public DateTime DateRegistered { get; set; }
        [Required]
        [MaxLength(200)]
        public string StoreName { get; set; }
        [Required]

        //Are u going to use the same email u use to register as the same email seller are going
        // to use to work 
        public string Email { get; set; }

        public Double StoreReview { get; set; }
        // Why do u need UserFlow, you dont need a navigation property back to UserFlow

        //Just the userId will Sorface 
      //  public ApplicationUser UserFlow { get; set; }
     //  [ForeignKey(ApplicationUser)]
        public Guid ApplicationUserId { get; set; }





    }
}
