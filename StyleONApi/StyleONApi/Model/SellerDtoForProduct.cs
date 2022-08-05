using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Model
{
    public class SellerDtoForProduct
    {
        public string StoreName { get; set; }
        public int NumberOfCompletedSales { get; set; }
        public Double StoreReview { get; set; }
        public DateTime DateRegistered { get; set; }
    }
}
