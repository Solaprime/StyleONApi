using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.ResourceParameters
{
    public class ProductResourceParameters
    {
      // Mian Category will be revamoed to better suit 
      // our enum of category in Producrs
      // Pur Products willhave category Like 
      // Product of the week, new Product, Most Sold,
      //Most viewed
        public string MainCategory { get; set; }
        public string SearchQuery { get; set; }
    }
}
