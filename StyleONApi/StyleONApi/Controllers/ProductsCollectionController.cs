using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using StyleONApi.Entities;
using StyleONApi.Helpers;
using StyleONApi.Model;
using StyleONApi.Repository;
using StyleONApi.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace StyleONApi.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsCollectionController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsCollectionController> _logger;
        public ProductsCollectionController(IProductRepository productRepository, IMapper mapper,
            ILogger<ProductsCollectionController> logger)
        {
            _repository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

  // Pagination data are passed as query Parameters ,
  // For best practice sake we returm a custom header that wull containd our pagination metadata
        [HttpGet(Name ="GetAllProduct")]
        //  [Authorize(Roles ="AppUser, AppSeller")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProduct(
        [FromQuery] ProductResourceParameters productResourceParameter)
        {
          


            // the flow for Login user
            var userId = User.Claims.FirstOrDefault(a=> a.Type == "Id")?.Value;
            _logger.LogInformation(message:"{Username} with {userId} with{role} is about to Get all the books",
                User.Identity.Name, userId );
            
            var result = await _repository.GetAllProducts(productResourceParameter);
            //if (result == null)
            //{
            //    return BadRequest("Sorry we cant find what you are Looking For ");
            //}



            var previousPageLink = result.HasPrevious ?
                CreateAuthorsResourceUri(productResourceParameter,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = result.HasNext ?
                CreateAuthorsResourceUri(productResourceParameter,
                ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = result.TotalCount,
                pageSize = result.PageSize,
                currentPage = result.CurrentPage,
                totalPages = result.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

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
            // Log dat seeler with sellerId abput to create product
            var productsToCreate = _mapper.Map<IEnumerable<Product>>(product);
            if (productsToCreate == null)
            {
                return BadRequest("No products was entereed");
            }
            foreach (var productItem in productsToCreate)
            {
                //Log product has been saved
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

        // Sice we are dealing with pagination we need to generate a link that will 
        // Lead to the previouse and NexrPage



        private string CreateAuthorsResourceUri(
           ProductResourceParameters productresourceParameters,
           ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAllProduct",
                      new
                      {
                          pageNumber = productresourceParameters.PageNumber - 1,
                          pageSize = productresourceParameters.PageSize,
                          mainCategory = productresourceParameters.MainCategory,
                          searchQuery = productresourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetAllProduct",
                      new
                      {
                          pageNumber = productresourceParameters.PageNumber + 1,
                          pageSize = productresourceParameters.PageSize,
                          mainCategory = productresourceParameters.MainCategory,
                          searchQuery = productresourceParameters.SearchQuery
                      });

                default:
                    return Url.Link("GetAllProduct",
                    new
                    {
                        pageNumber = productresourceParameters.PageNumber,
                        pageSize = productresourceParameters.PageSize,
                        mainCategory = productresourceParameters.MainCategory,
                        searchQuery = productresourceParameters.SearchQuery
                    });
            }

        }



    }

}