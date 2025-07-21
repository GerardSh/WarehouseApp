using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Data;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;

using static WarehouseApp.Common.Constants.ApplicationConstants;
using static WarehouseApp.Common.Constants.EntityConstants.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoice;
using WarehouseApp.Web.ViewModels.ExportInvoice;
using WarehouseApp.Common.OutputMessages;
using WarehouseApp.Web.ViewModels.ImportInvoice;

namespace WarehouseApp.Services.Data
{
    public class ExportInvoiceService : BaseService, IExportInvoiceService
    {
        private readonly WarehouseDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        private readonly IClientService clientService;
        private readonly IStockService stockService;

        public ExportInvoiceService(
            WarehouseDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            IClientService clientService,
            IStockService stockService)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.clientService = clientService;
            this.stockService = stockService;
        }

        public async Task<OperationResult> GetInvoicesForWarehouseAsync(
            AllExportInvoicesSearchFilterViewModel inputModel, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            var warehouse = await GetWarehouseOwnedByUserAsync(inputModel.WarehouseId, userId);

            if (warehouse == null)
                return OperationResult.Failure(NoPermissionOrWarehouseNotFound);

            inputModel.WarehouseName = warehouse.Name;

            IQueryable<ExportInvoice> allInvoicesQuery = dbContext.ExportInvoices
                .AsNoTracking()
                .Where(ii => ii.WarehouseId == inputModel.WarehouseId);

            inputModel.TotalInvoices = await allInvoicesQuery.CountAsync();

            if (!string.IsNullOrWhiteSpace(inputModel.SearchQuery))
            {
                allInvoicesQuery = allInvoicesQuery
                    .Where(ei => ei.InvoiceNumber.ToLower().Contains(inputModel.SearchQuery.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(inputModel.ClientName))
            {
                allInvoicesQuery = allInvoicesQuery
                    .Where(ei => ei.Client.Name.ToLower().Contains(inputModel.ClientName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(inputModel.YearFilter))
            {
                Match rangeMatch = Regex.Match(inputModel.YearFilter, YearFilterRangeRegex);
                if (rangeMatch.Success)
                {
                    int startYear = int.Parse(rangeMatch.Groups[1].Value);
                    int endYear = int.Parse(rangeMatch.Groups[2].Value);

                    allInvoicesQuery = allInvoicesQuery
                        .Where(ei => ei.Date.Year >= startYear &&
                                    ei.Date.Year <= endYear);
                }
                else
                {
                    bool isValidNumber = int.TryParse(inputModel.YearFilter, out int year);
                    if (isValidNumber)
                    {
                        allInvoicesQuery = allInvoicesQuery
                            .Where(ei => ei.Date.Year == year);
                    }
                }
            }

            allInvoicesQuery = allInvoicesQuery.OrderByDescending(ei => ei.Date);

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

            var exportInvoices = await allInvoicesQuery.Select(ei => new ExportInvoiceSummaryViewModel
            {
                Id = ei.Id.ToString(),
                InvoiceNumber = ei.InvoiceNumber,
                ClientName = ei.Client.Name,
                Date = ei.Date.ToString(DateFormat),
                ExportedProductsCount = ei.ExportInvoicesDetails.Count.ToString(),
            })
                .ToArrayAsync();

            inputModel.Invoices = exportInvoices;

            return OperationResult.Ok();
        }

        public async Task<OperationResult> CreateExportInvoiceAsync(CreateExportInvoiceInputModel inputModel, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            var warehouse = await GetWarehouseOwnedByUserAsync(inputModel.WarehouseId, userId);

            bool invoiceExists = await dbContext.ExportInvoices
                .AnyAsync(i => i.InvoiceNumber == inputModel.InvoiceNumber && i.WarehouseId == inputModel.WarehouseId);

            if (invoiceExists)
                return OperationResult.Failure(DuplicateInvoice);

            if (inputModel.ExportedProducts.Count == 0)
                return OperationResult.Failure(CannotCreateExportInvoiceWithoutExports);

            var duplicateGroups = inputModel.ExportedProducts
                .GroupBy(p => new { p.ImportInvoiceNumber, p.ProductName, p.CategoryName })
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicateGroups.Any())
                return OperationResult.Failure(DuplicateProduct);

            Client client;

            try
            {
                var clientResult = await clientService.GetOrCreateOrUpdateClientAsync(
                    inputModel.ClientName,
                    inputModel.ClientAddress,
                    inputModel.ClientPhoneNumber,
                    inputModel.ClientEmail);

                client = clientResult.Data!;
            }
            catch
            {
                return OperationResult.Failure(ErrorMessages.Client.CreationFailure);
            }

            var exportInvoice = new ExportInvoice
            {
                InvoiceNumber = inputModel.InvoiceNumber,
                Date = inputModel.Date,
                ClientId = client.Id,
                WarehouseId = inputModel.WarehouseId
            };

            dbContext.ExportInvoices.Add(exportInvoice);

            foreach (var detail in inputModel.ExportedProducts)
            {
                var importInvoice = await dbContext.ImportInvoices
                    .Include(ii => ii.ImportInvoicesDetails)
                    .ThenInclude(iid => iid.Product)
                    .ThenInclude(p => p.Category)
                    .FirstOrDefaultAsync(ii =>
                        ii.InvoiceNumber == detail.ImportInvoiceNumber &&
                        ii.WarehouseId == inputModel.WarehouseId);

                if (importInvoice == null)
                {
                    return OperationResult.Failure(NoPermissionOrImportInvoiceNotFound);
                }

                var importDetail = importInvoice.ImportInvoicesDetails.FirstOrDefault(iid =>
                    iid.Product.Name == detail.ProductName &&
                    iid.Product.Category.Name == detail.CategoryName);

                if (importDetail == null)
                {
                    return OperationResult.Failure(ProductNotFoundInImportInvoice);
                }

                if (importInvoice.Date > inputModel.Date)
                {
                    return OperationResult.Failure(CannotExportBeforeImportDate);
                }

                var stockResult = await stockService.GetAvailableQuantityAsync(importDetail.Id);

                var availableQuantity = stockResult.Data;

                if (availableQuantity - detail.Quantity < 0)
                {
                    return OperationResult.Failure(InsufficientStock);
                }

                try
                {
                    var exportDetail = new ExportInvoiceDetail
                    {
                        ExportInvoice = exportInvoice,
                        ImportInvoiceDetailId = importDetail.Id,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice ?? importDetail.UnitPrice
                    };

                    dbContext.ExportInvoiceDetails.Add(exportDetail);
                }
                catch
                {
                    return OperationResult.Failure(ErrorMessages.ExportInvoiceDetail.CreationFailure);
                }
            }

            try
            {
                await dbContext.SaveChangesAsync();
                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(ErrorMessages.ExportInvoice.CreationFailure);
            }
        }

        public async Task<OperationResult<ExportInvoiceDetailsViewModel>> GetExportInvoiceDetailsAsync(
           Guid warehouseId, Guid invoiceId, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult<ExportInvoiceDetailsViewModel>.Failure(UserNotFound);

            var warehouse = await GetWarehouseOwnedByUserAsync(warehouseId, userId);

            if (warehouse == null)
                return OperationResult<ExportInvoiceDetailsViewModel>.Failure(NoPermissionOrWarehouseNotFound);

            var exportInvoice = await dbContext.ExportInvoices
                .AsNoTracking()
                .Include(ei => ei.Client)
                .Include(ei => ei.ExportInvoicesDetails)
                    .ThenInclude(eid => eid.ImportInvoiceDetail)
                        .ThenInclude(iid => iid.Product)
                            .ThenInclude(p => p.Category)
                .Include(ei => ei.ExportInvoicesDetails)
                    .ThenInclude(eid => eid.ImportInvoiceDetail)
                        .ThenInclude(iid => iid.ImportInvoice)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.WarehouseId == warehouseId);

            if (exportInvoice == null)
                return OperationResult<ExportInvoiceDetailsViewModel>.Failure(NoPermissionOrExportInvoiceNotFound);

            var viewModel = new ExportInvoiceDetailsViewModel
            {
                Id = exportInvoice.Id,
                InvoiceNumber = exportInvoice.InvoiceNumber,
                Date = exportInvoice.Date,
                ClientName = exportInvoice.Client.Name,
                ClientAddress = exportInvoice.Client.Address,
                ClientPhone = exportInvoice.Client.PhoneNumber ?? "N/A",
                ClientEmail = exportInvoice.Client.Email ?? "N/A",
                ExportedProducts = exportInvoice.ExportInvoicesDetails.Select(eid => new ExportInvoiceDetailDetailsViewModel()
                {
                    ImportInvoiceNumber = eid.ImportInvoiceDetail.ImportInvoice.InvoiceNumber,
                    ProductName = eid.ImportInvoiceDetail.Product.Name,
                    ProductDescription = eid.ImportInvoiceDetail.Product.Description,
                    CategoryName = eid.ImportInvoiceDetail.Product.Category.Name,
                    CategoryDescription = eid.ImportInvoiceDetail.Product.Category.Description ?? "N/A",
                    Quantity = eid.Quantity,
                    UnitPrice = eid.UnitPrice ?? 0,
                    Total = eid.Quantity * eid.UnitPrice ?? 0
                }).ToList()
            };

            return OperationResult<ExportInvoiceDetailsViewModel>.Ok(viewModel);
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
