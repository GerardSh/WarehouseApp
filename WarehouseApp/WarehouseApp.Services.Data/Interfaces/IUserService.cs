using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.Admin.UserManagement;

namespace WarehouseApp.Services.Data.Interfaces
{
    public interface IUserService
    {
        Task<OperationResult> GetAllUsersAsync(
            AllUsersWithRolesSearchFilterViewModel inputModel, Guid userId);

        Task<OperationResult> UserExistsByIdAsync(Guid userId);

        Task<OperationResult> AssignUserToRoleAsync(Guid userId, string roleName);

        Task<OperationResult> RemoveUserRoleAsync(Guid userId, string roleName);

        Task<OperationResult> DeleteUserAsync(Guid userId);
    }
}