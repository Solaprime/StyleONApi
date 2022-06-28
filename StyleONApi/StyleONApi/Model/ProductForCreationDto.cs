using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Model
{
    public class ProductForCreationDto
    {
        
        [Required]
        public string Name { get; set; }
        public Double Price { get; set; }
        public string Description { get; set; }

        public Double SlashPrice { get; set; }

        public Double Reviews { get; set; }
        public DateTimeOffset DatePosted { get; set; }
    }
}
