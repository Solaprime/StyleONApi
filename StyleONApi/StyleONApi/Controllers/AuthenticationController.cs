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
        public async Task<IActionResult> GetUserRoles([FromBody] string email)
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
        [HttpGet("GetSingleUser")]
        public async Task<ActionResult<ApplicationUserDto>> FindUser([FromBody] string email)
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
    }
}

