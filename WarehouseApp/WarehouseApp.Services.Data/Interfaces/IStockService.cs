using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;

namespace WarehouseApp.Services.Data.Interfaces
{
    public interface IStockService
    {
        Task<OperationResult<int>> GetAvailableQuantityAsync(Guid importDetailId, Guid? excludeExportDetailId = null);
    }
}
