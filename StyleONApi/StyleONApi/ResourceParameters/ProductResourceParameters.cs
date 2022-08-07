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


        // properties set below work with Pagination
        const int maxPageSize = 20;


        public int PageNumber { get; set; } = 1;


        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }


    }
}
