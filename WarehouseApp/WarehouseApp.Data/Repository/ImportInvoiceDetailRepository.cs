using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace WarehouseApp.Data.Repositories
{
    public class ImportInvoiceDetailRepository : BaseRepository<ImportInvoiceDetail, Guid>, IImportInvoiceDetailRepository
    {
        public ImportInvoiceDetailRepository(WarehouseDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<List<ImportInvoiceDetail>> GetAvailableProductsByWarehouseIdAsync(Guid warehouseId)
        {
            return await dbSet
                .AsNoTracking()
                .Include(iid => iid.Product)
                    .ThenInclude(p => p.Category)
                .Include(iid => iid.ExportInvoicesPerProduct)
                .Where(iid => iid.ImportInvoice.WarehouseId == warehouseId &&
                              iid.Quantity > iid.ExportInvoicesPerProduct.Sum(eip => eip.Quantity))
                .ToListAsync();
        }
    }
}
