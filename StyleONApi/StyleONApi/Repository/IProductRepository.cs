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
        


        //  Check how multipe products are created
        //Task<IEnumerable<Product>> CreateMultipleProduct(IEnumerable<Product> products);



        // check our kelvi docks implmented Update
        //void UpdateProduct(Product product);


        // Since all product must have a selller 
        //Task<Product> GetProductById(Guid SellerId, Guid ProductId);
        //Task<Product> CreateProduct(Guid SellerId, Product product);

        // FOR sEARCHing and filtering 
        //IEnumerable<Author> GetAuthors(string mainCategory, string searchQuery);
        //IEnumerable<Author> GetAuthors(AuthorResourceParameters authorsResourceParameters);


    }
}
