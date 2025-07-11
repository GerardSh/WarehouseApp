using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.Shared;
using WarehouseApp.Web.ViewModels.Warehouse;

namespace WarehouseApp.Services.Data.Interfaces
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseCardViewModel>> GetWarehousesForUserAsync(AllWarehousesSearchFilterViewModel inputModel, Guid userId);

        Task<OperationResult<Guid>> CreateWarehouseAsync(CreateWarehouseInputModel inputModel, Guid userId);
    }
}
