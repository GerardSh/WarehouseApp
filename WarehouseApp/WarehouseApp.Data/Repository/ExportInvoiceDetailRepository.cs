using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository;

namespace WarehouseApp.Data.Repositories
{
    public class ExportInvoiceDetailRepository : BaseRepository<ExportInvoiceDetail, Guid>, IExportInvoiceDetailRepository
    {
        public ExportInvoiceDetailRepository(WarehouseDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
