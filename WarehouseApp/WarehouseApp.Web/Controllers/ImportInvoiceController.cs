using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.ImportInvoice;
using WarehouseApp.Web.ViewModels.Warehouse;


//using static WarehouseApp.Common.OutputMessages.ErrorMessages.;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;

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
        public async Task<IActionResult> Index(AllImportInvoicesSearchFilterViewModel inputModel, Guid warehouseId)
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
        public async Task<IActionResult> Create()
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            var model = new CreateImportInvoiceInputModel();

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

           return RedirectToAction("Index");
        }
    }
}
