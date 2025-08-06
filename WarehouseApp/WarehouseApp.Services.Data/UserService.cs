using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository.Interfaces;
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

        private readonly IWarehouseService warehouseService;
        private readonly IApplicationUserWarehouseRepository appUserWarehouseRepo;

        public UserService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IWarehouseService warehouseService,
            IApplicationUserWarehouseRepository appUserWarehouseRepo)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.warehouseService = warehouseService;
            this.appUserWarehouseRepo = appUserWarehouseRepo;
        }

        /// <summary>
        /// Retrieves all users along with their roles and fills the provided view model
        /// with paginated and filtered results based on the input criteria.
        /// Validates the requesting user before proceeding.
        /// </summary>
        /// <param name="inputModel">
        /// A view model containing filtering, pagination, and output data such as total users, current page, and the result list.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the operation. Used to verify that the requester exists.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success or failure. On success, the <paramref name="inputModel"/> is populated with the results.
        /// </returns>
        public async Task<OperationResult> GetAllUsersAsync(
            AllUsersWithRolesSearchFilterViewModel inputModel, Guid userId)
        {
            try
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
            catch
            {
                return OperationResult.Failure(GetAllUsersFailure);
            }
        }

        /// <summary>
        /// Checks whether a user with the specified ID exists in the system.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user to check.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success if the user exists,
        /// or failure with a specific reason if the user is not found or an error occurs.
        /// </returns>
        public async Task<OperationResult> UserExistsByIdAsync(Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(UserExistsFailure);
            }
        }

        /// <summary>
        /// Assigns a user to a specific role if both the user and role exist,
        /// and the user is not already in that role.
        /// </summary>
        /// <param name="userId">The ID of the user to assign the role to.</param>
        /// <param name="roleName">The name of the role to assign.</param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success if the role was assigned,
        /// or failure if the user or role is not found, the user is already in the role,
        /// or an error occurs during assignment.
        /// </returns>
        public async Task<OperationResult> AssignUserToRoleAsync(Guid userId, string roleName)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                bool roleExists = await roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    return OperationResult.Failure(RoleNotFound);
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
            catch
            {
                return OperationResult.Failure(AssignRoleFailure);
            }
        }

        /// <summary>
        /// Removes a user from a specific role if both the user and role exist,
        /// and the user is currently assigned to that role.
        /// </summary>
        /// <param name="userId">The ID of the user to remove the role from.</param>
        /// <param name="roleName">The name of the role to remove.</param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success if the role was removed,
        /// or failure if the user or role is not found, the user is not in the role,
        /// or an error occurs during the removal.
        /// </returns>
        public async Task<OperationResult> RemoveUserRoleAsync(Guid userId, string roleName)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                bool roleExists = await roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    return OperationResult.Failure(RoleNotFound);
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
            catch
            {
                return OperationResult.Failure(RemoveRoleFailure);
            }
        }

        /// <summary>
        /// Deletes a user by their ID. If the user is associated with any warehouses,
        /// the associations are removed. For each warehouse, if no other users are linked to it,
        /// the warehouse is marked as deleted (soft delete).
        /// Changes to warehouses and mappings are saved after the user is deleted.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success if the user (and possibly warehouses)
        /// were successfully processed and deleted; otherwise, a failure result with an error message.
        /// </returns>
        public async Task<OperationResult> DeleteUserAsync(Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                var mappings = await appUserWarehouseRepo.GetAllByUserIdAsync(userId);

                if (mappings != null && mappings.Any())
                {
                    foreach (var mapping in mappings)
                    {
                        var warehouseId = mapping.WarehouseId;

                        appUserWarehouseRepo.Delete(mapping);

                        bool hasOtherUsers = await appUserWarehouseRepo
                            .ExistsAsync(uw => uw.WarehouseId == warehouseId && uw.ApplicationUserId != userId);

                        if (!hasOtherUsers)
                        {
                            await warehouseService.MarkAsDeletedWithoutSavingAsync(warehouseId);
                        }
                    }
                }

                IdentityResult? result = await userManager
                    .DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return OperationResult.Failure(FailedToDeleteUser);
                }

                await appUserWarehouseRepo.SaveChangesAsync();

                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(DeletionFailure);
            }
        }
    }
}
