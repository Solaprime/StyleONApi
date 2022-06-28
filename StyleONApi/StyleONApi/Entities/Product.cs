using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Entities
{
    public class Product
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        public Double Price { get; set; }
        public string Description { get; set; }
      
        public Double SlashPrice { get; set; }
      
        public Double Reviews { get; set; }
        public DateTimeOffset DatePosted { get; set; }


        public List<ImageObject> Images { get; set; }

        // Check how to work with enum, and check how will u work with the User, seller, Buyer
        //public Guid SellerId { get; set; }
        //public enum Category { get; set; }
        // review working with List in EFCORE
        //public List<String> ImageUrl { get; set; } = new List<string>();



        // when u start making changes to my entity classs 
        // remeber to vhange the DTO AS well
    }
}
