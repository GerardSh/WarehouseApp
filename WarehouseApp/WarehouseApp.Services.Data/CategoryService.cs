using Microsoft.EntityFrameworkCore;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Services.Data;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;

public class CategoryService : BaseService, ICategoryService
{
    private readonly ICategoryRepository categoryRepo;

    public CategoryService(ICategoryRepository categoryRepo)
    {
        this.categoryRepo = categoryRepo;
    }

    /// <summary>
    /// Retrieves an existing category by name from the local change tracker or database.
    /// If no matching category is found, creates a new one with the provided name and optional description.
    /// Updates the description if it differs from the existing one.
    /// Saves changes to the database and returns the resulting category wrapped in an OperationResult.
    /// </summary>
    /// <param name="name">The name of the category to find or create.</param>
    /// <param name="description">An optional description for the category.</param>
    /// <returns>An OperationResult containing the found or newly created category.</returns>
    public async Task<OperationResult<Category>> GetOrCreateOrUpdateCategoryAsync(string name, string? description)
    {
        var localCategory = categoryRepo.GetTrackedLocal(c => c.Name == name);

        if (localCategory != null)
        {
            return OperationResult<Category>.Ok(localCategory);
        }

        var category = await categoryRepo
            .AllTracked()
            .FirstOrDefaultAsync(c => c.Name == name);

        if (category == null)
        {
            category = new Category
            {
                Name = name,
                Description = description
            };

            await categoryRepo.AddAsync(category);
        }
        else if (!string.IsNullOrWhiteSpace(description) && category.Description != description)
        {
            category.Description = description;
        }

        await categoryRepo.SaveChangesAsync();

        return OperationResult<Category>.Ok(category);
    }
}
