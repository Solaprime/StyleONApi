using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StyleONApi.Entities;
using StyleONApi.Repository;
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
        IProductRepository _repository;
        public SellersController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Seller>>> GetAllSellers()
        {
            var result = await _repository.GetAllSellers();
            return Ok(result);
        }
            
    }
}
