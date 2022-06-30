using StyleONApi.Entities;
using StyleONApi.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Repository
{
   public interface ISellerRepository
    {
        //CRUD
        //IO operation
        Task<IEnumerable<Seller>> GetAllSellerAsync();
        Task<Seller> GetSingleSellerAsync(Guid SellerId);
        bool SaveChanges();
        void UpdateSeller(Seller seller);
        void DeleteSeller(Seller seller);
        Task CreateMultipeSeller(IEnumerable<Seller> sellers);
        Task CreateSeller(Seller seller);
        Task<bool> SellerExist(Guid SellerId);
        Task<IEnumerable<Seller>> GetSellers(SellerResourceParameters resourceParameters);


    }
}
