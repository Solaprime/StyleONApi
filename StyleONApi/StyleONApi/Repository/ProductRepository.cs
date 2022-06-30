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

        public async Task<bool> ProductExist(Guid productId)
        {
            if (productId == null)
            {
                throw new ArgumentNullException(nameof(productId));
            }
            var productToFind = await _context.Products.Where(c => c.ProductId == productId).FirstOrDefaultAsync();
            if (productToFind == null)
            {
                return false;
            }
            return true;
        }

        public void UpdateProduct(Product product)
        {
            // No implementations
            _context.SaveChanges();
            
        }

        public async  Task<IEnumerable<Product>> GetAllProducts(ProductResourceParameters productResourceParameters)
        {
            if (productResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(productResourceParameters));
            }
            if (string.IsNullOrWhiteSpace(productResourceParameters.MainCategory)
                && 
                string.IsNullOrWhiteSpace(productResourceParameters.SearchQuery))
            {
                return GetAllProductNonAsync();
            }
            var collectionOfProduct = _context.Products as IQueryable<Product>;

            if (!string.IsNullOrWhiteSpace(productResourceParameters.SearchQuery))
            {
                var searchQuery = productResourceParameters.SearchQuery.Trim();
                collectionOfProduct = collectionOfProduct.Where(a => a.Name.Contains(searchQuery));
            }

            return await collectionOfProduct.ToListAsync();

            // implement the both dat if a consumer inputted both searchquery and maincategorry
            // the data returned must contain the two condition mentioned
            // code for when main Category will be Implemented
            //if (!string.IsNullOrWhiteSpace(productResourceParameters.MainCategory))
            //{
            //    var mainCategory = productResourceParameters.MainCategory.Trim();
            //    collectionOfProduct = collectionOfProduct.Where(a=>mainCategory )
            //}

            
        }

        public IEnumerable<Product> GetAllProductNonAsync()
        {
            // the reason we created this method is because of 
            // Our GetAllProduct return a Task<IEnumerable<Product>>
            // and our get asynv method with the resource aparamter return a
            //Task<IEnumerable<Product>> so we have clashing return type
            return  _context.Products.ToList();
        }

        public  async Task<IEnumerable<Seller>> GetAllSellers()
        {
            return await _context.Sellers.ToListAsync();
        }
    }
}
