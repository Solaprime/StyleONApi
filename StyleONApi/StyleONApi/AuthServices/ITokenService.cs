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

        Task<bool> RemoveRefreshTokenAsync(ApplicationUser user);
    }
}
// write a endpoint in the refreshtokem endpoint the emthod will pick the expired token sent to the Db 
//the expired token is then picked ans stored, pur custom middleware will use the flow as the blacklist