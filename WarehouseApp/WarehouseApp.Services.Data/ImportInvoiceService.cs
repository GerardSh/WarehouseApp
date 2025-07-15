using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Data;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.Constants.ApplicationConstants;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.Constants.EntityConstants.Warehouse;
using WarehouseApp.Web.ViewModels.ImportInvoice;

namespace WarehouseApp.Services.Data
{
    public class ImportInvoiceService : BaseService, IImportInvoiceService
    {
        private readonly WarehouseDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public ImportInvoiceService(WarehouseDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<OperationResult> GetInvoicesForWarehouseAsync(
            AllImportInvoicesSearchFilterViewModel inputModel, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            var warehouse = await GetWarehouseOwnedByUserAsync(inputModel.WarehouseId, userId);

            if (warehouse == null)
                return OperationResult.Failure(NoPermissionOrWarehouseNotFound);

            inputModel.WarehouseName = warehouse.Name;

            IQueryable<ImportInvoice> allInvoicesQuery = dbContext.ImportInvoices
                .AsNoTracking()
                .Where(ii => ii.WarehouseId == inputModel.WarehouseId);

            inputModel.TotalInvoices = await allInvoicesQuery.CountAsync();

            if (!string.IsNullOrWhiteSpace(inputModel.SearchQuery))
            {
                allInvoicesQuery = allInvoicesQuery
                    .Where(ii => ii.InvoiceNumber.ToLower().Contains(inputModel.SearchQuery.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(inputModel.SupplierName))
            {
                allInvoicesQuery = allInvoicesQuery
                    .Where(ii => ii.Supplier.Name.ToLower().Contains(inputModel.SupplierName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(inputModel.YearFilter))
            {
                Match rangeMatch = Regex.Match(inputModel.YearFilter, YearFilterRangeRegex);
                if (rangeMatch.Success)
                {
                    int startYear = int.Parse(rangeMatch.Groups[1].Value);
                    int endYear = int.Parse(rangeMatch.Groups[2].Value);

                    allInvoicesQuery = allInvoicesQuery
                        .Where(ii => ii.Date.Year >= startYear &&
                                    ii.Date.Year <= endYear);
                }
                else
                {
                    bool isValidNumber = int.TryParse(inputModel.YearFilter, out int year);
                    if (isValidNumber)
                    {
                        allInvoicesQuery = allInvoicesQuery
                            .Where(ii => ii.Date.Year == year);
                    }
                }
            }

            allInvoicesQuery = allInvoicesQuery.OrderBy(ii => ii.Date);

            inputModel.TotalItemsBeforePagination = await allInvoicesQuery.CountAsync();

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

            allInvoicesQuery = allInvoicesQuery
                .Skip(inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage!.Value - 1))
                .Take(inputModel.EntitiesPerPage.Value);

            var importInvoices = await allInvoicesQuery.Select(ii => new ImportInvoiceSummaryViewModel
            {
                Id = ii.Id.ToString(),
                InvoiceNumber = ii.InvoiceNumber,
                SupplierName = ii.Supplier.Name,
                Date = ii.Date.ToString(DateFormat),
                ProductCount = ii.ImportInvoicesDetails.Count.ToString(),
            })
                .ToArrayAsync();

            inputModel.Invoices = importInvoices;

            return OperationResult.Ok();
        }

        public Task<OperationResult> CreateImportInvoiceAsync(CreateImportInvoiceInputModel inputModel, Guid userId)
        {
            throw new NotImplementedException();
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
}
