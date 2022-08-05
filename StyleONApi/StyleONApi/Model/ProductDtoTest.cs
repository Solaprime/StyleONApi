using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Model
{
    public class ProductDtoTest
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public Double Price { get; set; }
        public string Description { get; set; }

        public Double SlashPrice { get; set; }

        public Double Reviews { get; set; }
        public DateTimeOffset DatePosted { get; set; }
        public List<ImageObject> Images { get; set; }
        public SellerDtoForProduct sellerInfo { get; set; }
    }
}
