using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StyleONApi.AuthServices
{
    public class UserService : IUserService
    {
        // Identity Provide us with two class UserManger and RoleManager
        //private RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;



        public UserService(UserManager<IdentityUser> userManger,
       IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManger;
            _configuration = configuration;
            _roleManager = roleManager;


        }

        public  async Task<UserManagerResponse> AddUserToRole(RoleEmail roleEmail)
        {

            var user = await _userManager.FindByEmailAsync(roleEmail.Email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "User can not be Found",
                    IsSuccess = false
                };
            }


            var roleExist = await _roleManager.RoleExistsAsync(roleEmail.RoleName);
            if (!roleExist)
            {
                return new UserManagerResponse
                {
                    Message = "Role can not be Found",
                    IsSuccess = false
                };
            }



            var roleToBeAdded = await _userManager.AddToRoleAsync(user, roleEmail.RoleName);
            if (roleToBeAdded.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = $"{roleEmail.RoleName} role was added to {roleEmail.Email} ",
                    IsSuccess = true
                };
            }


            return new UserManagerResponse
            {
                Message = "Something Bad happenes Found",
                IsSuccess = false
            };

        }

        public  async Task<UserManagerResponse> CreateRole(string rolename)
        {
            var roleExist = await _roleManager.RoleExistsAsync(rolename);
            if (!roleExist)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(rolename));
                if (roleResult.Succeeded)
                {
                    return new UserManagerResponse
                    {
                        Message = $"The role {rolename} has been succeede succesfully",
                        IsSuccess = true,
                    };
                }
                else
                {
                    return new UserManagerResponse
                    {
                        IsSuccess = false,
                        Message = $"The role {rolename} was not  created, Kindly try Again"
                    };
                }
            }
            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Role already exist"
            };

        }

        public async  Task<IEnumerable<IdentityRole>> GetAllRole()
        {

            var allRoles = await _roleManager.Roles.ToListAsync();

            return allRoles;

        }

        public async  Task<IEnumerable<IdentityUser>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;

        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {

                return new UserManagerResponse
                {
                    Message = "No user with this Email exist in our Databse",
                    IsSuccess = false
                };

            }
            var result = await _userManager.CheckPasswordAsync(user, model.PassWord);
            if (!result)
            {
                return new UserManagerResponse
                {
                    Message = "Invalid Password ",
                    IsSuccess = false
                };

            }
            var jwtToken =  await GenerateJwtToken(user);
            return new UserManagerResponse
            {
                Message = "Login Succesfful ",
                IsSuccess = true,
                Token = jwtToken,

            };
        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model)
        {
            if (model == null)
            {
                throw new NullReferenceException("Our registerModel is null");
            }
           
            var user_Exist = await _userManager.FindByEmailAsync(model.Email);
            if (user_Exist != null)
            {
                return new UserManagerResponse
                {
                    Message = "This user already exist",
                    IsSuccess = false,
                };
            }
            var identityUser = new IdentityUser()
            {
                Email = model.Email,
                UserName = model.Email,
                //Id = new Guid()


            };
            var result = await _userManager.CreateAsync(identityUser, model.PassWord);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(identityUser, "AppUser");
                var jwtToken =  await GenerateJwtToken(identityUser);
                return new UserManagerResponse
                {
                    Message = "User Created Succesfully",
                    IsSuccess = true,
                    Token = jwtToken,
                };
            }
            return new UserManagerResponse
            {
                Message = "Unable to create user",
                IsSuccess = false,
                Error = result.Errors.Select(e => e.Description),


            };
        }

        public  async Task<UserManagerResponse> RemoveUserFromRole(RoleEmail roleEmail)
        {


            var user = await _userManager.FindByEmailAsync(roleEmail.Email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "User can not be Found",
                    IsSuccess = false
                };
            }


            var roleExist = await _roleManager.RoleExistsAsync(roleEmail.RoleName);
            if (!roleExist)
            {
                return new UserManagerResponse
                {
                    Message = "Role can not be Found",
                    IsSuccess = false
                };
            }
            var result = await _userManager.RemoveFromRoleAsync(user, roleEmail.RoleName);
            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "User was succesfully Remove from  role",
                    IsSuccess = true
                };
            }


            return new UserManagerResponse
            {
                Message = "Some shit happedned Found",
                IsSuccess = false
            };


        }

        // private async Task<string> GenerateGJwtToken(IDentityUser user)
        private async Task<string> GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);
            // gET all valid claims for a user
            var claims = await GetAllValidClaims(user);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {

                //Subject = new ClaimsIdentity(new[]
                //{
                //    new Claim("Id", user.Id),
                //    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())

                //}),

                Subject = new ClaimsIdentity(claims),
               // //  Expires = DateTime.Now.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)

            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);


            // Write code to refreshh token as welll


        }

        // Getting all valid claims for the suer 
        // Incase i ant to add a claim


        private async Task<List<Claim>> GetAllValidClaims(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Getting the claims that we have assigned to the user
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // Get the user role and add it to the claims
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));

                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            return claims;
        }


    }
}
