using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StyleONApi.Entities;
using StyleONApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// since my repository class are in asynchronous pattern , My controller should also be In Asyn format to avoid some funny errors
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


        //[HttpGet]
        //public async Task<IActionResult> GetAllProduct()
        //{
        //    var result = await _repository.GetAllProducts();
        //    return Ok(result);

        //}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProduct()
        {
            var result = await _repository.GetAllProducts();
            return Ok(result);

        }


        [HttpGet("{productId}", Name = "GetProduct")]
        public async Task<ActionResult<Product>> GetProductsById(Guid productId)
        {
            var result = await _repository.GetProduct(productId);
            if (result == null)
            {
                return NotFound("We cant find the Product you are Looking for");
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
             await _repository.CreateProduct(product);
            //return CreatedAtRoute("GetProduct");
            return Ok();
        }

        [HttpDelete("{productId}")]
        public  async  Task<IActionResult> DeleteProduct(Guid productId)
        {
            var productToDelete = await _repository.GetProduct(productId);
            if (productToDelete == null)
            {
                return NotFound("We can find the Product you are Lokking for");
            }
            await  _repository.DeleteProduct(productToDelete);
            return NoContent();
        }
    }
}
