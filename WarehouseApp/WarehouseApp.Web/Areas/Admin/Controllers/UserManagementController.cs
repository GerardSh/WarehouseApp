using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Web.ViewModels.Admin.UserManagement;
using WarehouseApp.Web.Controllers;
using static WarehouseApp.Common.Constants.ApplicationConstants;
using static WarehouseApp.Common.Constants.RolesConstants;
using WarehouseApp.Services.Data.Models;

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
        public async Task<IActionResult> Index(AllUsersWithRolesSearchFilterViewModel inputModel)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult result
                = await userService
                .GetAllUsersAsync(inputModel, userGuid);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            if (inputModel.Users.Count() == 0)
            {
                TempData["Message"] = "No users found!";
            }

            TempData["CurrentPage"] = inputModel.CurrentPage;
            TempData["SearchQuery"] = inputModel.SearchQuery ?? string.Empty;
            TempData["EntitiesPerPage"] = inputModel.EntitiesPerPage;

            return View(inputModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult userExists = await userService
                .UserExistsByIdAsync(userGuid);
            if (!userExists.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            OperationResult assignResult = await userService
                .AssignUserToRoleAsync(userGuid, role);
            if (!assignResult.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            var currentPage = TempData.Peek("CurrentPage") as int? ?? 1;
            var searchQuery = TempData.Peek("SearchQuery") as string ?? string.Empty;
            var entitiesPerPage = TempData.Peek("EntitiesPerPage") as int? ?? 5;

            return RedirectToAction("Index", new { currentPage, searchQuery, entitiesPerPage });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult userExists = await userService
                .UserExistsByIdAsync(userGuid);
            if (!userExists.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            OperationResult removeResult = await userService
                .RemoveUserRoleAsync(userGuid, role);
            if (!removeResult.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            var currentPage = TempData.Peek("CurrentPage") as int? ?? 1;
            var searchQuery = TempData.Peek("SearchQuery") as string ?? string.Empty;
            var entitiesPerPage = TempData.Peek("EntitiesPerPage") as int? ?? 5;

            return RedirectToAction("Index", new { currentPage, searchQuery, entitiesPerPage });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult userExists = await userService
                .UserExistsByIdAsync(userGuid);
            if (!userExists.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            OperationResult removeResult = await userService
                .DeleteUserAsync(userGuid);
            if (!removeResult.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            var currentPage = TempData.Peek("CurrentPage") as int? ?? 1;
            var searchQuery = TempData.Peek("SearchQuery") as string ?? string.Empty;
            var entitiesPerPage = TempData.Peek("EntitiesPerPage") as int? ?? 5;

            return RedirectToAction("Index", new { currentPage, searchQuery, entitiesPerPage });
        }
    }
}
