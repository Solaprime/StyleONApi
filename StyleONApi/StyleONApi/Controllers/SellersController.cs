using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StyleONApi.Model;
using StyleONApi.Repository;
using StyleONApi.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Controllers
{
//[Route("api/[controller]")]
    
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class SellersController : ControllerBase
    {
        private readonly ISellerRepository _sellerrepository;
        private readonly IMapper _mapper;

        public SellersController(ISellerRepository sellerrepository, IMapper mapper)
        {
            _sellerrepository = sellerrepository;
            _mapper = mapper;
        }

        [HttpGet("GetAllUpdatedSellers")]
        public async Task<ActionResult<IEnumerable<SellerDto>>> GetAllSellers()
        {
            var result = await _sellerrepository.GetSellers();
            var sellerToReturn = _mapper.Map<IEnumerable<SellerDto>>(result);
            return Ok(sellerToReturn);   
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SellerDto>>> GetAllSellers(
            [FromQuery] SelllersResourceParameters resourceParameters)
        {
            var result = await _sellerrepository.GetSellers(resourceParameters);
            if (result == null)
            {
                return BadRequest("Soory we cant find wat you are Looking For");
            }
            return Ok(_mapper.Map<IEnumerable<SellerDto>>(result));
        }
        

        //Does my businees requirement require me to delete seller a

        //[HttpDelete("{sellerId}")]
        //public  async Task<ActionResult> DeleteSeller(Guid sellerId)
        //{
        //    var sellersFromRepo =  await _sellerrepository.GetSeller(sellerId);
        //    if (sellersFromRepo == null)
        //    {
        //        return NotFound();
        //    }
        //  await  _sellerrepository.DeleteSeller(sellersFromRepo);
        //    return NoContent();
        //}

        //Sellers shoulb be able partially update their details
        // Write a Logic such that each row in a seller table has a unique email
        // In our Api i treid Updating a seller with the same email and it went through
    }
}



