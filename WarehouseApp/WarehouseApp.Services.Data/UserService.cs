using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.Admin.UserManagement;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.UserManager;

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

        public async Task<OperationResult> GetAllUsersAsync(
            AllUsersWithRolesSearchFilterViewModel inputModel, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            IQueryable<ApplicationUser> allUsersQuery = userManager.Users
                .AsNoTracking();

            inputModel.TotalUsers = await allUsersQuery.CountAsync();

            if (!string.IsNullOrWhiteSpace(inputModel.SearchQuery))
            {
                allUsersQuery = allUsersQuery
                    .Where(u => u.Email!.ToLower().Contains(inputModel.SearchQuery.ToLower()));
            }

            allUsersQuery = allUsersQuery.OrderBy(u => u.Email);

            inputModel.TotalItemsBeforePagination = await allUsersQuery.CountAsync();

            if (inputModel.EntitiesPerPage <= 0)
            {
                inputModel.EntitiesPerPage = 5;
            }

            if (inputModel.EntitiesPerPage > 100)
            {
                inputModel.EntitiesPerPage = 100;
            }

            inputModel.TotalPages = (int)Math.Ceiling(inputModel.TotalItemsBeforePagination /
                                                (double)inputModel.EntitiesPerPage!.Value);

            if (inputModel.CurrentPage > inputModel.TotalPages)
            {
                inputModel.CurrentPage = inputModel.TotalPages > 0 ? inputModel.TotalPages : 1;
            }

            if (inputModel.CurrentPage <= 0)
            {
                inputModel.CurrentPage = 1;
            }

            allUsersQuery = allUsersQuery
                    .Skip(inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage!.Value - 1))
                    .Take(inputModel.EntitiesPerPage.Value);

            IEnumerable<string> allRoles = await roleManager.Roles
                .Select(r => r.Name!)
                .ToArrayAsync();

            inputModel.AllRoles = allRoles;

            IEnumerable<ApplicationUser> allUsers = await allUsersQuery
                .ToArrayAsync();

            var allUsersViewModel = new List<UserViewModel>();

            foreach (ApplicationUser currentUser in allUsers)
            {
                IEnumerable<string> roles = await userManager.GetRolesAsync(currentUser);

                allUsersViewModel.Add(new UserViewModel()
                {
                    Id = currentUser.Id.ToString(),
                    Email = currentUser.Email,
                    Roles = roles
                });
            }

            inputModel.Users = allUsersViewModel;

            return OperationResult.Ok();
        }

        public async Task<OperationResult> UserExistsByIdAsync(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

                return OperationResult.Ok();
        }

        public async Task<OperationResult> AssignUserToRoleAsync(Guid userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            bool roleExists = await roleManager.RoleExistsAsync(roleName);

            if (user == null || !roleExists)
            {
                return OperationResult.Failure(UserOrRoleNotFound);
            }

            bool alreadyInRole = await userManager.IsInRoleAsync(user, roleName);
            if (!alreadyInRole)
            {
                IdentityResult? result = await userManager
                    .AddToRoleAsync(user, roleName);

                if (!result.Succeeded)
                {
                    return OperationResult.Failure(FailedToAssignRole);
                }
            }

            return OperationResult.Ok();
        }

        public async Task<OperationResult> RemoveUserRoleAsync(Guid userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            bool roleExists = await roleManager.RoleExistsAsync(roleName);

            if (user == null || !roleExists)
            {    
                return OperationResult.Failure(UserOrRoleNotFound);
            }

            bool alreadyInRole = await userManager.IsInRoleAsync(user, roleName);
            if (alreadyInRole)
            {
                IdentityResult? result = await userManager
                    .RemoveFromRoleAsync(user, roleName);

                if (!result.Succeeded)
                {
                    return OperationResult.Failure(FailedToRemoveRole);
                }
            }

            return OperationResult.Ok();
        }

        public async Task<OperationResult> DeleteUserAsync(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            IdentityResult? result = await userManager
                .DeleteAsync(user);
            if (!result.Succeeded)
            {
                return OperationResult.Failure(FailedToDeleteUser);
            }

            return OperationResult.Ok();
        }
    }
}
