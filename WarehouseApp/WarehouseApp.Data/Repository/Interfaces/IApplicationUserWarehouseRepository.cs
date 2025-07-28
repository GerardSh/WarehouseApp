using WarehouseApp.Data.Models;

namespace WarehouseApp.Data.Repository.Interfaces
{
    public interface IApplicationUserWarehouseRepository : IRepository<ApplicationUserWarehouse, Guid>
    {
        Task<bool> UserHasWarehouseWithNameAsync(Guid userId, string warehouseName);

        Task<bool> UserHasWarehouseWithNameAsync(Guid userId, string warehouseName, Guid excludeWarehouseId);

        Task<IEnumerable<ApplicationUserWarehouse>> GetAllByUserIdAsync(Guid userId);
    }
}
