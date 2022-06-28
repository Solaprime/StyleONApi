using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Entities
{
    public class ImageObject
    {
        [Key]
        public int ImageObjectId { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
       
    }
}
