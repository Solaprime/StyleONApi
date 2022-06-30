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
        Task DeleteProduct(Product product);
        Task CreateMultipleProduct(IEnumerable<Product> products);
        Task<bool> ProductExist(Guid productId);
         void UpdateProduct(Product product);
        Task<IEnumerable<Product>> GetAllProducts(ProductResourceParameters productResourceParameters);

    





        

        // Since all product must have a selller 
        //Task<Product> GetProductById(Guid SellerId, Guid ProductId);
        //Task<Product> CreateProduct(Guid SellerId, Product product);

      


    }
}
