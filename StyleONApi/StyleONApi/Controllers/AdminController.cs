using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StyleONApi.AuthServices;
using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserService _userService;
        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, UserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
        }

        // Deleting a user

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody]string email)
        {
            // check for the email
            var userToDelete =  await _userManager.FindByEmailAsync(email);
            if (userToDelete == null)
            {
                return BadRequest("We cant find User");
            }
            // if user exist deleter user
            var result = await _userManager.DeleteAsync(userToDelete);
            if (result.Succeeded)
            {
                return Ok("uSER DELETEED SUCCESFULLy");
            }
            return BadRequest("pLS TRY Agib lATER");
        }



        // Bug to Fix

        [HttpGet("GetAllUserInaRole")]
        public async Task<IActionResult> GetAllUserInaRole([FromBody] string rolename)
        {
            if (rolename == null)
            {
                throw new ArgumentNullException(nameof(rolename));
            }
            var userInRole = await _userService.FindAllUserInRole(rolename);
            return Ok(userInRole);

        }
          
    }
}
