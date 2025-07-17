using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Data;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.ImportInvoice;
using WarehouseApp.Common.OutputMessages;

using static WarehouseApp.Common.Constants.ApplicationConstants;
using static WarehouseApp.Common.Constants.EntityConstants.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;
using WarehouseApp.Web.ViewModels.Warehouse;



namespace WarehouseApp.Services.Data
{
    public class ImportInvoiceService : BaseService, IImportInvoiceService
    {
        private readonly WarehouseDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        private readonly IClientService clientService;
        private readonly ICategoryService categoryService;
        private readonly IProductService productService;

        public ImportInvoiceService(
            WarehouseDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            IClientService clientService,
            ICategoryService categoryService,
            IProductService productService)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.clientService = clientService;
            this.categoryService = categoryService;
            this.productService = productService;
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

        public async Task<OperationResult> CreateImportInvoiceAsync(CreateEditImportInvoiceInputModel inputModel, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            var warehouse = await GetWarehouseOwnedByUserAsync(inputModel.WarehouseId, userId);

            if (warehouse == null)
                return OperationResult.Failure(NoPermissionOrWarehouseNotFound);

            bool invoiceExists = await dbContext.ImportInvoices
                .AnyAsync(i => i.InvoiceNumber == inputModel.InvoiceNumber && i.WarehouseId == inputModel.WarehouseId);

            if (invoiceExists)
                return OperationResult.Failure(DuplicateInvoice);

            Client client;

            try
            {
                var clientResult = await clientService.GetOrCreateOrUpdateClientAsync(
                inputModel.SupplierName,
                inputModel.SupplierAddress,
                inputModel.SupplierPhoneNumber,
                inputModel.SupplierEmail);

                client = clientResult.Data!;
            }
            catch
            {
                return OperationResult.Failure(ErrorMessages.Client.CreationFailure);
            }

            var importInvoice = new ImportInvoice
            {
                InvoiceNumber = inputModel.InvoiceNumber,
                Date = inputModel.Date,
                SupplierId = client.Id,
                WarehouseId = inputModel.WarehouseId
            };

            dbContext.ImportInvoices.Add(importInvoice);

            foreach (var detail in inputModel.Products)
            {
                Category category;

                try
                {
                    var categoryResult = await categoryService.GetOrCreateOrUpdateCategoryAsync(
                        detail.CategoryName,
                        detail.CategoryDescription);

                    category = categoryResult.Data!;
                }
                catch
                {
                    return OperationResult.Failure(ErrorMessages.Category.CreationFailure);
                }

                Product product;

                try
                {
                    var productResult = await productService.GetOrCreateOrUpdateProductAsync(
                    detail.ProductName,
                    detail.ProductDescription,
                    category.Id);

                    if (!productResult.Success)
                    {
                        return OperationResult.Failure(productResult.ErrorMessage!);
                    }

                    product = productResult.Data!;
                }
                catch
                {
                    return OperationResult.Failure(ErrorMessages.Product.CreationFailure);
                }

                try
                {
                    var invoiceDetail = new ImportInvoiceDetail
                    {
                        ImportInvoice = importInvoice,
                        ProductId = product.Id,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.Price
                    };

                    dbContext.ImportInvoiceDetails.Add(invoiceDetail);
                }
                catch
                {
                    return OperationResult.Failure(ErrorMessages.ImportInvoiceDetail.CreationFailure);
                }
            }

            try
            {
                await dbContext.SaveChangesAsync();
                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(ErrorMessages.ImportInvoice.CreationFailure);
            }
        }

        public async Task<OperationResult<CreateEditImportInvoiceInputModel>> GetImportInvoiceForEditingAsync(
            Guid warehouseId, Guid invoiceId, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult<CreateEditImportInvoiceInputModel>.Failure(UserNotFound);

            var warehouse = await GetWarehouseOwnedByUserAsync(warehouseId, userId);

            if (warehouse == null)
                return OperationResult<CreateEditImportInvoiceInputModel>.Failure(NoPermissionOrWarehouseNotFound);

            var invoice = await dbContext.ImportInvoices
                .Include(i => i.Supplier)
                .Include(i => i.ImportInvoicesDetails)
                    .ThenInclude(d => d.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.WarehouseId == warehouseId);

            if (invoice == null)
                return OperationResult<CreateEditImportInvoiceInputModel>.Failure(InvoiceNotFoundOrAccessDeniced);

            var model = new CreateEditImportInvoiceInputModel
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                Date = invoice.Date,
                WarehouseId = invoice.WarehouseId,
                SupplierName = invoice.Supplier.Name,
                SupplierAddress = invoice.Supplier.Address,
                SupplierEmail = invoice.Supplier.Email,
                SupplierPhoneNumber = invoice.Supplier.PhoneNumber,
                Products = invoice.ImportInvoicesDetails.Select(d => new ImportInvoiceDetailInputModel
                {
                    ProductName = d.Product.Name,
                    ProductDescription = d.Product.Description,
                    CategoryName = d.Product.Category.Name,
                    CategoryDescription = d.Product.Category.Description,
                    Quantity = d.Quantity,
                    Price = d.UnitPrice
                }).ToList()
            };

            return OperationResult<CreateEditImportInvoiceInputModel>.Ok(model);
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
