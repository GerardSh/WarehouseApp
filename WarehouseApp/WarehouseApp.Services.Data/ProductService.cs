using Microsoft.EntityFrameworkCore;

using WarehouseApp.Data;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.Product;

public class ProductService : BaseService, IProductService
{
    private readonly WarehouseDbContext context;

    public ProductService(WarehouseDbContext context)
    {
        this.context = context;
    }

    public async Task<OperationResult<Product>> GetOrCreateOrUpdateProductAsync(string name, string? description, Guid categoryId)
    {
        var product = await context.Products
                        .FirstOrDefaultAsync(p => p.Name == name && p.CategoryId == categoryId);

        if (product == null)
        {
            product = new Product
            {
                Name = name,
                Description = description,
                CategoryId = categoryId
            };

            context.Products.Add(product);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(description) && product.Description != description)
            {
                product.Description = description;
            }

            if (product.Name != name)
            {
                product.Name = name;
            }

            context.Products.Update(product);
        }

        return OperationResult<Product>.Ok(product);
    }
}
