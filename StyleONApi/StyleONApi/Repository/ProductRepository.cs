using Microsoft.EntityFrameworkCore;
using StyleONApi.Context;
using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly StyleONContext _context;
        public ProductRepository( StyleONContext styleONContext)
        {
            _context = styleONContext;
        }
      

        public  async Task CreateProduct(Product product)
        {
            if (product ==null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            product.ProductId = new Guid();
            await _context.Products.AddAsync(product);
            await  _context.SaveChangesAsync();
        }

      

        public async  Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public  async Task<Product> GetProduct(Guid productId)
        {
            if (productId == null)
            {
                throw new ArgumentNullException(nameof(productId));
            }
            return await _context.Products.Where(c => c.ProductId == productId).FirstOrDefaultAsync();
            // after the productId just add an "And" to check the 
            // seller Id 
        }
        public async  Task DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public  async Task CreateMultipleProduct(IEnumerable<Product> products)
        {
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }
            foreach (var item in products)
            {
                item.ProductId = new Guid();
                await _context.Products.AddAsync(item);
                
            }
            await _context.SaveChangesAsync();
        }
    }
}
