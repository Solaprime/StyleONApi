using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StyleONApi.AuthServices;
using StyleONApi.Entities;
using StyleONApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StyleONApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
      //  private readonly IUserService _userService; 
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(IMapper mapper, IOrderService orderService, UserManager<ApplicationUser> userManger)
        {
            _mapper = mapper;
            _orderService = orderService;
            _userManager = userManger;
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //A mean to Return the Order
        public async Task<IActionResult> CreateOrder([FromBody]OrderDto order)
        {

            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
            {

                return BadRequest("Something Bad occured");

                //Getting all claims
                // IEnumerable<Claim> claims = identity.Claims;
            }
            var resultId = identity.FindFirst("Id").Value;

            //   var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            // Find User by Id to 
            var userMakingtheOrder = await _userManager.FindByIdAsync(resultId);

            var ordertoCreate = _mapper.Map<Order>(order);

            ordertoCreate.userFlow = userMakingtheOrder;
            var result =  await _orderService.CreateOrder(ordertoCreate);
            if (result.IsSuccess)
            {
                return Ok(result);

                // You can mapp stuff to return the Order created
            }
            return BadRequest("Bad stuff happends");
        }


        [HttpGet("{orderid}")]
        public async Task<ActionResult<OrderDtoToReturn>> GetOrder(Guid orderid)
        {
            var orders = await _orderService.GetOrder(orderid);
            var orderToReturn = _mapper.Map<OrderDtoToReturn>(orders);
            return Ok(orderToReturn);
        }
    }
}
 