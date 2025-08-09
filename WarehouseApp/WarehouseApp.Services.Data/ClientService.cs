using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Services.Data;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;

public class ClientService : BaseService, IClientService
{
    private readonly IClientRepository clientRepo;

    public ClientService(IClientRepository clientRepo, ILogger<ClientService> logger)
            : base(logger)
    {
        this.clientRepo = clientRepo;
    }

    /// <summary>
    /// Retrieves an existing client by name or creates a new one if it doesn't exist.
    /// If the client exists, updates its address, phone number, and email only if new values are provided.
    /// </summary>
    /// <param name="name">
    /// The name of the client to retrieve or create. This field is used as a unique identifier.
    /// </param>
    /// <param name="address">
    /// The address to assign to the client (new or existing).
    /// </param>
    /// <param name="phone">
    /// An optional phone number to set or update for the client.
    /// </param>
    /// <param name="email">
    /// An optional email address to set or update for the client.
    /// </param>
    /// <returns>
    /// An <see cref="OperationResult{Client}"/> containing the created or updated client,
    /// or an error if the operation fails.
    /// </returns>
    public async Task<OperationResult<Client>> GetOrCreateOrUpdateClientAsync(
        string name, string address, string? phone, string? email)
    {
        var client = await clientRepo
            .AllTracked()
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

        if (client == null)
        {
            client = new Client
            {
                Name = name,
                Address = address,
                PhoneNumber = phone,
                Email = email
            };

            await clientRepo.AddAsync(client);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(address))
            {
                client.Address = address;
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                client.PhoneNumber = phone;
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                client.Email = email;
            }
        }

        client.Name = name;

        await clientRepo.SaveChangesAsync();

        return OperationResult<Client>.Ok(client);
    }
}
