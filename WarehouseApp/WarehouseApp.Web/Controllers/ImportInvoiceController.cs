using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.ImportInvoice;

//using static WarehouseApp.Common.OutputMessages.ErrorMessages.;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;

namespace WarehouseApp.Web.Controllers
{
    [Authorize]
    [Route("Warehouse/{warehouseId:guid}/[controller]/[action]/{id:int?}")]
    public class ImportInvoiceController : BaseController<ImportInvoiceController>
    {
        private readonly IImportInvoiceService importInvoiceService;

        public ImportInvoiceController(IImportInvoiceService importInvoiceService, ILogger<ImportInvoiceController> logger)
            : base(logger)
        {
            this.importInvoiceService = importInvoiceService;
        }

        public async Task<IActionResult> Index(AllImportInvoicesSearchFilterViewModel inputModel, Guid warehouseId)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult result =
                await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, warehouseId, userGuid);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("Index", "Warehouse");
            }

            return View(inputModel);
        }
    }
}
