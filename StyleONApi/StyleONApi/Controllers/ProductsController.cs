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
    [Produces("application/json", "application/xml")]
    [Route("api/v{version:apiVersion}/[controller]/{sellerId}/products")]
    [ApiController]
    //  [ApiConventionType(typeof(DefaultApiConventions))]
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
        /// 
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        // [Authorize(Roles ="AppSeller")]
        // [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<ProductDtoTest>> CreateProduct(Guid sellerId, [FromBody] ProductForCreationDto product)
        {
            var ifSellerExist = await _repository.SellerExist(sellerId);

            if (!ifSellerExist)
            {
                return NotFound();
            }

            var productToCreate = _mapper.Map<Product>(product);
            // Redundant code in If Block
            if (productToCreate == null)
            {
                return BadRequest("No products was entereed");
            }
            await _repository.CreateProduct(sellerId, productToCreate);
            await _repository.Save();
            var productToReturn = _mapper.Map<ProductDtoTest>(productToCreate);
            // return Created("Vibes ", productToCreate);
            return CreatedAtRoute("GetProductForSeller",
                new { sellerId = sellerId, productId = productToReturn.ProductId }, 
                productToReturn);

        }



        /// <summary>
        /// Partially Update a seller
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="sellerId"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        /// <remarks>
        ///  Sample request(this request updated the  **product description** )
        /// Patch /authors/id  
        ///    [ 
        ///          {  
        ///               "op": "replace".  
        ///               "path": "/description",  
        ///                "value": "new first name" 
        ///          }   
        ///    ]  
        ///        
        ///
        /// </remarks>

        [HttpPatch("{productId}")]
        // [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Edit))]
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
        ///   Searchong A list of product posted by seller with the given sellerId. The Resource Parameters contains search Info
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="productresourceparameter"></param>
        /// <returns></returns>

        [HttpGet("SearchProductForSeller")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> searchingproductforseller(Guid sellerId, [FromQuery] ProductResourceParameters productresourceparameter)
        {
            var ifsellerexist = await _repository.SellerExist(sellerId);

            if (!ifsellerexist)
            {
                return NotFound();
            }

            var searchedproduct = await _repository.GetSearchingProduct(sellerId, productresourceparameter);
            if (searchedproduct == null)
            {
                return BadRequest($"sorry we cant find what you are looking for in this store you can make a preordr with this seller or try other sellers");
            }
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(searchedproduct));
        }
        /// <summary>
        /// Test Method ot retereive seller Infor with Produict
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="productId"></param>
        /// <returns> An actionresult of ProductDtoTest </returns>
        /// <response code = "200">Returns the Book</response>

        [HttpGet("{productId}", Name ="GetProductForSeller")]
        public async Task<ActionResult<ProductDtoTest>> GetProductForSellerWithSellerInfor(Guid sellerId, Guid productId)
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
            return Ok(_mapper.Map<ProductDtoTest>(result));

        }
        [HttpDelete("{productId}")]
        public async  Task<ActionResult> DeleteProductForSeller(Guid sellerId, Guid productId)
        {
            // Check id Seller Exist 
            // you get product 
            //YOu Delet Course
            if (await _repository.SellerExist(sellerId))
            {
                return NotFound();
            }
            var productForSeller = await _repository.GetProduct(sellerId, productId);
            if (productForSeller == null)
            {
                return NotFound();
            }
            await  _repository.DeleteProduct(productForSeller);
            return NoContent();
        }
    }

}




// You are to Mix Attribute Response types with the one in Startu[ to match your use case
// Test delete a product and deleting a sellecte
