using WarehouseApp.Data.Models;

namespace WarehouseApp.Data.Repository.Interfaces
{
    public interface IExportInvoiceRepository : IRepository<ExportInvoice, Guid>
    {
        Task<ExportInvoice?> GetExportInvoiceWithDetailsAsync(Guid invoiceId, Guid warehouseId);
    }
}