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

        // TokenValidation parameters 
       // Contains a set of parameters that are used by a Microsoft.IdentityModel.Tokens.SecurityTokenHandler
        //     when validating a Microsoft.IdentityModel.Tokens.SecurityToken.
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

        //What is the use of these Method 
        //helper method
        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }



        //Wat is these Method Doing 
        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
        }

        // Getting all valid claims for the suer 
        // Incase i ant to add a claim


        /// <summary>
        /// This method Basically get all clains for the user 
        /// Depending on Ur use case Scenarion u can have Different means to work with Claims
        /// the claims  in these method is in three Part, Claims from thr user ItSelf,
        /// claims from the userManager flow,
        /// and claims from the Role flow 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<List<Claim>> GetAllValidClaims(ApplicationUser user)
        {
            // We are Configuring a Claim from the user Data, from the JWT configuration 
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),


                // we can passs in additonal claim to these flow 
            };

            // Getting the claims that we have assigned to the user
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // Get the Role associated with a user 
            var userRoles = await _userManager.GetRolesAsync(user);

            //  Note Certain claim Can be associated with certain Role

            // Iterate through the List of userRoles
            foreach (var userRole in userRoles)
            {

                //Find the specified role 
                var role = await _roleManager.FindByNameAsync(userRole);


                // Add the user role has  a claim 
                if (role != null)
                {
                    //Add the Role as  a claim 
                    claims.Add(new Claim(ClaimTypes.Role, userRole));

                    // Get all the Specific Claims associated to 
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

        // Generate Jwt 
        public  async Task<UserManagerResponse> GenerateJwtToken(ApplicationUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            // Wat is in the Appsettings.Json details for JWtonfig 
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);
            // 
            // gET all valid claims for a user, Using the Custom method we  Created  
            var claims = await GetAllValidClaims(user);
            //describes the token Flow
            //    SecurityTokenDescriptor class       Contains some information which used to create a security token.
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(20),    //AddSeconds(20) for test sake
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
                
                // you can configure the issuer, audience and other properties here
            };
            // create the token from a given description
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            //write the token, Not we need to have the token In compact form, so we need to Write it 
            var jwtToken = jwtTokenHandler.WriteToken(token);
           

            // Refresh token Flow, use to genrate a new token when a jwt token has expired
            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevorked = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                // A refreshToken Expires after  6 Months 
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                //The actual RefreshToken dat will be used 
                Token = RandomString(35) + Guid.NewGuid()
            };
            

            // Check thE ID property of Refrshtoken

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            // Return to the User the Jwt Token, and RefreshToken
            return new UserManagerResponse()
            {
                Token = jwtToken,
                IsSuccess = true,
                RefreshToken = refreshToken.Token
            };

        }


        //this flow is for refresh token and refresh token
        // this method basiccaly does the Refresh Token for us 
        public async Task<UserManagerResponse> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            // JwtSecurityTokenHandler class is  designed for creating and
            //     validating Json Web Tokens
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                //Token validation Parameters injhected here
                // TokenValidation parameters 
                // Contains a set of parameters that are used by a Microsoft.IdentityModel.Tokens.SecurityTokenHandler
                //     when validating a Microsoft.IdentityModel.Tokens.SecurityToken.


                // Validation 1 - Validation JWT token format



                // I assusme since u will be sending in an expired token then no need to validate the Token, Since it has expired
                _tokenValidationParams.ValidateLifetime = false;
                //thid flow validates the token
                //Note the Out Parameter that gives in the validatedToken
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParams, out var validatedToken);
                //Why is the Lifetime Validated here ????????????????????????????????????????
                //when it was false up


                

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
                    // How was the Comparison Done , the result varaible which will holds the comparison seems someHow
                }

                // Validation 3 - validate expiry date
                // using the Tokenvalidation Parameters to extract data from the Token(Which is the expiry date), then we use the Long.parse method
                // to convert the string to a number and int  
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                // Convert the int to date time
                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                //Since token works by adding minutes or seconds from the current Datetime has expirey date
                //if the Expirydate is greater than uTcNow it means few minutes from now the token is gonna expire
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

                //Check if the RefreshToken Exist in our 
                var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                //IF token does not exist 
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
                //Check if token has been revoked 
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


                //It seems we pass the Jwt.Id property in the refreshtoken to be eqaul to the Jti Property when writing out JWt
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

                //Check if expired token has Expired 
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


                // Update the Current RefreshToken used 
                storedToken.IsUsed = true;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();

                // Generate a new token

                // Find the UserId 
                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                //Genrate the new JwtToken and return the New Genrated Token
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

        // Remove the user RefreshtOken, why is that ????????
        // Probably remove or refacto this code 
        public async  Task<bool> RemoveRefreshTokenAsync(ApplicationUser user)
        {
           // get the user refresh token
           var userRefreshToken = await _context.RefreshTokens
                .Where(c => c.UserId == user.Id).FirstOrDefaultAsync();
            if (userRefreshToken == null)
            {
                return false;
            }
            if (userRefreshToken != null)
            {
                _context.RefreshTokens.Remove(userRefreshToken);
            }
            return false;
        }
    }
}


// Means to extract data from a jwt Token
//new Claim("Id", user.Id),
//new Claim(JwtRegisteredClaimNames.Email, user.Email),

//var demoTest = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value;
//var demoId = tokenInVerification.Claims.FirstOrDefault(x => x.Value == "Id").Value;
