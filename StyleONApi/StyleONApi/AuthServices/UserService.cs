//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared;
using StyleONApi.Context;
using StyleONApi.Entities;
using StyleONApi.Model;
using StyleONApi.Repository;
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
        private readonly ISellerRepository _sellerRepository;
        private readonly ITokenService _tokenService;
        private readonly TokenValidationParameters _tokenValidationParams;

        // HttpContext
      //  private readonly HttpContext _httpContext;



        public UserService(UserManager<ApplicationUser> userManger,
       IConfiguration configuration, RoleManager<IdentityRole> roleManager,
              ITokenService service,
            TokenValidationParameters tokenValidationParams,
            ISellerRepository sellerRepository,
       StyleONContext context)
        {
            
            _tokenService = service;
            _userManager = userManger;
            _configuration = configuration;
            _roleManager = roleManager;
            _context = context;
            _sellerRepository = sellerRepository;
            _tokenValidationParams = tokenValidationParams;
          //  _httpContext = httpContext;


        }

        public async Task<UserManagerResponse> AddUserToRole(RoleEmail roleEmail)
        {
            // Check if user with that email Exist.
            var user = await _userManager.FindByEmailAsync(roleEmail.Email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "User can not be Found",
                    IsSuccess = false
                };
            }

            // Check if the Role you wish to add the user to Exist 
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
            // If Role Does not exist ,
            if (!roleExist)
            {
                // Create the Role 
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(rolename));
                if (roleResult.Succeeded)
                {
                    return new UserManagerResponse
                    {
                        Message = $"The role {rolename} has been Created succesfully",
                        IsSuccess = true,
                    };
                }
                else
                {
                    return new UserManagerResponse
                    {
                        IsSuccess = false,
                        Message = $"The role {rolename} was not  created, Kindly try Again "
                        //  Message =   roleResult.Errors.ToString()
                    };
                }
            }
            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Role already exist, You can  not  create an Existing Role"
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
            // Check if user with Email with Email Wxist 
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {

                return new UserManagerResponse
                {
                    Message = "No user with this Email exist in our Databse",
                    IsSuccess = false
                };

            }
            // Check user Passsowrd
            var result = await _userManager.CheckPasswordAsync(user, model.PassWord);
            if (!result)
            {
                return new UserManagerResponse
                {
                    Message = "Invalid Password ",
                    IsSuccess = false
                };

            }
            // From above no nned to tell the user which is wrong. Just return a generic massage like 
            //Password and Email are not matching
            // var jwtTokenResponse = await GenerateJwtToken(user);

            // If all case checks our U generate Jwt  flow 
            var jwtTokenResponse = await _tokenService.GenerateJwtToken(user);
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
                    Message = "This user, with this Email  already exist",
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
                //  var jwtTokenResponse = await GenerateJwtToken(identityUser);
                var jwtTokenResponse = await _tokenService.GenerateJwtToken(identityUser);
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
                Message = "Unable to create user, Kindly try again",
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
                    Message = "Role can not be Found, this given role cant be found.",
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



        public async  Task<UserManagerResponse> RegisterasDispatch(Dispatch dispatch)
        {
            var user_Exist = await _userManager.FindByEmailAsync(dispatch.Email);
            if (user_Exist == null)
            {
                return new UserManagerResponse
                {
                    Message = "This user, with this Email  does not   exist",
                    IsSuccess = false,
                };
            }

            // For sellerfor update due to how i createed my flow
            //i used seller id to extablish relationship between them httpcintext abd the Likes
            dispatch.userId = user_Exist;
            var roleSeller = await _userManager.IsInRoleAsync(user_Exist, "AppDispatch");
            if (roleSeller)
            {
                return new UserManagerResponse
                {
                    Message = "This user, with this Email  has registered as a Dispatch",
                    IsSuccess = false,
                };
            }

            var roleEmail = new RoleEmail { Email = dispatch.Email, RoleName = "AppDispatch" };
            var result = await AddUserToRole(roleEmail);

            await _context.Dispatchs.AddAsync(dispatch);
            await _context.SaveChangesAsync();
            return result;

        }


        public async Task<UserManagerResponse> RegisterasSeller(Seller seller)
        {
            //Email Check
            var user_Exist = await _userManager.FindByEmailAsync(seller.Email);
            if (user_Exist == null)
            {
                return new UserManagerResponse
                {
                    Message = "This user, with this Email  does not   exist",
                    IsSuccess = false,
                };
            }

            // Check if email has been added to seller tole before

            var roleSeller = await _userManager.IsInRoleAsync(user_Exist, "AppSeller");
            if (roleSeller)
            {
                return new UserManagerResponse
                {
                    Message = "This user, with this Email  has registered as a seller",
                    IsSuccess = false,
                };
            }
            // Some Info to pass in

            // Like Date tiem regissters and other info to pass in

            //Genrate seeler Id



            //Add seller to role
            var roleEmail = new RoleEmail { Email = seller.Email, RoleName = "AppSeller" };
            var result = await AddUserToRole(roleEmail);

            //U need to connect the userId


            //  var httpContext = new HttpContext();
          //  var identity = _httpContext.User.Identity as ClaimsIdentity;
            //   var identity = HttpContext.User.Identity as ClaimsIdentity;

            //if (identity == null)
            //{

            //    return new UserManagerResponse
            //    {
            //        Message = "Something bad occured has occured as ",
            //        IsSuccess = false,
            //    };
            //    //Getting all claims
            //    // IEnumerable<Claim> claims = identity.Claims;

            //}


            //else
            //{
            //    var userId = identity.FindFirst("Id").Value;
            //    seller.ApplicationUserId = Guid.Parse(userId);

            //}





            await _context.Sellers.AddAsync(seller);
            await _context.SaveChangesAsync();
            return result;




        }


        public async Task<SimpleResponse> UpdateSeller(Seller seller)
        {
            if (seller == null)
            {
                throw new ArgumentNullException(nameof(seller));
            }

            // checj user exist'
            var user = await _userManager.FindByEmailAsync(seller.Email);

            // You have not even added the seller to that role
            if (user != null)
            {
                // check role
                var roleSeller = await _userManager.IsInRoleAsync(user, "AppSeller");
                if (roleSeller)
                {

                    // Start 
                    var ifSellerHasNotUpdated = await _sellerRepository.CheckIfSellershasnotUpdated(seller);
                    if (!ifSellerHasNotUpdated)
                    {
                        return new SimpleResponse
                        {
                            IsSuccess = false,
                            Message = $"A seller with this  Email is already registered as a seller"

                        };
                    }

                    // Finish



                    seller.SellerId = Guid.NewGuid();
                    //var identity = HttpContext.User.Identity as ClaimsIdentity;
                    //if (identity != null)
                    //{
                    //   var result = identity.FindFirst("Id").Value;



                    //}

                    //var identity = HttpContext.User.Identity as ClaimsIdentity;
                    //if (identity != null)
                    //{
                    //    //Getting all claims
                    //    // IEnumerable<Claim> claims = identity.Claims;
                    //    var result = identity.FindFirst("Id").Value;
                    //  //  return Ok(result);

                    //}

                    _context.Sellers.Add(seller);
                    await _context.SaveChangesAsync();


                    // Send a Mail to seller telling them Dere details have been Updated
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
        

        

        public async Task<SimpleResponse> FindAllUserInRole(string roleName)
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



        //// Flow to confirm email
        //public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        //{
        //    // It is in two part, If user regsuter succedffully we genrae a token den send to the user email to confirm.
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return new UserManagerResponse
        //        {
        //            IsSuccess = false,
        //            Message = "User can not be found",
        //        };
        //    }
        //    // U need to decode the toekn
        //    var decodedToken = WebEncoders.Base64UrlDecode(token);
        //    string normalToken = Encoding.UTF8.GetString(decodedToken);
        //    var result = await _userManager.ConfirmEmailAsync(user, normalToken);
        //    if (result.Succeeded)
        //    {
        //        return new UserManagerResponse
        //        {
        //            Message = "Email Confirmed Succesffully",
        //            IsSuccess = true,
        //        };
        //    }
        //    return new UserManagerResponse
        //    {
        //        IsSuccess = false,
        //        Message = "Email did not confirm",
        //        Error = result.Errors.Select(e => e.Description)
        //    };
        //}






        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user with this email exist"
                };
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // this token mught contain some special charcters like forward slash, --- dat might not load well or be rejected on browser
            // so we need to encode it
            var encodedEmailToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
            // If for web
            //  we can generate a link to the website in a web page
            //   this mail to send mail

            //string Url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";

            //// send the url via the mail service
            //await _emailService.SendEmailAsync(email, "Reset Password", "<h1>Follow the instructions to reset Your Password</h1>" +
            //    $"<p>reset your password <a href='{Url}'>Click here bro</a></p>");

            var userId = await _userManager.GetUserIdAsync(user);



            //  var result = await _userManager.Ch
            return new UserManagerResponse
            {
                IsSuccess = true,
                // Message = "Reset Password Url has been sent to YOU succesfully"
                Message = validToken,
                Id = userId
            };

        }

        //flow to ResetPassword Async
        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "User can not be found",
                };
            }
            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.Password);
            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    IsSuccess = true,
                    Message = "Password has been Resetted succesfully"
                };
            }
            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Somwthing went wrong Kidlu try again later",
                Error = result.Errors.Select(e => e.Description)
            };

        }


        // find a meanse to coonstruct the claims pricncipal from the jwtToken  flow 
        public async Task<UserManagerResponse> LogoutAsync(ApplicationUser user)
        {
            var refreshToken = await _context.RefreshTokens.Where(c => c.UserId == user.Id).FirstOrDefaultAsync();

            if (refreshToken == null)
            {
                return new UserManagerResponse { IsSuccess = true };
            }

            _context.RefreshTokens.Remove(refreshToken);

            var saveResponse = await _context.SaveChangesAsync();

            if (saveResponse >= 0)
            {
                return new UserManagerResponse { IsSuccess = true };
            }

            return new UserManagerResponse { IsSuccess = false, Message = "Unable to logout user" };

        }

        
    }


    // Write a custom method for Email Checker insteard of having to repaet urself
}
