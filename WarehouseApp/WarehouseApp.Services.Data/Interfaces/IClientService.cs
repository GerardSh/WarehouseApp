using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;

namespace WarehouseApp.Services.Data.Interfaces
{
    public interface IClientService
    {
        Task<OperationResult<Client>> GetOrCreateOrUpdateClientAsync(
            string name, string address, string? phone, string? email);
    }
}
