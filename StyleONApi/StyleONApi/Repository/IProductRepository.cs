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
        Task<IEnumerable<Product>> GetAllProducts();
        IEnumerable<Product> GetAllProductNonAsync();
        Task<Product> GetProduct(Guid productId);
        Task CreateProduct(Product product);
        
        Task CreateMultipleProduct(IEnumerable<Product> products);
        Task<bool> ProductExist(Guid productId);
        
        Task<IEnumerable<Product>> GetAllProducts(ProductResourceParameters productResourceParameters);


        // YOu will need to erase the above vode
        Task<IEnumerable<Product>> GetAllProducts(Guid SellerId);
        Task<Product> GetProduct(Guid SellerId,Guid productId);

        Task CreateProduct(Guid SellerId,Product product);

        Task<bool> SellerExist(Guid sellerId);

        Task<IEnumerable<Product>> GetSearchingProduct(Guid sellerId, ProductResourceParameters productResourceParameters);
        Task<IEnumerable<Product>> GetAllProductsInDatabase();
        Task<bool> Save();



        // Unchanged  product is void
        Task DeleteProduct(Product product);
        //Unchanged
        void UpdateProduct(Product product);









        //  Check how multipe products are created
        //Task<IEnumerable<Product>> CreateMultipleProduct(IEnumerable<Product> products);



        // check our kelvi docks implmented Update
        //void UpdateProduct(Product product);





    }
}
