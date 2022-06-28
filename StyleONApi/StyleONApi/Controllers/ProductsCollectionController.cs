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

namespace StyleONApi.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost()]
        public async Task<ActionResult<IEnumerable<ProductForCreationDto>>> CreatingMultipleProduct(IEnumerable<ProductForCreationDto> products)
        {
            var productsToCreate = _mapper.Map<IEnumerable<Product>>(products);
            if (productsToCreate == null)
            {
                return BadRequest("No products was entereed");
            }
            await _repository.CreateMultipleProduct(productsToCreate);
            return Ok(productsToCreate);
        }
    }
}