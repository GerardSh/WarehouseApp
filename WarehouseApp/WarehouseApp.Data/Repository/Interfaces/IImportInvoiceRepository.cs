using WarehouseApp.Data.Models;

namespace WarehouseApp.Data.Repository.Interfaces
{
    public interface IImportInvoiceRepository : IRepository<ImportInvoice, Guid>
    {
        IQueryable<ImportInvoice> GetAllForWarehouse(Guid warehouseId);

        Task<ImportInvoice?> GetInvoiceWithDetailsAsync(Guid invoiceId, Guid warehouseId);
    }
}