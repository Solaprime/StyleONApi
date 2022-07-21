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

    }
}
