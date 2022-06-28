using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StyleONApi.Entities;
using StyleONApi.Model;
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
        private readonly IMapper _mapper;
        public ProductsController(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


       

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProduct()
        {
            var result = await _repository.GetAllProducts();
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(result));

        }


        [HttpGet("{productId}", Name = "GetProduct")]
        public async Task<ActionResult<ProductDto>> GetProductsById(Guid productId)
        {
            var result = await _repository.GetProduct(productId);
            if (result == null)
            {
                return NotFound("We cant find the Product you are Looking for");
            }
            return Ok(_mapper.Map<ProductDto>(result));
        }

  

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductForCreationDto product)
        {
            var productToCreate = _mapper.Map<Product>(product);
            if (productToCreate == null)
            {
                return BadRequest("No products was entereed");
            }
            await _repository.CreateProduct(productToCreate);


            // to test if the correct stuff was returrned

            return Ok(productToCreate);
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
