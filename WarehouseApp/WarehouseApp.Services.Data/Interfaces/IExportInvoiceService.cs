using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.ExportInvoice;

namespace WarehouseApp.Services.Data.Interfaces
{
    public interface IExportInvoiceService
    {
        Task<OperationResult> GetInvoicesForWarehouseAsync(
            AllExportInvoicesSearchFilterViewModel inputModel, Guid userId);

        Task<OperationResult> CreateExportInvoiceAsync(CreateExportInvoiceInputModel inputModel, Guid userId);

        Task<OperationResult<ExportInvoiceDetailsViewModel>> GetExportInvoiceDetailsAsync(Guid warehouseId, Guid invoiceId, Guid userId);

        Task<OperationResult<EditExportInvoiceInputModel>> GetExportInvoiceForEditingAsync(Guid warehouseId, Guid invoiceId, Guid userId);

        Task<OperationResult> UpdateExportInvoiceAsync(EditExportInvoiceInputModel inputModel, Guid userId);

        Task<OperationResult> DeleteExportInvoiceAsync(Guid warehouseId, Guid invoiceId, Guid userId);
    }
}
