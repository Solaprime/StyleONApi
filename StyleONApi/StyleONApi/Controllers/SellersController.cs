using AutoMapper;
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

namespace StyleONApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellersController : ControllerBase
    {

        private readonly IProductRepository _repository;
        private readonly ISellerRepository _sellerrepository;
        private readonly IMapper _mapper;
        public SellersController(IProductRepository repository, IMapper mapper, ISellerRepository sellerRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _sellerrepository = sellerRepository;

        }


        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProduct()
        //{
        //    var result = await _repository.GetAllProducts();
        //    return Ok(_mapper.Map<IEnumerable<ProductDto>>(result));

        //}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SellerDto>>> GetAllSeller(
            [FromQuery] SellerResourceParameters resourceParameter)
        {
            var result = await _sellerrepository.GetSellers(resourceParameter);
            if (result == null)
            {
                return BadRequest("Sorry we cant find what you are Looking For ");
            }
            return Ok(_mapper.Map<IEnumerable<SellerDto>>(result));

        }



        [HttpGet("{sellerId}", Name = "GetSeller")]
        public async Task<ActionResult<SellerDto>> GetSellerById(Guid sellerId)
        {
            var result = await _sellerrepository.GetSingleSellerAsync(sellerId);
            if (result == null)
            {
                return NotFound("We cant find the Product you are Looking for");
            }
            return Ok(_mapper.Map<SellerDto>(result));
        }



        [HttpPost]
        public async Task<IActionResult> CreateSeller(SellerForCreationDto seller)
        {
            var sellerToCreate = _mapper.Map<Seller>(seller);
            if (sellerToCreate == null)
            {
                return BadRequest("No seller was entereed");
            }
            await _sellerrepository.CreateSeller(sellerToCreate);


            // to test if the correct stuff was returrned

            return Ok(sellerToCreate);
        }


        [HttpDelete("{sellerId}")]
        public async Task<IActionResult> DeleteSeller(Guid sellerId)
        {
            var SellerToDelete = await _sellerrepository.GetSingleSellerAsync(sellerId);
            if (SellerToDelete == null)
            {
                return NotFound("We can find the selller you are Lokking for");
            }
            _sellerrepository.DeleteSeller(SellerToDelete);
            return NoContent();
        }
        [HttpPatch("{sellerId}")]
        public async Task<ActionResult> partiallyUpdateASeller(Guid sellerId,
            JsonPatchDocument<SellerForUpdate> patchDocument)
        {
            var sellerFromRepo = await _sellerrepository.GetSingleSellerAsync(sellerId);
            if (sellerFromRepo == null)
            {
                return NotFound();
            }
            var sellerToPatch = _mapper.Map<SellerForUpdate>(sellerFromRepo);
            // add some validation attribute
            patchDocument.ApplyTo(sellerToPatch);
            _mapper.Map(sellerToPatch, sellerFromRepo);
            _sellerrepository.UpdateSeller(sellerFromRepo);

            //   await  _repository.UpdateProduct(productToPatch);
            return NoContent();


        }
    }
}
