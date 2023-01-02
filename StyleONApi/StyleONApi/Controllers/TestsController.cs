using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StyleONApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class TestsController : ControllerBase
    {


        public TestsController()
        {
            
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //  [Authenticate]

        [HttpGet]
        public IActionResult Get()
        {

            //Where will the JwtToken come from, 
            //You can either mAnually pass the toekn, or u find a means to extract 
            //the token from The authentication header or anywhwre
            //var jwtTokenHandler = new JwtSecurityTokenHandler();
            //var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParams, out var validatedToken);
            //var demoTest = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value;
            //return Ok(demoTest);


            //var demoTest = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value;
            //var demoId = tokenInVerification.Claims.FirstOrDefault(x => x.Value == "Id").Value;

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity!= null)
            {
                //Getting all claims
                // IEnumerable<Claim> claims = identity.Claims;
                var result =  identity.FindFirst("Id").Value;
                return Ok(result);

            }

            return Ok("No identity");

        }

        [HttpGet("Noauthorization")]
        public IActionResult GetFlow()
        {
            return Ok("no Authorization");
        }
    }
}
