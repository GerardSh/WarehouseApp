using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Web.ViewModels.Admin.UserManagement;

namespace WarehouseApp.Services.Data
{
    public class UserService : BaseService, IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;

        public UserService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<AllUsersWithRolesViewModel> GetAllUsersAsync()
        {
            IEnumerable<ApplicationUser> allUsers = await userManager.Users
                .ToArrayAsync();

            IEnumerable<string> allRoles = await roleManager.Roles
                .Select(r => r.Name!)
                .ToArrayAsync();

            ICollection<UserViewModel> allUsersViewModel = new List<UserViewModel>();

            foreach (ApplicationUser user in allUsers)
            {
                IEnumerable<string> roles = await userManager.GetRolesAsync(user);

                allUsersViewModel.Add(new UserViewModel()
                {
                    Id = user.Id.ToString(),
                    Email = user.Email,
                    Roles = roles
                });
            }

            var allUsersWithRolesViewModel = new AllUsersWithRolesViewModel
            {
                Users = allUsersViewModel,
                AllRoles = allRoles
            };

            return allUsersWithRolesViewModel;
        }

        public async Task<bool> UserExistsByIdAsync(Guid userId)
        {
            ApplicationUser? user = await userManager
                .FindByIdAsync(userId.ToString());

            return user != null;
        }

        public async Task<bool> AssignUserToRoleAsync(Guid userId, string roleName)
        {
            ApplicationUser? user = await userManager
                .FindByIdAsync(userId.ToString());
            bool roleExists = await roleManager.RoleExistsAsync(roleName);

            if (user == null || !roleExists)
            {
                return false;
            }

            bool alreadyInRole = await userManager.IsInRoleAsync(user, roleName);
            if (!alreadyInRole)
            {
                IdentityResult? result = await userManager
                    .AddToRoleAsync(user, roleName);

                if (!result.Succeeded)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> RemoveUserRoleAsync(Guid userId, string roleName)
        {
            ApplicationUser? user = await userManager
                .FindByIdAsync(userId.ToString());
            bool roleExists = await roleManager.RoleExistsAsync(roleName);

            if (user == null || !roleExists)
            {
                return false;
            }

            bool alreadyInRole = await userManager.IsInRoleAsync(user, roleName);
            if (alreadyInRole)
            {
                IdentityResult? result = await userManager
                    .RemoveFromRoleAsync(user, roleName);

                if (!result.Succeeded)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            ApplicationUser? user = await userManager
                .FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return false;
            }

            IdentityResult? result = await userManager
                .DeleteAsync(user);
            if (!result.Succeeded)
            {
                return false;
            }

            return true;
        }
    }
}
