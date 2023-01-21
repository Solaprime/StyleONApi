using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Model
{
    public class SellerForCreationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<ProductForCreationDto> Products { get; set; }
        = new List<ProductForCreationDto>();

    }
}
