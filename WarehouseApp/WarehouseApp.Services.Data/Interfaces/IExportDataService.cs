using WarehouseApp.Services.Data.Dtos.ImportInvoices;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.Shared;
using WarehouseApp.Web.ViewModels.Warehouse;

namespace WarehouseApp.Services.Data.Interfaces
{
    public interface IExportDataService
    {
        Task<OperationResult<IEnumerable<string>>> GetAvailableInvoiceNumbersAsync(Guid warehouseId, Guid userId);

        Task<OperationResult<IEnumerable<ProductDto>>> GetAvailableProductsForInvoiceAsync(Guid warehouseId, Guid userId, string invoiceNumber);
    }
}
