﻿using Microsoft.EntityFrameworkCore;
using StyleONApi.Context;
using StyleONApi.Entities;
using StyleONApi.Helpers;
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
            _context = styleONContext ?? throw new ArgumentNullException(nameof(StyleONContext));
        }
      

        //public  async Task CreateProduct(Product product)
        //{
        //    if (product ==null)
        //    {
        //        throw new ArgumentNullException(nameof(product));
        //    }
        //    product.ProductId = new Guid();
        //    await _context.Products.AddAsync(product);
        //    await  _context.SaveChangesAsync();
        //}

      

        //public async  Task<IEnumerable<Product>> GetAllProducts()
        //{
        //    return await _context.Products.ToListAsync();
        //}

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

      

        //public async Task<bool> ProductExist(Guid productId)
        //{
        //    if (productId == null)
        //    {
        //        throw new ArgumentNullException(nameof(productId));
        //    }
        //    var productToFind = await _context.Products.Where(c => c.ProductId == productId).FirstOrDefaultAsync();
        //    if (productToFind == null)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        public void UpdateProduct(Product product)
        {
            // No implementations
            _context.SaveChanges();

        }

        // Related to Seller
        public  async Task<IEnumerable<Product>> GetAllProducts(Guid sellerId)
        {
            if (sellerId == null)
            {
                throw new ArgumentNullException(nameof(sellerId));
            }

            return await _context.Products.Where(c => c.SellerId == sellerId)
                .OrderBy(c => c.Name).ToListAsync();
        }

        public async  Task<Product> GetProduct(Guid sellerId, Guid productId)
        {
            if (sellerId == null)
            {
                throw new ArgumentNullException(nameof(sellerId));
            }
            return await _context.Products.FirstOrDefaultAsync(a => a.ProductId == productId);
        }

        public  async Task CreateProduct(Guid sellerId, Product product)
        {
            if (sellerId == null)
            {
                throw new ArgumentNullException(nameof(sellerId));
            }

            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            product.SellerId = sellerId;
            // Try and check if u can pass A seller Info directly to product.Seller.
           await   _context.Products.AddAsync(product);
        }

        public async  Task<bool> SellerExist(Guid sellerId)
        {
            if (sellerId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(sellerId));
            }
              return  _context.Sellers.Any(a => a.SellerId == sellerId);
        }

        public async Task<IEnumerable<Product>> GetAllProductsInDatabase()
        {
            return await _context.Products.ToListAsync();
        }

        public async  Task<bool> Save()
        {
            return  ( await _context.SaveChangesAsync() >= 0);
        }

        public  async Task<IEnumerable<Product>> GetSearchingProduct(Guid sellerId, ProductResourceParameters productResourceParameters)
        {
           
            if (sellerId == null)
            {
                throw new ArgumentNullException(nameof(sellerId));
            }
         
                var sellersProduct = await GetAllProducts(sellerId);
                if (productResourceParameters == null)
                {
                    throw new ArgumentNullException(nameof(productResourceParameters));
                }
                if (string.IsNullOrWhiteSpace(productResourceParameters.MainCategory)
                    &&
                    string.IsNullOrWhiteSpace(productResourceParameters.SearchQuery))
                {
                    // return GetAllProductNonAsync();
                    return sellersProduct;
                }
                 sellersProduct = _context.Products as IQueryable<Product>;

                if (!string.IsNullOrWhiteSpace(productResourceParameters.SearchQuery))
                {
                    var searchQuery = productResourceParameters.SearchQuery.Trim();
                    sellersProduct = sellersProduct.Where(a => a.Name.Contains(searchQuery));
                }

                return sellersProduct;
        }


        // Changed to pAGEDlIst
        public async Task<PagedList<Product>> GetAllProducts(ProductResourceParameters productResourceParameters)
        {
            if (productResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(productResourceParameters));
            }
            //if (string.IsNullOrWhiteSpace(productResourceParameters.MainCategory)
            //    &&
            //    string.IsNullOrWhiteSpace(productResourceParameters.SearchQuery))
            //{
            //    // return GetAllProductNonAsync();
            //    return await GetAllProductsInDatabase();
            //}
            var collectionOfProduct = _context.Products as IQueryable<Product>;

            if (!string.IsNullOrWhiteSpace(productResourceParameters.SearchQuery))
            {
                var searchQuery = productResourceParameters.SearchQuery.Trim();
                collectionOfProduct = collectionOfProduct.Where(a => a.Name.Contains(searchQuery));
            }

            return PagedList<Product>.Create(collectionOfProduct, productResourceParameters.PageNumber,
                productResourceParameters.PageSize);

            // implement the both dat if a consumer inputted both searchquery and maincategorry
            // the data returned must contain the two condition mentioned
            // code for when main Category will be Implemented
            //if (!string.IsNullOrWhiteSpace(productResourceParameters.MainCategory))
            //{
            //    var mainCategory = productResourceParameters.MainCategory.Trim();
            //    collectionOfProduct = collectionOfProduct.Where(a=>mainCategory )
            //}


        }

        public  async Task<Product> GetProductWithSeller(Guid sellerId, Guid productId)
        {
            if (sellerId == null)
            {
                throw new ArgumentNullException(nameof(sellerId));
            }
            // return await _context.Products.FirstOrDefaultAsync(a => a.ProductId == productId);

            return await _context.Products.Include(s => s.Seller).FirstOrDefaultAsync(a => a.ProductId == productId);
        }

        public  async Task<IEnumerable<Product>> GetProductCollection(Guid sellerId, IEnumerable<Guid> productId)
        {
            if (sellerId == null)
            {
                throw new ArgumentNullException(nameof(sellerId));
            }
            if (productId == null)
            {
                throw new ArgumentNullException(nameof(productId));
            }
            return  await _context.Products.Where(a => productId.Contains(a.ProductId)).ToListAsync();
        }
    }
}
