using WarehouseApp.Data.Models;

namespace WarehouseApp.Data.Repository.Interfaces
{
    public interface IWarehouseRepository : IRepository<Warehouse, Guid>
    {
        IQueryable<Warehouse> GetUserWarehousesQueryAsync(Guid userId);

        Task<Warehouse?> GetWarehouseDetailsByIdAsync(Guid warehouseId);
    }
}
