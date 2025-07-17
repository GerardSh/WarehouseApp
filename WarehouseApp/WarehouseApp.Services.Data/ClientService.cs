using Microsoft.EntityFrameworkCore;

using WarehouseApp.Data;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;

public class ClientService : BaseService, IClientService
{
    private readonly WarehouseDbContext context;

    public ClientService(WarehouseDbContext context)
    {
        this.context = context;
    }

    public async Task<OperationResult<Client>> GetOrCreateOrUpdateClientAsync(
        string name, string address, string? phone, string? email)
    {
        var client = await context.Clients
            .FirstOrDefaultAsync(c => c.Name == name);

        if (client == null)
        {
            client = new Client
            {
                Name = name,
                Address = address,
                PhoneNumber = phone,
                Email = email
            };

            context.Clients.Add(client);
        }
        else
        {
            client.Address = address ?? client.Address;
            client.PhoneNumber = phone ?? client.PhoneNumber;
            client.Email = email ?? client.Email;

            context.Clients.Update(client);
        }

        return OperationResult<Client>.Ok(client);
    }
}