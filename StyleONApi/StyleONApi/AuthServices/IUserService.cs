using Microsoft.AspNetCore.Identity;
using Shared;
using StyleONApi.Entities;
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

        //Task<IdentityRole> GetUserRoles(string email);
    }
}
