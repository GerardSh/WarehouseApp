using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using WarehouseApp.Services.Data.Interfaces;

using static WarehouseApp.Common.OutputMessages.SuccessMessages.ExportData;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;
using WarehouseApp.Common.OutputMessages;

namespace WarehouseApp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/warehouse/{warehouseId}")]
    public class ExportDataController : ControllerBase
    {
        private readonly ILogger<ExportDataController> logger;
        private readonly IExportDataService exportDataService;

        public ExportDataController(
            ILogger<ExportDataController> logger,
            IExportDataService exportDataService)
        {
            this.logger = logger;
            this.exportDataService = exportDataService;
        }

        [HttpGet("invoices")]
        public async Task<IActionResult> GetAvailableInvoiceNumbers(Guid warehouseId)
        {
            string? userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid userGuid = Guid.Empty;

            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out userGuid))
            {
                logger.LogWarning(UnauthorizedAccess);
                return Forbid();
            }

            var invoiceNumbersResult = await exportDataService
                .GetAvailableInvoiceNumbersAsync(warehouseId, userGuid);

            if (!invoiceNumbersResult.Success)
            {
                if (invoiceNumbersResult.ErrorMessage == UserNotFound
                    || invoiceNumbersResult.ErrorMessage == NoPermissionOrWarehouseNotFound)
                {
                    logger.LogInformation(invoiceNumbersResult.ErrorMessage);
                    return NotFound(invoiceNumbersResult.ErrorMessage);
                }

                if (invoiceNumbersResult.ErrorMessage == NoInvoicesFound)
                {
                    logger.LogInformation(invoiceNumbersResult.ErrorMessage);
                    return Ok(Enumerable.Empty<string>());
                }

                return StatusCode(500, invoiceNumbersResult.ErrorMessage);
            }

            logger.LogInformation(FetchedInvoices);
            return Ok(invoiceNumbersResult.Data);
        }

        [HttpGet("invoices/{invoiceNumber}/products")]
        public async Task<IActionResult> GetProductsForInvoice(Guid warehouseId, string invoiceNumber)
        {
            string? userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid userGuid = Guid.Empty;

            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out userGuid))
            {
                logger.LogWarning(UnauthorizedAccess);
                return Forbid();
            }

            var productsResult = await exportDataService
                .GetAvailableProductsForInvoiceAsync(warehouseId, userGuid, invoiceNumber);

            if (!productsResult.Success)
            {
                if (productsResult.ErrorMessage == UserNotFound
                    || productsResult.ErrorMessage == NoPermissionOrWarehouseNotFound
                    || productsResult.ErrorMessage == NoPermissionOrImportInvoiceNotFound)
                {
                    logger.LogInformation(productsResult.ErrorMessage);
                    return NotFound(new { error = productsResult.ErrorMessage });
                }

                if (productsResult.ErrorMessage == ErrorMessages.ImportInvoice.GetModelFailure
                    || productsResult.ErrorMessage == ErrorMessages.ExportData.RetrievingProductsFailure)
                {
                    return StatusCode(500, new { error = productsResult.ErrorMessage });
                }

                logger.LogWarning(ErrorMessages.ExportData.RetrievingProductsFailure);
                return BadRequest(new { error = productsResult.ErrorMessage });
            }

            logger.LogInformation($"{FetchedProducts} {invoiceNumber}");
            return Ok(productsResult.Data);
        }
    }
}
