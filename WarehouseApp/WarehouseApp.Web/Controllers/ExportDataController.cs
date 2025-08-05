using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using WarehouseApp.Services.Data.Interfaces;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;

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
                logger.LogInformation("No invoices found for warehouse: {WarehouseId}", warehouseId);
                return Ok(Enumerable.Empty<string>());
            }

            logger.LogInformation("Fetched available invoice numbers.");
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

            var invoiceNumbersResult = await exportDataService
                .GetAvailableProductsForInvoiceAsync(warehouseId, userGuid, invoiceNumber);

            if (!invoiceNumbersResult.Success)
            {
                logger.LogWarning("Failed to get products for invoice {InvoiceNumber}: {Error}", invoiceNumber, invoiceNumbersResult.ErrorMessage);
                return BadRequest(new { error = invoiceNumbersResult.ErrorMessage });
            }

            logger.LogInformation($"Fetched products for invoice: {invoiceNumber}");
            return Ok(invoiceNumbersResult.Data);
        }
    }
}
