﻿using AutoMapper;
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
   
    [Route("api/[controller]/{sellerId}/products")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
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
       /// 
       /// </summary>
       /// <param name="sellerId"></param>
       /// <param name="product"></param>
       /// <returns></returns>
        [HttpPost]
        // [Authorize(Roles ="AppSeller")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
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
            return Created("Vibes ", productToCreate);
          //  return Ok(productToCreate);
        }

        /// <summary>
        /// Delete a resource
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpDelete("{productId}")]
        //  [Authorize(Roles = "AppSeller")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
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
        /// <param name="sellerId"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        /// <remarks>
        ///  Sample request(this request updated the product description)\
        /// Patch /authors/id  \
        ///    [ \
        ///          {  \
        ///               "op": "replace".  \
        ///               "path": "/description",  \
        ///                "value": "new first name" \
        ///          }   \
        ///    ]  \
        ///        
        ///
        /// </remarks>

        [HttpPatch("{productId}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Edit))]
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

        [HttpGet]
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
        /// <returns></returns>

         [HttpGet("WithSeller/{productId}")]
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

    }

}




