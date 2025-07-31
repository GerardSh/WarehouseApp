using Microsoft.EntityFrameworkCore;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository;
using WarehouseApp.Data.Repository.Interfaces;

namespace WarehouseApp.Data.Repositories
{
    public class ImportInvoiceRepository : BaseRepository<ImportInvoice, Guid>, IImportInvoiceRepository
    {
        public ImportInvoiceRepository(WarehouseDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<ImportInvoice?> GetInvoiceWithDetailsAsync(Guid invoiceId, Guid warehouseId)
        {
            return await dbSet
                .AsNoTracking()
                .Include(i => i.Supplier)
                .Include(i => i.ImportInvoicesDetails)
                    .ThenInclude(d => d.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.WarehouseId == warehouseId);
        }
    }
}
