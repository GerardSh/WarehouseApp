using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;

namespace WarehouseApp.Services.Data.Interfaces
{
    public interface ICategoryService
    {
        Task<OperationResult<Category>> GetOrCreateOrUpdateCategoryAsync(string name, string? description);
    }
}
