using Microsoft.EntityFrameworkCore;

using WarehouseApp.Data;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;

public class CategoryService : BaseService, ICategoryService
{
    private readonly WarehouseDbContext context;

    public CategoryService(WarehouseDbContext context)
    {
        this.context = context;
    }

    public async Task<OperationResult<Category>> GetOrCreateOrUpdateCategoryAsync(string name, string? description)
    {
        var localCategory = context.ChangeTracker.Entries<Category>()
            .Select(e => e.Entity)
            .FirstOrDefault(c => c.Name == name);

        if (localCategory != null)
        {
            return OperationResult<Category>.Ok(localCategory);
        }

        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Name == name);

        if (category == null)
        {
            category = new Category
            {

                Name = name,
                Description = description
            };

            context.Categories.Add(category);
        }
        else
        {
            category.Description = description ?? category.Description;
            context.Categories.Update(category);
        }

        return OperationResult<Category>.Ok(category);
    }
}