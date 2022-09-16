using Shared;
using StyleONApi.Entities;
using System.Threading.Tasks;

namespace StyleONApi.AuthServices
{
   public  interface ITokenService
    {
        // flow to remove RefreshToken Async 
        // flow to invalidate token 

        Task<UserManagerResponse> GenerateJwtToken(ApplicationUser user);
        Task<UserManagerResponse> VerifyAndGenerateToken(TokenRequest tokenRequest);

    }
}
