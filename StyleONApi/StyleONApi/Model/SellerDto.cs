using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Model
{
    public class SellerDto
    {
        public Guid SellerId { get; set; }
        public int NumberOfCompletedSales { get; set; }
        public DateTime DateRegistered { get; set; }
        public string StoreName { get; set; }
        public Double StoreReview { get; set; }

        public ICollection<Product> Products { get; set; }

        public string Email { get; set; }

    }
}
