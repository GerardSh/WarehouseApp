using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.ImportInvoice;
using WarehouseApp.Web.ViewModels.Warehouse;

namespace WarehouseApp.Services.Data.Interfaces
{
    public interface IImportInvoiceService
    {
        Task<OperationResult> GetInvoicesForWarehouseAsync(
            AllImportInvoicesSearchFilterViewModel inputModel, Guid userId);

        Task<OperationResult> CreateImportInvoiceAsync(CreateImportInvoiceInputModel inputModel, Guid userId);

        Task<OperationResult<EditImportInvoiceInputModel>> GetImportInvoiceForEditingAsync(Guid warehouseId, Guid invoiceId, Guid userId);

        Task<OperationResult> UpdateImportInvoiceAsync(EditImportInvoiceInputModel inputModel, Guid userId);
    }
}
