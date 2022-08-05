using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using StyleONApi.Entities;
using StyleONApi.Helpers;
using StyleONApi.Model;
using StyleONApi.Repository;
using StyleONApi.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductsCollectionController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        public ProductsCollectionController(IProductRepository productRepository, IMapper mapper)
        {
            _repository = productRepository;
            _mapper = mapper;
        }

        // Re visit this Method it has no seller
        //[HttpPost()]
        //public async Task<ActionResult<IEnumerable<ProductForCreationDto>>> 
        //    CreatingMultipleProduct(IEnumerable<ProductForCreationDto> products)
        //{
        //    var productsToCreate = _mapper.Map<IEnumerable<Product>>(products);
        //    if (productsToCreate == null)
        //    {
        //        return BadRequest("No products was entereed");
        //    }
        //    await _repository.CreateMultipleProduct(productsToCreate);
        //    return Ok(productsToCreate);
        //}

        [HttpGet()]
        //  [Authorize(Roles ="AppUser, AppSeller")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProduct(
        [FromQuery] ProductResourceParameters productResourceParameter)
        {
            var result = await _repository.GetAllProducts(productResourceParameter);
            if (result == null)
            {
                return BadRequest("Sorry we cant find what you are Looking For ");
            }
            //return Ok(result);
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(result));

        }

        [HttpPost("{sellerId}")]
        public async Task<ActionResult<ProductDtoTest>> AddMultipleProduct(Guid sellerId,
            [FromBody] IEnumerable<ProductForCreationDto> product)
        {
            var ifSellerExist = await _repository.SellerExist(sellerId);

            if (!ifSellerExist)
            {
                return NotFound();
            }
            var productsToCreate = _mapper.Map<IEnumerable<Product>>(product);
            if (productsToCreate == null)
            {
                return BadRequest("No products was entereed");
            }
            foreach (var productItem in productsToCreate)
            {
                await _repository.CreateProduct(sellerId,  productItem);
                await _repository.Save();
            }


            var productDtoCollectionToReturn = _mapper.Map<IEnumerable<ProductDtoTest>>(productsToCreate);
            var idAsString = string.Join(",", productDtoCollectionToReturn.Select(a => a.ProductId ));

            return CreatedAtRoute("GetProductCollection", new { sellerId = sellerId, ids = idAsString },
                productDtoCollectionToReturn);
          //  return Ok();

        }

         [HttpGet("{sellerId}/({ids})",  Name = "GetProductCollection")]
         public async  Task<ActionResult> GetProductForCollection(Guid sellerId, [FromRoute] 
         [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids )
         {
            if (ids == null)
            {
                return BadRequest();
            }
            var productEntities =   await _repository.GetProductCollection(sellerId, ids);
            if (ids.Count() != productEntities.Count()) 
            {
                return NotFound();
            }
            return Ok(productEntities);
         }


        //Method to edit

    }

}