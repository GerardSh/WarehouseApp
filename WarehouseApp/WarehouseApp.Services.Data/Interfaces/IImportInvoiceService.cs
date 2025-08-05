using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Dtos.ImportInvoices;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.ImportInvoice;

namespace WarehouseApp.Services.Data.Interfaces
{
    public interface IImportInvoiceService
    {
        Task<OperationResult> GetInvoicesForWarehouseAsync(
            AllImportInvoicesSearchFilterViewModel inputModel, Guid userId);

        Task<OperationResult> CreateImportInvoiceAsync(CreateImportInvoiceInputModel inputModel, Guid userId);

        Task<OperationResult<ImportInvoiceDetailsViewModel>> GetImportInvoiceDetailsAsync(Guid warehouseId, Guid invoiceId, Guid userId);

        Task<OperationResult<EditImportInvoiceInputModel>> GetImportInvoiceForEditingAsync(Guid warehouseId, Guid invoiceId, Guid userId);

        Task<OperationResult> UpdateImportInvoiceAsync(EditImportInvoiceInputModel inputModel, Guid userId);

        Task<OperationResult> DeleteImportInvoiceAsync(Guid warehouseId, Guid invoiceId, Guid userId);

        Task<OperationResult<IEnumerable<ImportInvoiceSummaryDto>>> GetInvoicesByWarehouseIdAsync(Guid warehouseId);

        Task<OperationResult<ImportInvoice>> GetInvoiceByNumberAsync(Guid warehouseId, string invoiceNumber);
    }
}
