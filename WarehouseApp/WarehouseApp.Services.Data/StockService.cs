using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

using WarehouseApp.Data;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.Stock;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

public class StockService : BaseService, IStockService
{
    private readonly WarehouseDbContext dbContext;
    private readonly UserManager<ApplicationUser> userManager;

    public StockService(
        WarehouseDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
    }

    public async Task<OperationResult> GetInvoicesForWarehouseAsync(
        AllProductsSearchFilterViewModel inputModel, Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return OperationResult.Failure(UserNotFound);

        var warehouse = await GetWarehouseOwnedByUserAsync(inputModel.WarehouseId, userId);

        if (warehouse == null)
            return OperationResult.Failure(NoPermissionOrWarehouseNotFound);

        inputModel.WarehouseName = warehouse.Name;

        var products = await dbContext.ImportInvoiceDetails
            .AsNoTracking()
            .Include(iid => iid.Product)
                .ThenInclude(p => p.Category)
                .Include(iid => iid.ExportInvoicesPerProduct)
            .Where(iid => iid.ImportInvoice.WarehouseId == inputModel.WarehouseId)
            .ToListAsync();

        inputModel.TotalProducts = products
             .GroupBy(iid => new
             {
                 iid.ProductId,
                 ProductName = iid.Product.Name,
                 CategoryName = iid.Product.Category.Name
             })
             .Where(g => g.Sum(iid => iid.Quantity) > g.Sum(iid => iid.ExportInvoicesPerProduct.Sum(e => e.Quantity)))
             .Count();

        var filteredProducts = products
            .Where(iid => string.IsNullOrWhiteSpace(inputModel.ProductQuery) ||
                          iid.Product.Name.ToLower().Contains(inputModel.ProductQuery.ToLower()))
            .Where(iid => string.IsNullOrWhiteSpace(inputModel.CategoryQuery) ||
                          iid.Product.Category.Name.ToLower().Contains(inputModel.CategoryQuery.ToLower()))
            .ToList();

        var groupedProducts = filteredProducts
            .GroupBy(iid => new
            {
                iid.ProductId,
                ProductName = iid.Product.Name,
                CategoryName = iid.Product.Category.Name
            })
            .Select(g => new ProductStockViewModel
            {
                ProductName = g.Key.ProductName,
                CategoryName = g.Key.CategoryName,
                TotalImported = g.Sum(x => x.Quantity),
                TotalExported = g.Sum(x => x.ExportInvoicesPerProduct.Sum(e => e.Quantity))
            })
            .Where(p => p.Available > 0)
            .ToList();

        inputModel.TotalItemsBeforePagination = groupedProducts.Count;

        if (inputModel.EntitiesPerPage <= 0)
        {
            inputModel.EntitiesPerPage = 5;
        }

        if (inputModel.EntitiesPerPage > 100)
        {
            inputModel.EntitiesPerPage = 100;
        }

        inputModel.TotalPages = (int)Math.Ceiling(inputModel.TotalItemsBeforePagination /
                                            (double)inputModel.EntitiesPerPage!.Value);

        if (inputModel.CurrentPage > inputModel.TotalPages)
        {
            inputModel.CurrentPage = inputModel.TotalPages > 0 ? inputModel.TotalPages : 1;
        }

        if (inputModel.CurrentPage <= 0)
        {
            inputModel.CurrentPage = 1;
        }

        groupedProducts = groupedProducts
            .Skip(inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage!.Value - 1))
            .Take(inputModel.EntitiesPerPage.Value)
            .OrderByDescending(p => p.Available)
            .ThenBy(p => p.ProductName)
            .ToList();

        inputModel.Products = groupedProducts;

        return OperationResult.Ok();
    }

    /// <summary>
    /// Returns available stock for the given import detail, optionally excluding one export detail.
    /// </summary>
    /// <param name="importDetailId">ID of the import detail to check.</param>
    /// <param name="excludeExportDetailId">Optional export detail to exclude from calculation.</param>
    /// <returns>Available quantity as an <see cref="OperationResult{T}"/>.</returns>
    public async Task<OperationResult<int>> GetAvailableQuantityAsync(
        Guid importDetailId,
        Guid? excludeExportDetailId = null)
    {
        var imported = await dbContext.ImportInvoiceDetails
            .Where(i => i.Id == importDetailId)
            .Select(i => (int?)i.Quantity)
            .FirstOrDefaultAsync() ?? 0;

        var exported = await dbContext.ExportInvoiceDetails
            .Where(e => e.ImportInvoiceDetailId == importDetailId &&
                (excludeExportDetailId == null || e.Id != excludeExportDetailId))
            .SumAsync(e => (int?)e.Quantity) ?? 0;

        var available = imported - exported;

        return OperationResult<int>.Ok(available);
    }

    private async Task<Warehouse?> GetWarehouseOwnedByUserAsync(Guid warehouseId, Guid userId)
    {
        return await dbContext.UsersWarehouses
                  .Where(uw => uw.WarehouseId == warehouseId && uw.ApplicationUserId == userId)
                  .Select(uw => uw.Warehouse)
                  .AsNoTracking()
                  .FirstOrDefaultAsync();
    }
}
