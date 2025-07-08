using WarehouseApp.Web.ViewModels.Admin.UserManagement;

namespace WarehouseApp.Services.Data.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<AllUsersViewModel>> GetAllUsersAsync();

        Task<bool> UserExistsByIdAsync(Guid userId);

        Task<bool> AssignUserToRoleAsync(Guid userId, string roleName);

        Task<bool> RemoveUserRoleAsync(Guid userId, string roleName);

        Task<bool> DeleteUserAsync(Guid userId);
    }
}