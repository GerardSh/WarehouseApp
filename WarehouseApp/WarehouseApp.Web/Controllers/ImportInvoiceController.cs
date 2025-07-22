using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.ImportInvoice;
using WarehouseApp.Common.OutputMessages;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoiceDetail;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Product;
using static WarehouseApp.Common.OutputMessages.SuccessMessages.ImportInvoice;

namespace WarehouseApp.Web.Controllers
{
    [Authorize]
    [Route("Warehouse/{warehouseId:guid}/{controller}/{action=Index}/{id:guid?}")]
    public class ImportInvoiceController : BaseController<ImportInvoiceController>
    {
        private readonly IImportInvoiceService importInvoiceService;

        private readonly HashSet<string> knownClientErrors = new HashSet<string>
        {
            DuplicateInvoice,
            ProductDuplicate,
            CannotCreateInvoiceWithoutProducts,
            ProductDeletionFailure,
            QuantityRange,
            ExistingExportInvoices,
            CannnotDeleteAllProducts
        };

        public ImportInvoiceController(IImportInvoiceService importInvoiceService, ILogger<ImportInvoiceController> logger)
            : base(logger)
        {
            this.importInvoiceService = importInvoiceService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(AllImportInvoicesSearchFilterViewModel inputModel)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult result =
                await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userGuid);

            if (!result.Success)
            {
                logger.LogError(result.ErrorMessage);

                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            TempData["SearchQuery"] = inputModel.SearchQuery ?? string.Empty;
            TempData["YearFilter"] = inputModel.YearFilter ?? string.Empty;
            TempData["SupplierName"] = inputModel.SupplierName ?? string.Empty;
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

            var model = new CreateImportInvoiceInputModel()
            {
                Date = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateImportInvoiceInputModel inputModel)
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

            OperationResult result = await importInvoiceService.CreateImportInvoiceAsync(inputModel, userGuid);

            if (!result.Success)
            {
                if (knownClientErrors.Contains(result.ErrorMessage!))
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                    return View(inputModel);
                }

                logger.LogError(result.ErrorMessage);

                TempData["ErrorMessage"] = result.ErrorMessage ?? ErrorMessages.ImportInvoice.CreationFailure;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            TempData["Message"] = CreationSuccess;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid warehouseId, Guid id)
        {
            string userId = GetUserId()!;
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult<ImportInvoiceDetailsViewModel> result =
                await importInvoiceService.GetImportInvoiceDetailsAsync(warehouseId, id, userGuid);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = NoPermissionOrImportInvoiceNotFound;
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

            var result = await importInvoiceService.GetImportInvoiceForEditingAsync(warehouseId, id, userGuid);

            if (!result.Success)
            {
                logger.LogError(result.ErrorMessage);

                TempData["ErrorMessage"] = result.ErrorMessage ?? EditingFailure;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditImportInvoiceInputModel inputModel)
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

            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userGuid);

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
            var supplierName = TempData.Peek("SupplierName") as string ?? string.Empty;
            var yearFilter = TempData.Peek("YearFilter") as string ?? string.Empty;
            var entitiesPerPage = TempData.Peek("EntitiesPerPage") as int? ?? 5;
            var currentPage = TempData.Peek("CurrentPage") as int? ?? 1;

            return RedirectToAction(nameof(Index), new { searchQuery, supplierName, yearFilter, entitiesPerPage, currentPage });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid warehouseId, Guid id)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult result = await importInvoiceService.DeleteImportInvoiceAsync(warehouseId, id, userGuid);

            var searchQuery = TempData.Peek("SearchQuery") as string ?? string.Empty;
            var supplierName = TempData.Peek("SupplierName") as string ?? string.Empty;
            var yearFilter = TempData.Peek("YearFilter") as string ?? string.Empty;
            var entitiesPerPage = TempData.Peek("EntitiesPerPage") as int? ?? 5;
            var currentPage = TempData.Peek("CurrentPage") as int? ?? 1;

            if (!result.Success)
            {
                if (knownClientErrors.Contains(result.ErrorMessage!))
                {
                    TempData["Message"] = result.ErrorMessage;
                    return RedirectToAction(nameof(Index), new { searchQuery, supplierName, yearFilter, entitiesPerPage, currentPage });
                }

                logger.LogError(result.ErrorMessage);

                TempData["ErrorMessage"] = result.ErrorMessage ?? ErrorMessages.ImportInvoice.DeletionFailure;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            TempData["Message"] = DeletionSuccess;

            return RedirectToAction(nameof(Index), new { searchQuery, supplierName, yearFilter, entitiesPerPage, currentPage });
        }
    }
}
