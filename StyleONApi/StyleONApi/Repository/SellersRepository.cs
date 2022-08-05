using Microsoft.EntityFrameworkCore;
using StyleONApi.Context;
using StyleONApi.Entities;
using StyleONApi.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Repository
{
    public class SellersRepository : ISellerRepository
    {

        private readonly StyleONContext _context;
        public SellersRepository(StyleONContext styleONContext)
        {
            _context = styleONContext;
        }

        public async Task<IEnumerable<Seller>> GetSellers()
        {
            return await _context.Sellers.ToListAsync();
        }

        public async Task<IEnumerable<Seller>> GetSellers(SelllersResourceParameters resourceParameters)
        {
            if (resourceParameters == null)
            {
                throw new ArgumentNullException(nameof(resourceParameters));
            }
            if (string.IsNullOrWhiteSpace(resourceParameters.SearchQuery))
            {
                return await GetSellers();
            }
            var collectionOfSellers = _context.Sellers as IQueryable<Seller>;
            if (!String.IsNullOrWhiteSpace(resourceParameters.SearchQuery))
            {
                var searchQuery = resourceParameters.SearchQuery.Trim();
                collectionOfSellers = collectionOfSellers.Where(a => a.StoreName.Contains(searchQuery));
            }
            return await collectionOfSellers.ToListAsync();
        }



        // am yet to Put this In the seller repo iNTERFACE

        public async Task<bool> CheckIfSellershasnotUpdated(Seller seller)
        {
            // Retrive the List of sellers
            //Itereate the List of Sellers and check 
            // return base on 

            var aLLSellers = await GetSellers();
            foreach (var sellerTo in aLLSellers)
            {
                if (seller.Email == sellerTo.Email)
                {
                    return false;
                }
            }
            return true;

        }

        public async Task<Seller> GetSeller(Guid sellerId)
        {

            if (sellerId == null)
            {
                throw new ArgumentNullException(nameof(sellerId));
            }

            // Find seller
           return await _context.Sellers.Where(c => c.SellerId == sellerId).FirstOrDefaultAsync();


        }
        public async Task DeleteSeller(Seller seller)
        {
            _context.Sellers.Remove(seller);
            await _context.SaveChangesAsync();
        }
    }
}
