using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared;
using StyleONApi.Context;
using StyleONApi.Entities;
using StyleONApi.Model;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly StyleONContext _context;

        private readonly TokenValidationParameters _tokenValidationParams;



        public UserService(UserManager<ApplicationUser> userManger,
       IConfiguration configuration, RoleManager<IdentityRole> roleManager,

            TokenValidationParameters tokenValidationParams,

       StyleONContext context)
        {
            _userManager = userManger;
            _configuration = configuration;
            _roleManager = roleManager;
            _context = context;

            _tokenValidationParams = tokenValidationParams;


        }

        public async Task<UserManagerResponse> AddUserToRole(RoleEmail roleEmail)
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

        public async Task<UserManagerResponse> CreateRole(string rolename)
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

        public async Task<IEnumerable<IdentityRole>> GetAllRole()
        {

            var allRoles = await _roleManager.Roles.ToListAsync();

            return allRoles;

        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
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
            var jwtTokenResponse = await GenerateJwtToken(user);
            //return new UserManagerResponse
            //{
            //    Message = "Login Succesfful ",
            //    IsSuccess = true,
            //    Token = jwtToken,

            //};
            return jwtTokenResponse;
               
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
            var identityUser = new ApplicationUser()
            {
                Email = model.Email,
                UserName = model.Email,
                //Id = new Guid()


            };
            var result = await _userManager.CreateAsync(identityUser, model.PassWord);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(identityUser, "AppUser");
                var jwtTokenResponse = await GenerateJwtToken(identityUser);
                //return new UserManagerResponse
                //{
                //    Message = "User Created Succesfully",
                //    IsSuccess = true,
                //    Token = jwtToken,
                //};
                return jwtTokenResponse;
            }
            return new UserManagerResponse
            {
                Message = "Unable to create user",
                IsSuccess = false,
                Error = result.Errors.Select(e => e.Description),


            };
        }

        public async Task<UserManagerResponse> RemoveUserFromRole(RoleEmail roleEmail)
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
        // private async Task<string> GenerateJwtToken(ApplicationUser user)
        private async Task<UserManagerResponse> GenerateJwtToken(ApplicationUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);
            // gET all valid claims for a user
            var claims = await GetAllValidClaims(user);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(5),    //AddSeconds(20) for test sake
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };




            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevorked = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = RandomString(35) + Guid.NewGuid()
            };


            // Check thE ID property of Refrshtoken
          
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();


            return new UserManagerResponse()
            {
                Token = jwtToken,
                IsSuccess = true,
                RefreshToken = refreshToken.Token
            };

        }





        // Getting all valid claims for the suer 
        // Incase i ant to add a claim


        private async Task<List<Claim>> GetAllValidClaims(ApplicationUser user)
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




        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }




        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
        }






        // Method to Genrate and RefreshToken
        public async Task<UserManagerResponse> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Validation 1 - Validation JWT token format
                _tokenValidationParams.ValidateLifetime = false;
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParams, out var validatedToken);
                _tokenValidationParams.ValidateLifetime = true;

                // Validation 2 - Validate encryption alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    // To check Validation Algoriothm
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        return null;
                    }
                }

                // Validation 3 - validate expiry date
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    return new UserManagerResponse()
                    {
                        IsSuccess = false,
                        Error = new List<string>() {
                            "Token has not yet expired"
                        }
                    };
                }

                // validation 4 - validate existence of the token
                var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedToken == null)
                {
                    return new UserManagerResponse()
                    {
                        IsSuccess = false,
                        Error = new List<string>() {
                            "Token does not exist"
                        }
                    };
                }

                // Validation 5 - validate if used
                if (storedToken.IsUsed)
                {
                    return new UserManagerResponse()
                    {
                        IsSuccess = false,
                        Error = new List<string>() {
                            "Token has been used"
                        }
                    };
                }

                // Validation 6 - validate if revoked
                if (storedToken.IsRevorked)
                {
                    return new UserManagerResponse()
                    {
                        IsSuccess = false,
                        Error = new List<string>() {
                            "Token has been revoked"
                        }
                    };
                }

                // Validation 7 - validate the id
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedToken.JwtId != jti)
                {
                    return new UserManagerResponse()
                    {
                        IsSuccess = false,
                        Error = new List<string>() {
                            "Token doesn't match"
                        }
                    };
                }

                // Validation 8 - validate stored token expiry date
                if (storedToken.ExpiryDate < DateTime.UtcNow)
                {
                    return new UserManagerResponse()
                    {
                        IsSuccess = false,
                        Error = new List<string>() {
                            "Refresh token has expired"
                        }
                    };
                }

                // update current token 

                storedToken.IsUsed = true;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();

                // Generate a new token
                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                return await GenerateJwtToken(dbUser);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Lifetime validation failed. The token is expired."))
                {

                    return new UserManagerResponse()
                    {
                        IsSuccess = false,
                        Error = new List<string>() {
                            "Token has expired please re-login"
                        }
                    };

                }
                else
                {
                    return new UserManagerResponse()
                    {
                        IsSuccess = false,
                        Error = new List<string>() {
                            "Something went wrong."
                        }
                    };
                }
            }
        }

        public async  Task<SimpleResponse> UpdateSeller(Seller seller)
        {
            if (seller == null)
            {
                throw new ArgumentNullException(nameof(seller));
            }
         
            // checj user exist'
            var user = await _userManager.FindByEmailAsync(seller.Email);
           
            if (user != null)
            {
                // check role
                var roleSeller = await _userManager.IsInRoleAsync(user, "AppSeller");
                if (roleSeller)
                {
                    seller.SellerId = Guid.NewGuid();
                    _context.Sellers.Add(seller);
                   await  _context.SaveChangesAsync();
                    return new SimpleResponse
                    {
                         IsSuccess = true,
                         Message = $"You are now ready to start Posting ur product On styleOn"
                    };
                }
                return new SimpleResponse
                {
                    IsSuccess = false,
                    Message = $"user with this email isnt registered as a Seller"
                };
            }
            return new SimpleResponse
            {
                IsSuccess = false,
                 Message = $"User with {seller.Email} does not exist in our Databse"
            };
         
          // Genreate Id, add seller to use flow
          // Add to sellr table
        }

        public async  Task<SimpleResponse>FindAllUserInRole(string roleName)
        {

            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (roleExist)
            {
                var userInRole = await _userManager.GetUsersInRoleAsync(roleName);
                if (userInRole == null)
                {
                    return new SimpleResponse
                    {
                          IsSuccess = true,
                          Message = $"No user with this {roleName} exist"
                    };
                }
                return new SimpleResponse
                {
                    IsSuccess = true,
                    Message = $"This are the user in this role",
                    ObjectToReturn = userInRole
                };
            }
          

            return new SimpleResponse
            {
                IsSuccess = false,
                Message = $" this role doesnt  exist",

            };
        }
        // Replacre the response and context
    }
}
