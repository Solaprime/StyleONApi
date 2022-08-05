using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Model
{
    /// <summary>
    /// The OuterFacing needed to Create A product
    /// </summary>
    public class ProductForCreationDto
    {
        /// <summary>
        /// the Name of the Product to Post
        /// </summary>
        
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Price of Product
        /// </summary>
        public Double Price { get; set; }
        /// <summary>
        /// short description of product
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// SlashPrice of Product
        /// </summary>
        public Double SlashPrice { get; set; }
        /// <summary>
        /// Review of Product
        /// </summary>
        public Double Reviews { get; set; }
        /// <summary>
        /// Represnt the DateTime product ws posted
        /// </summary>
        public DateTimeOffset DatePosted { get; set; }
        /// <summary>
        /// a product will have Images
        /// </summary>
        public List<ImageObject> Images { get; set; }

    }
}
