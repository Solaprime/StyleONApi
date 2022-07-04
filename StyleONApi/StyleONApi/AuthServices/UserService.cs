using Microsoft.AspNetCore.Identity;
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
        private UserManager<IdentityUser> _userManger;
        private IConfiguration _configuration;



        public UserService(UserManager<IdentityUser> userManger,
       IConfiguration configuration)
        {
            _userManger = userManger;
            _configuration = configuration;

        }
        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManger.FindByEmailAsync(model.Email);
            if (user == null)
            {

                return new UserManagerResponse
                {
                    Message = "No user with this Email exist in our Databse",
                    IsSuccess = false
                };

            }
            var result = await _userManger.CheckPasswordAsync(user, model.PassWord);
            if (!result)
            {
                return new UserManagerResponse
                {
                    Message = "Invalid Password ",
                    IsSuccess = false
                };

            }
            var jwtToken = GenerateJwtToken(user);
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
            if (model.PassWord != model.ConfirmPassword)
            {
                return new UserManagerResponse
                {

                    Message = "Confirm password is not matching with password",
                    IsSuccess = false,

                };

            }
            var user_Exist = await _userManger.FindByEmailAsync(model.Email);
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
            var result = await _userManger.CreateAsync(identityUser, model.PassWord);
            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "User Created Succesfully",
                    IsSuccess = true,
                };
            }
            return new UserManagerResponse
            {
                Message = "Unable to create user",
                IsSuccess = false,
                Error = result.Errors.Select(e => e.Description),


            };
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())

                }),
                //  Expires = DateTime.Now.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)

            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);


        }
    }
}
