using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
<<<<<<< HEAD
=======
using System.ComponentModel.DataAnnotations.Schema;
>>>>>>> SwaggerFlowController
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Entities
{
    public class Seller
    {
<<<<<<< HEAD
<<<<<<< HEAD
      
error: Pulling is not possible because you have unmerged files.


=======
        [Required]
        public Guid SellerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HouseAddress { get; set; }
        public string State { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
>>>>>>> 0e8e96d9364b6b5479621b2c91a2f3a1cbf646c6
=======
        public Seller()
        {
            Products = new List<Product>();
        }
        [Required]
        
        public Guid SellerId { get; set; }
        public int NumberOfCompletedSales { get; set; }
        public ICollection<Product> Products { get; set; }
        public DateTime DateRegistered { get; set; }
        [Required]
        [MaxLength(200)]
        public string StoreName { get; set; }
        [Required]
        public string Email { get; set; }
        public ApplicationUser UserFlow { get; set; }
        public Double StoreReview { get; set; }

>>>>>>> SwaggerFlowController
    }
}
