using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using StyleONApi.AuthServices;
using StyleONApi.Context;    //context is not meant to be In the controller remember to delete this
using StyleONApi.Entities; 
using StyleONApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StyleONApi.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly StyleONContext _context;    // delete and refactor
        public AuthenticationController(IUserService userService,
           UserManager<ApplicationUser> userManager,
           ITokenService tokenService,
            StyleONContext context, /// Delete and refactor 
        RoleManager<IdentityRole> roleManager, IMapper mapper)

        {
            _tokenService = tokenService;
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _context = context;     // dleter and refacto


        }
        [HttpPost("Register")]

        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUserAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Some properrtied are InvALID");
        }




        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // we login withn the Model
                var result = await _userService.LoginUserAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Some properrtied are not  InvALID");
        }



        // Get ALL rOLES
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var allRoles = await _userService.GetAllRole();
            return Ok(allRoles);
        }

        // gET aLL User
        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<ApplicationUserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            var result = _mapper.Map<IEnumerable<ApplicationUserDto>>(users);
            //  return Ok(users);
            return Ok(result);
        }



        // On;y admin can create a role
        // CREATING A Role
       
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            var result = await _userService.CreateRole(roleName);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }




        // Addd a role to a user
        [HttpPost("AddRoleToUser")]
        public async Task<IActionResult> AddUserToRole([FromBody] RoleEmail roleEmail)
        {
            var result = await _userService.AddUserToRole(roleEmail);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }

        }




        [HttpGet]
        [Route("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles([FromQuery] string email)
        {
            // check if the email is valid
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) // User does not exist
            {

                return BadRequest(new
                {
                    error = "User does not exist"
                });
            }

            // return the roles
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(roles);
        }




        [HttpPost("RemovefromRole")]
        public async Task<IActionResult> RemoveUserFromRole(RoleEmail roleEmail)
        {
            var result = await _userService.RemoveUserFromRole(roleEmail);
            if (result.IsSuccess)
            {
                return Ok(result.Message);

            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (ModelState.IsValid)
            {
               // var result = await _userService.VerifyAndGenerateToken(tokenRequest);
                var result = await _tokenService.VerifyAndGenerateToken(tokenRequest);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            return BadRequest("Something Bad happened");
            //when refreshing a token u needd to pass in the userId of the perosn refereshong th
            //token consider the flow if it is legit
        }

        // Findng a user
        // revamp this get shouldnt take in parameter fromBody
        [HttpGet("GetSingleUser")]
        public async Task<ActionResult<ApplicationUserDto>> FindUser([FromQuery] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("We cant find User");
            }
            var result = _mapper.Map<ApplicationUserDto>(user);
            return Ok(result);
        }

        //Refactor
        [HttpPost("UpdateSeller")]
        // [HttpPost("RegisterasSeller")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateSeller([FromBody] SellerForUpdateDto sellerDto)
        {
            var selllerToUpdate = _mapper.Map<Seller>(sellerDto);
            var result = await _userService.UpdateSeller(selllerToUpdate);
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result);
        }

        //From Upddate seeler we can try assign the role
         [HttpPost("RegisterasSeller")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RegisterasSeller([FromBody] SellerForUpdateDto sellerDto)
        {
               var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
            {

                return BadRequest("Something Bad occured");
              
                //Getting all claims
                // IEnumerable<Claim> claims = identity.Claims;

            }
            var resultId = identity.FindFirst("Id").Value;
            var selllerToUpdate = _mapper.Map<Seller>(sellerDto);
            selllerToUpdate.ApplicationUserId = Guid.Parse(resultId);
            var result = await _userService.RegisterasSeller(selllerToUpdate);
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result);
        }




        // you hit the forget endpoint ang get token and Id
        // u will need the token to reset the password, token is from message
        //api/auth/forgetPassword
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] EmailFlow email)
        {
            if (string.IsNullOrWhiteSpace(email.Email))
            {
                return BadRequest("The string is empty");
            }

            var result = await _userService.ForgetPasswordAsync(email.Email);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }




        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ResetPasswordAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            return BadRequest("Some Model validation Occur");
        }


        //How to Properly Logout, Refactor Logout 

        [HttpPost("Logout")]
        public async  Task<IActionResult> Logout([FromQuery]string userId)
        {
            //the refrshtoken i am workign with is null
            //    var refreshToken =  await  _context.RefreshTokens.Where(C => C.UserId == userId).FirstOrDefaultAsync();
            var refreshToken = await _context.RefreshTokens.Where(c => c.UserId == userId).ToListAsync();
            if (refreshToken == null)
            {
                return BadRequest("No refresh token exist for these user");
            }
            // check the refreshtoken dat has not been used
            var unUsedToken = new List<RefreshToken>();
            foreach (var item in refreshToken)
            {
                // flow to check if token has been used, you can also check if token has been revoked has the case may be 
                if (!item.IsUsed)
                {
                    unUsedToken.Add(item);
                }
            }

            // we can either delete all the Unuu=sed token or revoke those token or set the used property to true
            foreach (var item in unUsedToken)
            {
                _context.RefreshTokens.Remove(item);
                _context.SaveChanges();
            }
             // _context.RefreshTokens.Remove(refreshToken);
          //  _context.SaveChanges();
            return Ok("User Logged out Succesffully");
        }
        




        //api/auth/confirmEmail?userId&token
        //[HttpGet("ConfirmEmail")]
        //public async Task<IActionResult> ConfirmEmail(string userId, string token)
        //{
        //    if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
        //    {
        //        return NotFound();
        //    }
        //    var result = await _userService.ConfirmEmailAsync(userId, token);
        //    if (result.IsSuccess)
        //    {
        //        // we are redirecting the user to a static Html Page
        //        //  return Redirect($"{_configuration["AppUrl"]}/EmailConfirmed.html");
        //        return Ok($"About to confirm emaiil");
        //    }
        //    return BadRequest(result);
        //}


    }
}

