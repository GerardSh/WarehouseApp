using WarehouseApp.Data.Models;

namespace WarehouseApp.Data.Repository.Interfaces
{
    public interface IImportInvoiceRepository : IRepository<ImportInvoice, Guid>
    {
        Task<ImportInvoice?> GetInvoiceWithDetailsAsync(Guid invoiceId, Guid warehouseId);
    }
}