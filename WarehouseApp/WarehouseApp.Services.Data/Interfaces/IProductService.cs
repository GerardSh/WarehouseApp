using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;

namespace WarehouseApp.Services.Data.Interfaces
{
    public interface IProductService
    {
        Task<OperationResult<Product>> GetOrCreateOrUpdateProductAsync(string name, string? description, Guid categoryId);
    }
}
