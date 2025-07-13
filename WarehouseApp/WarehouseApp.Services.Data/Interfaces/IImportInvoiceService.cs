using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.ImportInvoice;

namespace WarehouseApp.Services.Data.Interfaces
{
    public interface IImportInvoiceService
    {
        Task<OperationResult> GetInvoicesForWarehouseAsync(
            AllImportInvoicesSearchFilterViewModel inputModel, Guid warehouseId, Guid userId);
    }
}
