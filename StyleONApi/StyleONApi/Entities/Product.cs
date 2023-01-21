using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Entities
{
    public class Product
    {
        [Required]
        [Key]
        public Guid ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Double Price { get; set; }
        [Required]
        public string Description { get; set; }
      

        public Double SlashPrice { get; set; }
      
        public Double Reviews { get; set; }
        public DateTimeOffset DatePosted { get; set; }
        public List<ImageObject> Images { get; set; }

        public Seller Seller { get; set; }
        public Guid SellerId { get; set; }


      
          
      
       

    }
}
