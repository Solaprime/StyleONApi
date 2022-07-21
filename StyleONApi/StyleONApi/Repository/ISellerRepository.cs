using StyleONApi.Entities;
using StyleONApi.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Repository
{
   public  interface ISellerRepository
    {
        Task<IEnumerable<Seller>> GetSellers();
        Task<IEnumerable<Seller>> GetSellers(SelllersResourceParameters resourceParameters);
          Task<bool>  CheckIfSellershasnotUpdated(Seller seller);
    }
}
