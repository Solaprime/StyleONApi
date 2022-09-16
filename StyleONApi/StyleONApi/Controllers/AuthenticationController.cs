using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared;
using StyleONApi.AuthServices;
using StyleONApi.Entities;
using StyleONApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public AuthenticationController(IUserService userService,
           UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager, IMapper mapper)

        {
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;

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
                var result = await _userService.VerifyAndGenerateToken(tokenRequest);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            return BadRequest("Something Bad happened");
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

        [HttpPost("UpdateSeller")]
        public async Task<IActionResult> UpdateSeller([FromBody] SellerForUpdateDto sellerDto)
        {
            var selllerToUpdate = _mapper.Map<Seller>(sellerDto);
            var seller = await _userService.UpdateSeller(selllerToUpdate);
            if (seller.IsSuccess)
            {
                return Ok(seller.Message);
            }
            return BadRequest(seller);
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

