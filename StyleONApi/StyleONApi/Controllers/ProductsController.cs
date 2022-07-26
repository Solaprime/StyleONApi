using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using StyleONApi.Entities;
using StyleONApi.Model;
using StyleONApi.Repository;
using StyleONApi.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// since my repository class are in asynchronous pattern , My controller should also be In Asyn format to avoid some funny errors
namespace StyleONApi.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/[controller]/{sellerId}/products")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, AppSeller")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    //  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        public ProductsController(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all Product for a single seller
        /// </summary>
        /// <param name="sellerId"></param>
        /// <returns></returns>


        //[HttpGet]
        //// [Authorize(Roles = "AppUser, AppSeller")]
        //public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProductForSeller(
        //  Guid sellerId)
        //{


        //    var ifSellerExist = await _repository.SellerExist(sellerId);

        //    if (!ifSellerExist)
        //    {
        //        return NotFound();
        //    }
        //    var result = await _repository.GetAllProducts(sellerId);
        //    if (result == null)
        //    {
        //        return BadRequest("Sorry we cant find what you are Looking For ");
        //    }
        //    return Ok(_mapper.Map<IEnumerable<ProductDto>>(result));

        //}













        /// <summary>
        /// Get a single product for a seller
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>

        [HttpGet("{productId}", Name = "GetProduct")]
        //    [Authorize(Roles = "AppUser, AppSeller")]
        public async Task<ActionResult<ProductDto>> GetProductForSeller(Guid sellerId, Guid productId)
        {

            var ifSellerExist = await _repository.SellerExist(sellerId);

            if (!ifSellerExist)
            {
                return NotFound();
            }
            var result = await _repository.GetProduct(sellerId, productId);
            if (result == null)
            {
                return NotFound("We cant find the Product you are Looking for");
            }
            return Ok(_mapper.Map<ProductDto>(result));

        }



        /// <summary>
        /// Creating a new Product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>



     
        [HttpPost]
        // [Authorize(Roles ="AppSeller")]
        public async Task<IActionResult> CreateProduct(Guid sellerId, [FromBody] ProductForCreationDto product)
        {
            var ifSellerExist = await _repository.SellerExist(sellerId);

            if (!ifSellerExist)
            {
                return NotFound();
            }

            var productToCreate = _mapper.Map<Product>(product);
            if (productToCreate == null)
            {
                return BadRequest("No products was entereed");
            }
            await _repository.CreateProduct(sellerId, productToCreate);
            await _repository.Save();


           

            // to test if the correct stuff was returrned

            return Ok(productToCreate);
        }

        /// <summary>
        /// Delete a resource
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpDelete("{productId}")]
        //  [Authorize(Roles = "AppSeller")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var productToDelete = await _repository.GetProduct(productId);
            if (productToDelete == null)
            {
                return NotFound("We can find the Product you are Lokking for");
            }
            await _repository.DeleteProduct(productToDelete);
            return NoContent();
        }

        /// <summary>
        /// Partially Update a seller
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        /// 

        [HttpPatch("{productId}")]
        public async Task<ActionResult> partiallyUpdateAProduct(Guid sellerId, Guid productId,
          [FromBody] JsonPatchDocument<ProductForUpdate> patchDocument)
        {

            var ifSellerExist = await _repository.SellerExist(sellerId);

            if (!ifSellerExist)
            {
                return NotFound();
            }

            var productFromRepo = await _repository.GetProduct(sellerId, productId);
            if (productFromRepo == null)
            {
                return NotFound();
            }
            var productToPatch = _mapper.Map<ProductForUpdate>(productFromRepo);
            // add some validation attribute
            patchDocument.ApplyTo(productToPatch);
            _mapper.Map(productToPatch, productFromRepo);
            _repository.UpdateProduct(productFromRepo);

            //   await  _repository.UpdateProduct(productToPatch);
            return NoContent();


        }

        /// <summary>
        /// Searching a list of product a seller has posted
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        /// 
        // Attach a route to this place


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> searchingproductforseller(Guid sellerid, [FromQuery] ProductResourceParameters productresourceparameter)
        {

            // check if seller exist 
            // call our searching method
            //we need to cast the list our searchingmethod returned
            var ifsellerexist = await _repository.SellerExist(sellerid);

            if (!ifsellerexist)
            {
                return NotFound();
            }

            var searchedproduct = await _repository.GetSearchingProduct(sellerid, productresourceparameter);
            if (searchedproduct == null)
            {
                return BadRequest($"sorry we cant find what you are looking for in this store you can make a preordr with this seller or try other sellers");
            }
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(searchedproduct));
        }


         [HttpGet("WithSeller/{productId}")]
        public async Task<ActionResult<ProductDto>> GetProductForSellerWithSellerInfor(Guid sellerId, Guid productId)
        {

            var ifSellerExist = await _repository.SellerExist(sellerId);

            if (!ifSellerExist)
            {
                return NotFound();
            }
            var result = await _repository.GetProductWithSeller(sellerId, productId);
            if (result == null)
            {
                return NotFound("We cant find the Product you are Looking for");
            }
            return Ok(_mapper.Map<ProductDto>(result));
          //  return Ok(result);

        }


          // And the Include method In the repo classs
          // Editing the ProductDto To include seller




        



        // Remove Unused Method 
        // Refacto product returm to have sellers Details
        // When Posting to a product the product u are mapped to always return seller has a null either change the Map and confirm for Kelvin





        ///Refacto Product ResourceParameters one for all Produc one to search only wat seller has posted
        ///




        //Refacto get all product posted by a single seller throwing exception stuff




        //Past a product to seller 
        //Retrieving a prooduct tby Id
        //Deleting works
        // Patcginh Worrks
    }
}
