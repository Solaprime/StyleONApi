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
    public class SellerRepository : ISellerRepository
    {
        private readonly StyleONContext _context;
        public SellerRepository(StyleONContext context)
        {
            _context = context;
        }

        public Task<bool> CheckIfSellershasnotUpdated(Seller seller)
        {
            throw new NotImplementedException();
        }

        public  async Task CreateMultipeSeller(IEnumerable<Seller> sellers)
        {
            if (sellers == null)
            {
                throw new ArgumentNullException(nameof(sellers));
            }
            foreach (var seller in sellers)
            {
                seller.SellerId= new Guid();
                await _context.AddAsync(seller);
                await _context.SaveChangesAsync();
            }
        }

        public  async Task CreateSeller(Seller seller)
        {
            if (seller == null)
            {
                throw new ArgumentNullException(nameof(seller));
            }
            seller.SellerId = new Guid();
            await _context.Sellers.AddAsync(seller);
            await _context.SaveChangesAsync();
        }

        public void DeleteSeller(Seller seller)
        {
            _context.Sellers.Remove(seller);
            _context.SaveChanges();
        }

        public async  Task<IEnumerable<Seller>> GetAllSellerAsync()
        {
            return await _context.Sellers.ToListAsync();
        }

        public Task<Seller> GetSeller(Guid sellerId)
        {
            throw new NotImplementedException();
        }

        //Refacto Note the Usernam and First Name is now coming from ApplicationUSER
        //public  async Task<IEnumerable<Seller>> GetSellers(SellerResourceParameters resourceParameters)
        //{
        //    if (resourceParameters == null)
        //    {
        //        throw new ArgumentNullException(nameof(resourceParameters));
        //    }
        //    if (string.IsNullOrWhiteSpace(resourceParameters.SearchQuery))
        //    {
        //        return await GetAllSellerAsync();
        //    }
        //    var collectionOfSeller = _context.Sellers as IQueryable<Seller>;
        //    if (!string.IsNullOrWhiteSpace(resourceParameters.SearchQuery))
        //    {
        //        var searchQuery = resourceParameters.SearchQuery.Trim();
        //        collectionOfSeller = collectionOfSeller.Where(a => a.FirstName.Contains(searchQuery)||
        //        a.LastName.Contains(searchQuery));
          
        //    }
        //    return await collectionOfSeller.ToListAsync();
        //}

        public Task<IEnumerable<Seller>> GetSellers(SelllersResourceParameters resourceParameters)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Seller>> GetSellers()
        {
            throw new NotImplementedException();
        }

        public  async Task<Seller> GetSingleSellerAsync(Guid SellerId)
        {
            if (SellerId == null)
            {
                throw new ArgumentNullException(nameof(SellerId));
            }
            return await _context.Sellers.Where(b => b.SellerId == SellerId).FirstOrDefaultAsync();
        }

        /// <summary>
        ///  TODo
        /// </summary>
        /// <returns></returns>
        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public  async Task<bool> SellerExist(Guid SellerId)
        {
            if (SellerId == null)
            {
                throw new ArgumentNullException(nameof(SellerId));
            }
            var sellerToFind = await _context.Sellers.Where(c => c.SellerId == SellerId).FirstOrDefaultAsync();
            if (sellerToFind == null)
            {
                return false;
            }
            return true;
        }

        public void UpdateSeller(Seller seller)
        {
            _context.SaveChanges();
        }

        Task ISellerRepository.DeleteSeller(Seller seller)
        {
            throw new NotImplementedException();
        }
    }
}
