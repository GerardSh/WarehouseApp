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
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoiceDetail;

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

        public async Task<OperationResult> CreateImportInvoiceAsync(CreateImportInvoiceInputModel inputModel, Guid userId)
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

            if (inputModel.Products.Count == 0)
            {
                return OperationResult.Failure(CannotCreateInvoiceWithoutProducts);
            }

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

        public async Task<OperationResult<EditImportInvoiceInputModel>> GetImportInvoiceForEditingAsync(
            Guid warehouseId, Guid invoiceId, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult<EditImportInvoiceInputModel>.Failure(UserNotFound);

            var warehouse = await GetWarehouseOwnedByUserAsync(warehouseId, userId);

            if (warehouse == null)
                return OperationResult<EditImportInvoiceInputModel>.Failure(NoPermissionOrWarehouseNotFound);

            var invoice = await dbContext.ImportInvoices
                .Include(i => i.Supplier)
                .Include(i => i.ImportInvoicesDetails)
                    .ThenInclude(d => d.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.WarehouseId == warehouseId);

            if (invoice == null)
                return OperationResult<EditImportInvoiceInputModel>.Failure(InvoiceNotFoundOrAccessDenied);

            var model = new EditImportInvoiceInputModel
            {
                InvoiceNumber = invoice.InvoiceNumber,
                Date = invoice.Date,
                WarehouseId = invoice.WarehouseId,
                SupplierName = invoice.Supplier.Name,
                SupplierAddress = invoice.Supplier.Address,
                SupplierEmail = invoice.Supplier.Email,
                SupplierPhoneNumber = invoice.Supplier.PhoneNumber,
                Products = invoice.ImportInvoicesDetails.Select(d => new EditImportInvoiceDetailInputModel
                {
                    ProductName = d.Product.Name,
                    ProductDescription = d.Product.Description,
                    CategoryName = d.Product.Category.Name,
                    CategoryDescription = d.Product.Category.Description,
                    Quantity = d.Quantity,
                    Price = d.UnitPrice
                }).ToList()
            };

            return OperationResult<EditImportInvoiceInputModel>.Ok(model);
        }

        public async Task<OperationResult> UpdateImportInvoiceAsync(EditImportInvoiceInputModel inputModel, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            var warehouse = await GetWarehouseOwnedByUserAsync(inputModel.WarehouseId, userId);

            if (warehouse == null)
                return OperationResult.Failure(NoPermissionOrWarehouseNotFound);

            ImportInvoice? invoice = await dbContext.ImportInvoices
                .FirstOrDefaultAsync(i => i.Id == inputModel.Id && i.WarehouseId == inputModel.WarehouseId);

            if (invoice == null)
                return OperationResult.Failure(InvoiceNotFoundOrAccessDenied);

            bool invoiceExists = await dbContext.ImportInvoices
                .AnyAsync(i => i.InvoiceNumber == inputModel.InvoiceNumber &&
                i.WarehouseId == inputModel.WarehouseId &&
                i.Id != inputModel.Id);

            if (invoiceExists)
                return OperationResult.Failure(DuplicateInvoice);

            if (inputModel.Products.Count == 0)
            {
                return OperationResult.Failure(CannnotDeleteAllProducts);
            }

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

            invoice.InvoiceNumber = inputModel.InvoiceNumber;
            invoice.Date = inputModel.Date;
            invoice.Supplier = client;

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
                    if (detail.Id.HasValue)
                    {
                        var localDetail = dbContext.ImportInvoiceDetails.Local
                            .FirstOrDefault(i => i.Id == detail.Id);

                        var invoiceDetail = localDetail ?? await dbContext.ImportInvoiceDetails
                            .Include(iid => iid.Product)
                            .ThenInclude(p => p.Category)
                            .FirstOrDefaultAsync(iid => iid.Id == detail.Id);

                        if (invoiceDetail == null)
                        {
                            return OperationResult.Failure(ErrorMessages.ImportInvoiceDetail.ProductNotFound);
                        }

                        invoiceDetail.Quantity = detail.Quantity;
                        invoiceDetail.UnitPrice = detail.Price;
                        invoiceDetail.Product = product;
                        invoiceDetail.Product.Category = category;
                    }
                    else
                    {
                        var invoiceDetail = new ImportInvoiceDetail
                        {
                            ImportInvoiceId = invoice.Id,
                            ProductId = product.Id,
                            Quantity = detail.Quantity,
                            UnitPrice = detail.Price
                        };

                        dbContext.ImportInvoiceDetails.Add(invoiceDetail);
                    }

                }
                catch
                {
                    return OperationResult.Failure(ErrorMessages.ImportInvoiceDetail.CreationFailure);
                }
            }

            try
            {
                var existingDetails = await dbContext.ImportInvoiceDetails
                .Where(d => d.ImportInvoiceId == invoice.Id)
                .Include(d => d.Product)
                .ThenInclude(d => d.Category)
                .ToListAsync();

                var inputDetailIds = inputModel.Products
                    .Where(p => p.Id.HasValue)
                    .Select(p => p.Id!.Value)
                    .ToHashSet();

                var detailsToRemove = await dbContext.ImportInvoiceDetails
                    .Where(d => d.ImportInvoiceId == invoice.Id && !inputDetailIds.Contains(d.Id))
                    .Include(d => d.ExportInvoicesPerProduct)
                    .ToListAsync();

                var safeToDelete = detailsToRemove
                    .Where(d => d.ExportInvoicesPerProduct == null || d.ExportInvoicesPerProduct.Count == 0)
                    .ToList();

                if (safeToDelete.Count < detailsToRemove.Count)
                {
                    var undeletableDetails = detailsToRemove
                        .Where(d => d.ExportInvoicesPerProduct != null && d.ExportInvoicesPerProduct.Count > 0)
                        .ToList();

                    var undeletableProductInputs = undeletableDetails.Select(d => new EditImportInvoiceDetailInputModel
                    {
                        Id = d.Id,
                        ProductDescription = d.Product.Description,
                        ProductName = d.Product.Name,
                        Quantity = d.Quantity,
                        Price = d.UnitPrice,
                        CategoryDescription = d.Product.Category.Description,
                        CategoryName = d.Product.Category.Name
                    });

                    var existingProductIds = inputModel.Products.Select(p => p.Id).ToHashSet();

                    foreach (var product in undeletableProductInputs)
                    {
                        if (!existingProductIds.Contains(product.Id))
                        {
                            inputModel.Products.Add(product);
                        }
                    }

                    return OperationResult.Failure(ProductDeletionFailure);
                }

                dbContext.ImportInvoiceDetails.RemoveRange(detailsToRemove);
            }
            catch
            {
                return OperationResult.Failure(ProductDeletionFailure);
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
