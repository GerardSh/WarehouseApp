using WarehouseApp.Data.Models;

namespace WarehouseApp.Data.Repository.Interfaces
{
    public interface IImportInvoiceDetailRepository : IRepository<ImportInvoiceDetail, Guid>
    {
        Task<List<ImportInvoiceDetail>> GetAvailableProductsByWarehouseIdAsync(Guid warehouseId);
    }
}
