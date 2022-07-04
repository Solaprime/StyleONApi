using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Entities
{
    public class Seller
    {
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
    }
}
