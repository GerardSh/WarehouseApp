using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.Shared;
using WarehouseApp.Web.ViewModels.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;

namespace WarehouseApp.Web.Controllers
{
    [Authorize]
    public class WarehouseController : BaseController<WarehouseController>
    {
        private readonly IWarehouseService warehouseService;

        public WarehouseController(IWarehouseService warehouseService, ILogger<WarehouseController> logger)
            : base(logger)
        {
            this.warehouseService = warehouseService;
        }

        public async Task<IActionResult> Index(AllWarehousesSearchFilterViewModel inputModel)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult result
                = await warehouseService.GetWarehousesForUserAsync(inputModel, userGuid);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            return View(inputModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            var model = new CreateWarehouseInputModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateWarehouseInputModel model)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OperationResult result =
                await warehouseService.CreateWarehouseAsync(model, userGuid);

            if (!result.Success)
            {
                if (result.ErrorMessage == UserNotFound)
                {
                    TempData["ErrorMessage"] = UserNotFound;
                    return RedirectToAction("Error", "Home", new { statusCode = 403 });
                }

                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? CreationFailure);
                return View(model);
            }

            TempData["Message"] = CreationSuccess;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            string userId = GetUserId()!;
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult<WarehouseDetailsViewModel> result =
                await warehouseService.GetWarehouseDetailsAsync(id, userGuid);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = NoPermissionOrWarehouseNotFound;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, bool returnToDetails)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult<EditWarehouseInputModel> result =
                await warehouseService.GetWarehouseForEditingAsync(id, userGuid);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = NoPermissionOrWarehouseNotFound;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditWarehouseInputModel model)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OperationResult result = 
                await warehouseService.UpdateWarehouseAsync(model, userGuid);

            if (!result.Success)
            {
                if (result.ErrorMessage == WarehouseDuplicateName)
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage ?? EditingFailure);
                    return View(model);
                }

                TempData["ErrorMessage"] = NoPermissionOrWarehouseNotFound;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            TempData["Message"] = EditingSuccess;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult result = await warehouseService.DeleteWarehouseAsync(id, userGuid);

            if (!result.Success)
            {
                if (result.ErrorMessage == AlreadyDeleted)
                {
                    TempData["InfoMessage"] = AlreadyDeleted;
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = result.ErrorMessage ?? DeletionFailure;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            TempData["Message"] = DeletionSuccess;
            return RedirectToAction(nameof(Index));
        }
    }
}
