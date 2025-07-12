using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.Shared;
using WarehouseApp.Web.ViewModels.Warehouse;

namespace WarehouseApp.Services.Data.Interfaces
{
    public interface IWarehouseService
    {
        Task<OperationResult> GetWarehousesForUserAsync(AllWarehousesSearchFilterViewModel inputModel, Guid userId);

        Task<OperationResult> CreateWarehouseAsync(CreateWarehouseInputModel inputModel, Guid userId);

        Task<OperationResult<WarehouseDetailsViewModel>> GetWarehouseDetailsAsync(Guid warehouseId, Guid userId);

        Task<OperationResult<EditWarehouseInputModel>> GetWarehouseForEditingAsync(Guid warehouseId, Guid userId);

        Task<OperationResult> UpdateWarehouseAsync(EditWarehouseInputModel model, Guid userId);

        Task<OperationResult> DeleteWarehouseAsync(Guid warehouseId, Guid userId);
    }
}
