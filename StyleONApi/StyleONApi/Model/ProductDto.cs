using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Model
{
    /// <summary>
    /// Outerfacing For oUR Product
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Name of Product
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Price of Product
        /// </summary>
          public Double Price { get; set; }
        /// <summary>
        /// Description of Product
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// SlashPrice of Product
        /// </summary>

        public Double SlashPrice { get; set; }
        /// <summary>
        ///  Reviews of Product
        /// </summary>

        public Double Reviews { get; set; }
        /// <summary>
        /// DateTime Product was Posted
        /// </summary> 
        public DateTimeOffset DatePosted { get; set; }
        /// <summary>
        /// Images of Product Posted
        /// </summary>

        public List<ImageObject> Images { get; set; }

        //public Guid SellerId { get; set; }
        ////public SelerDto Seller { get; set; }


        // public ProductWithSellerDto SellerInfo { get; set; }


    }
}
