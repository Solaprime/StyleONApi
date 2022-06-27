using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StyleONApi.Entities;
using StyleONApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        public ProductsController(IProductRepository repository)
        {
            _repository = repository;
        }


        // Get all Authors
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAllProduct()
        {
            var result = _repository.GetAllProducts();
            return Ok(result);
        }

        // Get Single Author
        [HttpGet("{productId}", Name = "GetAuthor")]
        public ActionResult<Product> GetProduct(Guid productId)
        {
            var result = _repository.GetProduct(productId);
            if (result == null)
            {
                return NotFound("Product Not Found");
            }
            return Ok(result);
        }
       
    }
}
