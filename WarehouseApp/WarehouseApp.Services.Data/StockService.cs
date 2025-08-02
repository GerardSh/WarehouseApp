using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Services.Data;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.Stock;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

public class StockService : BaseService, IStockService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IImportInvoiceDetailRepository importInvoiceDetailRepo;
    private readonly IExportInvoiceDetailRepository exportInvoiceDetailRepo;
    private readonly IApplicationUserWarehouseRepository appUserWarehouseRepo;

    public StockService(
        UserManager<ApplicationUser> userManager,
        IImportInvoiceDetailRepository importInvoiceDetailRepo,
        IExportInvoiceDetailRepository exportInvoiceDetailRepo,
        IApplicationUserWarehouseRepository appUserWarehouseRepo)
    {
        this.userManager = userManager;
        this.importInvoiceDetailRepo = importInvoiceDetailRepo;
        this.exportInvoiceDetailRepo = exportInvoiceDetailRepo;
        this.appUserWarehouseRepo = appUserWarehouseRepo;
    }

    /// <summary>
    /// Retrieves a filtered and paginated list of product stock information for a warehouse owned by the specified user.
    /// </summary>
    /// <param name="inputModel">
    /// The search filter and pagination model containing warehouse ID, product and category queries, pagination settings,
    /// and flags such as whether to include fully exported products. This model is updated with the filtered products,
    /// total items, total pages, current page, and warehouse name.
    /// </param>
    /// <param name="userId">
    /// The unique identifier of the user requesting the stock information.
    /// </param>
    /// <returns>
    /// An <see cref="OperationResult"/> indicating success if the stock information was retrieved successfully,
    /// or failure with an appropriate error message if the user is not found or the warehouse is inaccessible.
    /// </returns>
    public async Task<OperationResult> GetInvoicesForWarehouseAsync(
        AllProductsSearchFilterViewModel inputModel, Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return OperationResult.Failure(UserNotFound);

        var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(inputModel.WarehouseId, userId);

        if (warehouse == null)
            return OperationResult.Failure(NoPermissionOrWarehouseNotFound);

        inputModel.WarehouseName = warehouse.Name;

        var products = await importInvoiceDetailRepo
            .All()
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

        var groupedProductsQuery = filteredProducts
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
            });

        if (!inputModel.IncludeExportedProducts)
        {
            groupedProductsQuery = groupedProductsQuery
                .Where(p => p.Available > 0);
        }

        var groupedProducts = groupedProductsQuery.ToList();

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
            .OrderByDescending(p => p.Available)
            .ThenBy(p => p.ProductName)
            .Skip(inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage!.Value - 1))
            .Take(inputModel.EntitiesPerPage.Value)
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
        var imported = await importInvoiceDetailRepo
            .All()
            .Where(i => i.Id == importDetailId)
            .Select(i => (int?)i.Quantity)
            .FirstOrDefaultAsync() ?? 0;

        var exported = await exportInvoiceDetailRepo
            .All()
            .Where(e => e.ImportInvoiceDetailId == importDetailId &&
                (excludeExportDetailId == null || e.Id != excludeExportDetailId))
            .SumAsync(e => (int?)e.Quantity) ?? 0;

        var available = imported - exported;

        return OperationResult<int>.Ok(available);
    }
}
