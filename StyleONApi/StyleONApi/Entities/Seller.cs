using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Entities
{
    public class Seller
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public String FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
