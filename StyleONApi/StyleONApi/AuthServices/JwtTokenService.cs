using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Shared;
using StyleONApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StyleONApi.Context;
using StyleONApi.Repository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace StyleONApi.AuthServices
{
    public class JwtTokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly StyleONContext _context;
        private readonly ISellerRepository _sellerRepository;
       

        private readonly TokenValidationParameters _tokenValidationParams;
        public JwtTokenService(UserManager<ApplicationUser> userManger,
       IConfiguration configuration, RoleManager<IdentityRole> roleManager,

            TokenValidationParameters tokenValidationParams,
            ISellerRepository sellerRepository,
            

       StyleONContext context)
        {

            _userManager = userManger;
            _configuration = configuration;
            _roleManager = roleManager;
            _context = context;
            _sellerRepository = sellerRepository;
            _tokenValidationParams = tokenValidationParams;


        }


        //helper method
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
                // we can passs in additonal claim to these flow 
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


        // private async Task<string> GenerateGJwtToken(IDentityUser user)
        // private async Task<string> GenerateJwtToken(ApplicationUser user)
        public  async Task<UserManagerResponse> GenerateJwtToken(ApplicationUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);
            // gET all valid claims for a user
            var claims = await GetAllValidClaims(user);
            //describes the token Flow
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(5),    //AddSeconds(20) for test sake
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
                // you can configure the issuer, audience and other properties here
            };
            // create the token from a given description
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            //write the token
            var jwtToken = jwtTokenHandler.WriteToken(token);

            // Refresh token Flow, use to genrate a new token when a jwt token has expired
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
    }
}
