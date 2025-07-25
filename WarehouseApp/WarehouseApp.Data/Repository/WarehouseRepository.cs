using Microsoft.EntityFrameworkCore;

using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository;

namespace WarehouseApp.Data.Repositories
{
    public class WarehouseRepository : BaseRepository<Warehouse, Guid>, IWarehouseRepository
    {
        public WarehouseRepository(WarehouseDbContext dbContext)
            : base(dbContext)
        {
        }

        public IQueryable<Warehouse> GetUserWarehousesQueryAsync(Guid userId)
        {
            return dbSet
                .AsNoTracking()
                .Where(w => w.WarehouseUsers.Any(uw => uw.ApplicationUserId == userId));
        }

        public async Task<Warehouse?> GetWarehouseDetailsByIdAsync(Guid warehouseId)
        {
            return await dbSet
                .AsNoTracking()
                .Include(w => w.WarehouseUsers)
                .Include(w => w.CreatedByUser)
                .Include(w => w.ImportInvoices)
                .Include(w => w.ExportInvoices)
                .FirstOrDefaultAsync(w => w.Id == warehouseId);
        }
    }
}
