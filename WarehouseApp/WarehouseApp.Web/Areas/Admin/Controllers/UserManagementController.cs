using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Web.ViewModels.Admin.UserManagement;
using WarehouseApp.Web.Controllers;
using static WarehouseApp.Common.Constants.ApplicationConstants;
using static WarehouseApp.Common.Constants.RolesConstants;

namespace WarehouseApp.Web.Areas.Admin.Controllers
{
    [Area(AdminArea)]
    [Authorize(Roles = AdminRoleName)]
    public class UserManagementController : BaseController<UserManagementController>
    {
        private readonly IUserService userService;

        public UserManagementController(IUserService userService, ILogger<UserManagementController> logger)
            : base(logger)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            AllUsersWithRolesViewModel allUsersWithRoles = await userService
                .GetAllUsersAsync();

            return View(allUsersWithRoles);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            bool userExists = await userService
                .UserExistsByIdAsync(userGuid);
            if (!userExists)
            {
                return RedirectToAction(nameof(Index));
            }

            bool assignResult = await userService
                .AssignUserToRoleAsync(userGuid, role);
            if (!assignResult)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            bool userExists = await userService
                .UserExistsByIdAsync(userGuid);
            if (!userExists)
            {
                return RedirectToAction(nameof(Index));
            }

            bool removeResult = await userService
                .RemoveUserRoleAsync(userGuid, role);
            if (!removeResult)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            bool userExists = await userService
                .UserExistsByIdAsync(userGuid);
            if (!userExists)
            {
                return RedirectToAction(nameof(Index));
            }

            bool removeResult = await userService
                .DeleteUserAsync(userGuid);
            if (!removeResult)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
