using Microsoft.AspNetCore.Identity;
using Shared;
using StyleONApi.Entities;
using StyleONApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.AuthServices
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model);
        Task<UserManagerResponse> LoginUserAsync(LoginViewModel model);
        Task<UserManagerResponse> CreateRole(string name);
        Task<IEnumerable<IdentityRole>> GetAllRole();
        Task<IEnumerable<ApplicationUser>> GetAllUsers();
        Task<UserManagerResponse> AddUserToRole(RoleEmail roleEmail);
        Task<UserManagerResponse> RemoveUserFromRole(RoleEmail roleEmail);

        Task<UserManagerResponse> VerifyAndGenerateToken(TokenRequest tokenRequest);
        Task<SimpleResponse> UpdateSeller(Seller seller);
        Task<SimpleResponse> FindAllUserInRole(string roleName);
        //Task<IdentityRole> GetUserRoles(string email);

        // Logout
        //How to restrict a seller from Posting
        //Account LockOut
        //
    }
}
