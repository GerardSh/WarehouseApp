using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Dtos.ImportInvoices;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;

namespace WarehouseApp.Services.Data
{
    public class ExportDataService : BaseService, IExportDataService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IImportInvoiceService importInvoiceService;
        private readonly IStockService stockService;
        private readonly IApplicationUserWarehouseRepository appUserWarehouseRepo;

        public ExportDataService(
            UserManager<ApplicationUser> userManager,
            IImportInvoiceService importInvoiceService,
            IStockService stockService,
            IApplicationUserWarehouseRepository appUserWarehouseRepo,
            ILogger<ExportDataService> logger
            )
            : base(logger)
        {
            this.userManager = userManager;
            this.importInvoiceService = importInvoiceService;
            this.stockService = stockService;
            this.appUserWarehouseRepo = appUserWarehouseRepo;
        }

        /// <summary>
        /// Retrieves a list of import invoice numbers for the specified warehouse
        /// that have at least one product with available stock for export.
        /// In case of an unexpected exception, logs the error and returns a failure result.
        /// </summary>
        /// <param name="warehouseId">The unique identifier of the warehouse.</param>
        /// <param name="userId">The unique identifier of the user requesting the data.</param>
        /// <returns>
        /// An <see cref="OperationResult{T}"/> containing a list of invoice numbers
        /// that have products with available quantity, or an error if the user is not authorized
        /// or the warehouse is invalid.
        /// </returns>
        public async Task<OperationResult<IEnumerable<string>>> GetAvailableInvoiceNumbersAsync(Guid warehouseId, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult<IEnumerable<string>>.Failure(UserNotFound);

                var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(warehouseId, userId);

                if (warehouse == null)
                    return OperationResult<IEnumerable<string>>.Failure(NoPermissionOrWarehouseNotFound);

                var result = await importInvoiceService.GetInvoicesByWarehouseIdAsync(warehouseId);

                if (!result.Success)
                    return OperationResult<IEnumerable<string>>.Failure(result.ErrorMessage!);

                var invoices = result.Data;

                var invoicesToReturn = new List<string>();

                foreach (var invoice in invoices!)
                {
                    foreach (var detail in invoice.ImportDetails)
                    {
                        var stockResult = await stockService.GetAvailableQuantityAsync(detail);

                        if (stockResult.Data > 0)
                        {
                            invoicesToReturn.Add(invoice.InvoiceNumber);
                            break;
                        }
                    }
                }

                return OperationResult<IEnumerable<string>>.Ok(invoicesToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.ExportData.RetrievingInvoicesFailure);
                return OperationResult<IEnumerable<string>>.Failure(ErrorMessages.ExportData.RetrievingInvoicesFailure);
            }
        }

        /// <summary>
        /// Retrieves all products from a specific import invoice that have available stock
        /// for export from the given warehouse.
        /// In case of an unexpected exception, logs the error and returns a failure result.
        /// </summary>
        /// <param name="warehouseId">The unique identifier of the warehouse.</param>
        /// <param name="userId">The unique identifier of the user requesting the data.</param>
        /// <param name="invoiceNumber">The invoice number from which to retrieve available products.</param>
        /// <returns>
        /// An <see cref="OperationResult{T}"/> containing a list of available products with their
        /// name, category, and quantity. Returns an error if the user is unauthorized, the warehouse
        /// does not belong to them, or the invoice is not found.
        /// </returns>
        public async Task<OperationResult<IEnumerable<ProductDto>>> GetAvailableProductsForInvoiceAsync(
            Guid warehouseId,
            Guid userId,
            string invoiceNumber)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult<IEnumerable<ProductDto>>.Failure(UserNotFound);

                var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(warehouseId, userId);

                if (warehouse == null)
                    return OperationResult<IEnumerable<ProductDto>>.Failure(NoPermissionOrWarehouseNotFound);

                var invoiceResult = await importInvoiceService.GetInvoiceByNumberAsync(warehouseId, invoiceNumber);

                if (!invoiceResult.Success)
                    return OperationResult<IEnumerable<ProductDto>>.Failure(invoiceResult.ErrorMessage!);

                var invoice = invoiceResult.Data;

                var productsToReturn = new List<ProductDto>();

                foreach (var detail in invoice!.ImportInvoicesDetails)
                {
                    var stockResult = await stockService.GetAvailableQuantityAsync(detail.Id);

                    if (stockResult.Data > 0)
                    {
                        productsToReturn.Add(new ProductDto
                        {
                            Name = detail.Product.Name,
                            Category = detail.Product.Category.Name,
                            AvailableQuantity = stockResult.Data
                        });
                    }
                }

                return OperationResult<IEnumerable<ProductDto>>.Ok(productsToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.ExportData.RetrievingProductsFailure);
                return OperationResult<IEnumerable<ProductDto>>.Failure(ErrorMessages.ExportData.RetrievingProductsFailure);
            }
        }
    }
}
