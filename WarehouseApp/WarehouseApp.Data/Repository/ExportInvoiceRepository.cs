using Microsoft.EntityFrameworkCore;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository;
using WarehouseApp.Data.Repository.Interfaces;

namespace WarehouseApp.Data.Repositories
{
    public class ExportInvoiceRepository : BaseRepository<ExportInvoice, Guid>, IExportInvoiceRepository
    {
        public ExportInvoiceRepository(WarehouseDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<ExportInvoice?> GetExportInvoiceWithDetailsAsync(Guid invoiceId, Guid warehouseId)
        {
            return await dbSet
                .AsNoTracking()
                .Include(ei => ei.Client)
                .Include(ei => ei.ExportInvoicesDetails)
                    .ThenInclude(eid => eid.ImportInvoiceDetail)
                        .ThenInclude(iid => iid.Product)
                            .ThenInclude(p => p.Category)
                .Include(ei => ei.ExportInvoicesDetails)
                    .ThenInclude(eid => eid.ImportInvoiceDetail)
                        .ThenInclude(iid => iid.ImportInvoice)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.WarehouseId == warehouseId);
        }
    }
}
