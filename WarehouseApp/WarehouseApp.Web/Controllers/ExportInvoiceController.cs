using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.ExportInvoice;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoiceDetail;
using static WarehouseApp.Common.OutputMessages.SuccessMessages.ExportInvoice;

namespace WarehouseApp.Web.Controllers
{
    [Authorize]
    [Route("Warehouse/{warehouseId:guid}/{controller}/{action=Index}/{id:guid?}")]
    public class ExportInvoiceController : BaseController<ExportInvoiceController>
    {
        private readonly IExportInvoiceService exportInvoiceService;

        private readonly HashSet<string> knownClientErrors = new HashSet<string>
        {
            DuplicateInvoice,
            ProductNotFoundInImportInvoice,
            InsufficientStock,
            CannotExportBeforeImportDate,
            DuplicateProduct,
            CannnotDeleteAllExports,
            ExportDuplicate
        };

        public ExportInvoiceController(IExportInvoiceService exportInvoiceService, ILogger<ExportInvoiceController> logger)
            : base(logger)
        {
            this.exportInvoiceService = exportInvoiceService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(AllExportInvoicesSearchFilterViewModel inputModel)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult result =
                await exportInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userGuid);

            if (!result.Success)
            {
                logger.LogError(result.ErrorMessage);

                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            TempData["SearchQuery"] = inputModel.SearchQuery ?? string.Empty;
            TempData["ClientName"] = inputModel.ClientName ?? string.Empty;
            TempData["YearFilter"] = inputModel.YearFilter ?? string.Empty;
            TempData["EntitiesPerPage"] = inputModel.EntitiesPerPage;
            TempData["CurrentPage"] = inputModel.CurrentPage;

            return View(inputModel);
        }

        [HttpGet]
        public IActionResult Create(Guid warehouseId)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            var model = new CreateExportInvoiceInputModel()
            {
                Date = DateTime.Now,
                WarehouseId = warehouseId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateExportInvoiceInputModel inputModel)
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

            OperationResult result = await exportInvoiceService.CreateExportInvoiceAsync(inputModel, userGuid);

            if (!result.Success)
            {
                if (knownClientErrors.Contains(result.ErrorMessage!))
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                    return View(inputModel);
                }

                logger.LogError(result.ErrorMessage);

                TempData["ErrorMessage"] = result.ErrorMessage ?? ErrorMessages.ExportInvoice.CreationFailure;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            TempData["Message"] = CreationSuccess;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid warehouseId, Guid id)
        {
            string userId = GetUserId()!;
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult<ExportInvoiceDetailsViewModel> result =
                await exportInvoiceService.GetExportInvoiceDetailsAsync(warehouseId, id, userGuid);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = NoPermissionOrExportInvoiceNotFound;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid warehouseId, Guid id, bool returnToDetails = false)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;


            TempData["ReturnToDetails"] = returnToDetails;

            var result = await exportInvoiceService.GetExportInvoiceForEditingAsync(warehouseId, id, userGuid);

            if (!result.Success)
            {
                logger.LogError(result.ErrorMessage);

                TempData["ErrorMessage"] = result.ErrorMessage ?? EditingFailure;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditExportInvoiceInputModel inputModel)
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

            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userGuid);

            if (!result.Success)
            {
                if (knownClientErrors.Contains(result.ErrorMessage!) ||
                    result.ErrorMessage!.StartsWith(InvalidDate) ||
                    result.ErrorMessage!.StartsWith("Cannot set quantity for product"))
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                    return View(inputModel);
                }

                logger.LogError(result.ErrorMessage);

                TempData["ErrorMessage"] = result.ErrorMessage ?? EditingFailure;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            TempData["Message"] = EditingSuccess;

            if (TempData["ReturnToDetails"] != null && (bool)TempData["ReturnToDetails"]!)
            {
                return RedirectToAction(nameof(Details), new { warehouseId = inputModel.WarehouseId, id = inputModel.Id });
            }

            var searchQuery = TempData.Peek("SearchQuery") as string ?? string.Empty;
            var clientName = TempData.Peek("ClientName") as string ?? string.Empty;
            var yearFilter = TempData.Peek("YearFilter") as string ?? string.Empty;
            var entitiesPerPage = TempData.Peek("EntitiesPerPage") as int? ?? 5;
            var currentPage = TempData.Peek("CurrentPage") as int? ?? 1;

            return RedirectToAction(nameof(Index), new { searchQuery, clientName, yearFilter, entitiesPerPage, currentPage });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid warehouseId, Guid id)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult result = await exportInvoiceService.DeleteExportInvoiceAsync(warehouseId, id, userGuid);

            var searchQuery = TempData.Peek("SearchQuery") as string ?? string.Empty;
            var clientName = TempData.Peek("ClientName") as string ?? string.Empty;
            var yearFilter = TempData.Peek("YearFilter") as string ?? string.Empty;
            var entitiesPerPage = TempData.Peek("EntitiesPerPage") as int? ?? 5;
            var currentPage = TempData.Peek("CurrentPage") as int? ?? 1;

            if (!result.Success)
            {
                if (knownClientErrors.Contains(result.ErrorMessage!))
                {
                    TempData["Message"] = result.ErrorMessage;
                    return RedirectToAction(nameof(Index), new { searchQuery, clientName, yearFilter, entitiesPerPage, currentPage });
                }

                logger.LogError(result.ErrorMessage);

                TempData["ErrorMessage"] = result.ErrorMessage ?? ErrorMessages.ExportInvoice.DeletionFailure;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            TempData["Message"] = DeletionSuccess;

            return RedirectToAction(nameof(Index), new { searchQuery, clientName, yearFilter, entitiesPerPage, currentPage });
        }
    }
}
