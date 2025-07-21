using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.ExportInvoice;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoice;
using static WarehouseApp.Common.OutputMessages.SuccessMessages.ExportInvoice;

namespace WarehouseApp.Web.Controllers
{
    [Authorize]
    [Route("Warehouse/{warehouseId:guid}/{controller}/{action=Index}/{id:guid?}")]
    public class ExportInvoiceController : BaseController<ExportInvoiceController>
    {
        private readonly IExportInvoiceService exportInvoiceService;

        private readonly HashSet<string> knownClientErrors = new HashSet<string>
        {   DuplicateInvoice,
            ProductNotFoundInImportInvoice,
            InsufficientStock,
            CannotExportBeforeImportDate,
            DuplicateProduct
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
            TempData["YearFilter"] = inputModel.YearFilter ?? string.Empty;
            TempData["ClientName"] = inputModel.ClientName ?? string.Empty;
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

            var model = new CreateExportInvoiceInputModel()
            {
                Date = DateTime.Now
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

            OperationResult<ExportInvoiceDetailsViewModel> result =
                await exportInvoiceService.GetExportInvoiceDetailsAsync(warehouseId, id, userGuid);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = NoPermissionOrExportInvoiceNotFound;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            return View(result.Data);
        }
    }
}
