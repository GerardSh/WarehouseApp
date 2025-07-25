using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.Stock;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoiceDetail;
using static WarehouseApp.Common.OutputMessages.SuccessMessages.ExportInvoice;
using WarehouseApp.Services.Data;

namespace WarehouseApp.Web.Controllers
{
    [Authorize]
    [Route("Warehouse/{warehouseId:guid}/{controller}/{action=Index}/{id:guid?}")]
    public class StockController : BaseController<StockController>
    {
        private readonly IStockService stockService;

        private readonly HashSet<string> knownClientErrors = new HashSet<string>
        {

        };

        public StockController(IStockService stockService, ILogger<StockController> logger)
            : base(logger)
        {
            this.stockService = stockService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(AllProductsSearchFilterViewModel inputModel)
        {
            string? userId = GetUserId();
            Guid userGuid = Guid.Empty;

            IActionResult? validationResult = ValidateUserIdOrRedirect(userId, ref userGuid);
            if (validationResult != null)
                return validationResult;

            OperationResult result =
                await stockService.GetInvoicesForWarehouseAsync(inputModel, userGuid);

            if (!result.Success)
            {
                logger.LogError(result.ErrorMessage);

                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            TempData["SearchQuery"] = inputModel.ProductQuery ?? string.Empty;
            TempData["ClientName"] = inputModel.CategoryQuery ?? string.Empty;
            TempData["EntitiesPerPage"] = inputModel.EntitiesPerPage;
            TempData["CurrentPage"] = inputModel.CurrentPage;

            return View(inputModel);
        }

        public async Task<IActionResult> InvoiceDetails(Guid warehouseId)
        {
            await Task.CompletedTask;
            return View("_UnderConstruction");
        }
    }
}
