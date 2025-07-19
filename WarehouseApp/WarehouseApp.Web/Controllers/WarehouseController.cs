using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.Shared;
using WarehouseApp.Web.ViewModels.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.SuccessMessages.Warehouse;

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

            TempData["SearchQuery"] = inputModel.SearchQuery ?? string.Empty;
            TempData["YearFilter"] = inputModel.YearFilter ?? string.Empty;
            TempData["EntitiesPerPage"] = inputModel.EntitiesPerPage;
            TempData["CurrentPage"] = inputModel.CurrentPage;

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
        public async Task<IActionResult> Edit(Guid id, bool returnToDetails = false)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            TempData["ReturnToDetails"] = returnToDetails;

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
        public async Task<IActionResult> Edit(EditWarehouseInputModel inputModel)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            if (!ModelState.IsValid)
            {
                return View(inputModel);
            }

            OperationResult result =
                await warehouseService.UpdateWarehouseAsync(inputModel, userGuid);

            if (!result.Success)
            {
                if (result.ErrorMessage == WarehouseDuplicateName)
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage ?? EditingFailure);
                    return View(inputModel);
                }

                TempData["ErrorMessage"] = NoPermissionOrWarehouseNotFound;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            TempData["Message"] = EditingSuccess;

            if (TempData["ReturnToDetails"] != null && (bool)TempData["ReturnToDetails"]!)
            {
                return RedirectToAction(nameof(Details), new { id = inputModel.Id });
            }

            var currentPage = TempData.Peek("CurrentPage") as int? ?? 1;
            var searchQuery = TempData.Peek("SearchQuery") as string ?? string.Empty;
            var yearFilter = TempData.Peek("YearFilter") as string ?? string.Empty;
            var entitiesPerPage = TempData.Peek("EntitiesPerPage") as int? ?? 5;

            return RedirectToAction(nameof(Index), new { currentPage, searchQuery, yearFilter, entitiesPerPage });
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

            var currentPage = TempData.Peek("CurrentPage") as int? ?? 1;
            var searchQuery = TempData.Peek("SearchQuery") as string ?? string.Empty;
            var yearFilter = TempData.Peek("YearFilter") as string ?? string.Empty;
            var entitiesPerPage = TempData.Peek("EntitiesPerPage") as int? ?? 5;

            if (!result.Success)
            {
                if (result.ErrorMessage == AlreadyDeleted)
                {
                    TempData["Message"] = AlreadyDeleted;
                    return RedirectToAction(nameof(Index), new { currentPage, searchQuery, yearFilter, entitiesPerPage });
                }

                TempData["ErrorMessage"] = result.ErrorMessage ?? DeletionFailure;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            TempData["Message"] = DeletionSuccess;

            return RedirectToAction(nameof(Index), new { currentPage, searchQuery, yearFilter, entitiesPerPage });
        }
    }
}
