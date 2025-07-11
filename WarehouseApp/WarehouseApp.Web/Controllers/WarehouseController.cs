using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.Shared;
using WarehouseApp.Web.ViewModels.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

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
            string userId = GetUserId()!;

            Guid userGuid = Guid.Empty;

            bool userIdValid = ValidateUserId(userId, ref userGuid);

            if (!userIdValid)
            {
                return RedirectToAction(nameof(Index));
            }

            var filteredWarehouses = await warehouseService.GetWarehousesForUserAsync(inputModel, userGuid);

            AllWarehousesSearchFilterViewModel model = new AllWarehousesSearchFilterViewModel()
            {
                Warehouses = filteredWarehouses,
                SearchQuery = inputModel.SearchQuery,
                YearFilter = inputModel.YearFilter,
                CurrentPage = inputModel.CurrentPage,
                EntitiesPerPage = inputModel.EntitiesPerPage,
                TotalPages = (int)Math.Ceiling((double)inputModel.TotalItemsBeforePagination /
                                               inputModel.EntitiesPerPage!.Value)
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            string? userId = GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new CreateWarehouseInputModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateWarehouseInputModel model)
        {
            string userId = GetUserId()!;

            Guid userGuid = Guid.Empty;

            bool userIdValid = ValidateUserId(userId, ref userGuid);

            if (!userIdValid)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OperationResult<Guid> addResult = await warehouseService.CreateWarehouseAsync(model, userGuid);

            if (!addResult.Success)
            {
                ModelState.AddModelError(string.Empty, addResult.ErrorMessage ?? CreationFailure);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
