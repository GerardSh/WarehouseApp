using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.ImportInvoice;
using WarehouseApp.Common.OutputMessages;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;

namespace WarehouseApp.Web.Controllers
{
    [Authorize]
    [Route("Warehouse/{warehouseId:guid}/{controller}/{action=Index}/{id:guid?}")]
    public class ImportInvoiceController : BaseController<ImportInvoiceController>
    {
        private readonly IImportInvoiceService importInvoiceService;

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

            var model = new CreateEditImportInvoiceInputModel()
            {
                Date = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEditImportInvoiceInputModel inputModel)
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
                if (result.ErrorMessage == DuplicateInvoice || result.ErrorMessage == ErrorMessages.Product.ProductDuplicate)
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                    return View(inputModel);
                }

                logger.LogError(result.ErrorMessage);

                TempData["ErrorMessage"] = result.ErrorMessage ?? CreationFailure;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid warehouseId, Guid id)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            var result = await importInvoiceService.GetImportInvoiceForEditingAsync(warehouseId, id, userGuid);

            if (!result.Success)
            {
                logger.LogError(result.ErrorMessage);

                TempData["ErrorMessage"] = result.ErrorMessage ?? EditingFailure;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            return View("Create", result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Details(CreateEditImportInvoiceInputModel inputModel)
        {
            return View();
        }
    }
}
