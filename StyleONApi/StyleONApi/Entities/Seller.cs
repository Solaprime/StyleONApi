using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Entities
{
    public class Seller
    {
        [Required]
        public Guid SellerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HouseAddress { get; set; }
        public string State { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
