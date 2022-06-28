using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Model
{
    public class ProductForUpdate
    {
        [Required]
        public string Name { get; set; }
        public Double Price { get; set; }
        public string Description { get; set; }

        public Double SlashPrice { get; set; }
        public List<ImageObject> Images { get; set; }

    }
}
