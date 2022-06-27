using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product> GetProduct(Guid productId);
        Task CreateProduct(Product product);


        //  Check how multipe products are created
        //Task CreateMultipleProduct(IEnumerable<Product> products);


        //Check how delte work
        //Task  DelteProduct(Guid productId);

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
