using StyleONApi.Entities;
using StyleONApi.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Repository
{
    public interface IProductRepository
    {
        //Task<IEnumerable<Product>> GetAllProducts();
        //Task<Product> GetProduct(Guid productId);
        //Task CreateProduct(Product product);

        //Task CreateMultipleProduct(IEnumerable<Product> products);
        //Task<bool> ProductExist(Guid productId);


        Task<IEnumerable<Product>> GetAllProductsInDatabase();


        
        Task<IEnumerable<Product>> GetAllProducts(ProductResourceParameters productResourceParameters);


        // YOu will need to erase the above vode
        Task<IEnumerable<Product>> GetAllProducts(Guid SellerId);
        Task<Product> GetProduct(Guid SellerId,Guid productId);

        Task CreateProduct(Guid SellerId,Product product);

        Task<bool> SellerExist(Guid sellerId);

        Task<IEnumerable<Product>> GetSearchingProduct(Guid sellerId, ProductResourceParameters productResourceParameters);
       
        Task<bool> Save();
        Task<Product> GetProductWithSeller(Guid sellerId, Guid productId);

        Task<IEnumerable<Product>> GetProductCollection(Guid sellerId, IEnumerable<Guid> productId);

        Task DeleteProduct(Product product);
        


        //// Unchanged  product is void
        //Task DeleteProduct(Product product);
        ////Unchanged
        void UpdateProduct(Product product);


        // Collection of product vibes
        // Delete Product
        // UPdate Product  Check
        // the value returned when posting isnt returning SellerInfo. THe Dto mapping for productTOcReate, Entities and ProductTest


       

    }
}
