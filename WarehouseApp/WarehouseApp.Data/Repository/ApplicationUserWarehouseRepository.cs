using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace WarehouseApp.Data.Repositories
{
    public class ApplicationUserWarehouseRepository : BaseRepository<ApplicationUserWarehouse, Guid>, IApplicationUserWarehouseRepository
    {
        public ApplicationUserWarehouseRepository(WarehouseDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<bool> UserHasWarehouseWithNameAsync(Guid userId, string warehouseName)
        {
            return await dbSet
                .AnyAsync(uw => uw.ApplicationUserId == userId &&
                                uw.Warehouse.Name.ToLower() == warehouseName.ToLower());
        }

        public async Task<bool> UserHasWarehouseWithNameAsync(Guid userId, string warehouseName, Guid excludeWarehouseId)
        {
            return await dbSet
                .AnyAsync(uw => uw.ApplicationUserId == userId &&
                                uw.Warehouse.Name.ToLower() == warehouseName.ToLower() &&
                                uw.WarehouseId != excludeWarehouseId);
        }
    }
}
