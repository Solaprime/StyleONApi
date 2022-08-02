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
    // [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
     [ProducesResponseType(StatusCodes.Status500InternalServerError)]
   // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
        }

        /// <summary>
        /// Deleting a User
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>

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
        /// <summary>
        /// Get all User in a role specified in the Body of the Reques
        /// </summary>
        /// <param name="rolename"></param>
        /// <returns> A List of all User confirm the return typs</returns>
        /// <response code ="200">Returns the List of User</response>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        [HttpGet("GetAllUserInaRole")]
        public async Task<IActionResult> GetAllUserInaRole([FromBody] string rolename)
        {
            if (rolename == null)
            {
                throw new ArgumentNullException(nameof(rolename));
            }
            var roleExist = await _roleManager.RoleExistsAsync(rolename);
            if (roleExist)
            {
                var userInRole = await _userService.FindAllUserInRole(rolename);
                return Ok(userInRole);
            }
            return NotFound("Role does not Exist ");
         

        }
          
    }
}
// Add a propety to Access seller from ApplicationUser flow