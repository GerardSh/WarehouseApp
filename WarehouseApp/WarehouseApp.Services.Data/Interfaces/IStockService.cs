using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.Stock;

namespace WarehouseApp.Services.Data.Interfaces
{
    public interface IStockService
    {
        Task<OperationResult<int>> GetAvailableQuantityAsync(Guid importDetailId, Guid? excludeExportDetailId = null);

        Task<OperationResult> GetInvoicesForWarehouseAsync(AllProductsSearchFilterViewModel importDetailId, Guid userId);
    }
}
