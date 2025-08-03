using Microsoft.EntityFrameworkCore;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Services.Data;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;

public class ProductService : BaseService, IProductService
{
    private readonly IProductRepository productRepo;

    public ProductService(IProductRepository productRepo)
    {
        this.productRepo = productRepo;
    }

    /// <summary>
    /// Retrieves an existing product by name and category, or creates a new one if it doesn't exist.
    /// If the product exists and the provided description is different and not null or whitespace, it updates the description.
    /// Also updates the product name if it's different.
    /// </summary>
    /// <param name="name">
    /// The name of the product to retrieve or create.
    /// </param>
    /// <param name="description">
    /// An optional description to set or update for the product.
    /// </param>
    /// <param name="categoryId">
    /// The unique identifier of the category to which the product belongs.
    /// </param>
    /// <returns>
    /// An <see cref="OperationResult{Product}"/> containing the created or updated product,
    /// or an error if the operation fails.
    /// </returns>
    public async Task<OperationResult<Product>> GetOrCreateOrUpdateProductAsync(string name, string? description, Guid categoryId)
    {
        var product = await productRepo
            .AllTracked()
            .FirstOrDefaultAsync(p => p.Name == name && p.CategoryId == categoryId);

        if (product == null)
        {
            product = new Product
            {
                Name = name,
                Description = description,
                CategoryId = categoryId
            };

            await productRepo.AddAsync(product);
        }
        else if (!string.IsNullOrWhiteSpace(description) && product.Description != description)
        {
            product.Description = description;
        }

        await productRepo.SaveChangesAsync();

        return OperationResult<Product>.Ok(product);
    }
}
