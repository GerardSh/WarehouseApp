using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.ImportInvoice;
using WarehouseApp.Data.Repository.Interfaces;

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
        private readonly IImportInvoiceRepository importInvoiceRepo;
        private readonly IImportInvoiceDetailRepository importInvoiceDetailRepo;
        private readonly IExportInvoiceDetailRepository exportInvoiceDetailRepo;
        private readonly IApplicationUserWarehouseRepository appUserWarehouseRepo;
        private readonly UserManager<ApplicationUser> userManager;

        private readonly IClientService clientService;
        private readonly ICategoryService categoryService;
        private readonly IProductService productService;

        public ImportInvoiceService(
            UserManager<ApplicationUser> userManager,
            IImportInvoiceRepository importInvoiceRepo,
            IImportInvoiceDetailRepository importInvoiceDetailRepo,
            IExportInvoiceDetailRepository exportInvoiceDetailRepo,
            IApplicationUserWarehouseRepository appUserWarehouseRepo,
            IClientService clientService,
            ICategoryService categoryService,
            IProductService productService)
        {
            this.importInvoiceRepo = importInvoiceRepo;
            this.importInvoiceDetailRepo = importInvoiceDetailRepo;
            this.exportInvoiceDetailRepo = exportInvoiceDetailRepo;
            this.appUserWarehouseRepo = appUserWarehouseRepo;
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

            var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(inputModel.WarehouseId, userId);

            if (warehouse == null)
                return OperationResult.Failure(NoPermissionOrWarehouseNotFound);

            inputModel.WarehouseName = warehouse.Name;

            IQueryable<ImportInvoice> allInvoicesQuery = importInvoiceRepo
                .GetAllForWarehouse(inputModel.WarehouseId);

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

            allInvoicesQuery = allInvoicesQuery.OrderByDescending(ii => ii.Date);

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

            var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(inputModel.WarehouseId, userId);

            if (warehouse == null)
                return OperationResult.Failure(NoPermissionOrWarehouseNotFound);

            bool invoiceExists = await importInvoiceRepo
                .ExistsAsync(ii => ii.InvoiceNumber == inputModel.InvoiceNumber && ii.WarehouseId == inputModel.WarehouseId);

            if (invoiceExists)
                return OperationResult.Failure(DuplicateInvoice);

            if (inputModel.Products.Count == 0)
            {
                return OperationResult.Failure(CannotCreateInvoiceWithoutProducts);
            }

            var duplicateProducts = inputModel.Products
                .GroupBy(p => new
                {
                    ProductName = p.ProductName.ToLower(),
                    CategoryName = p.CategoryName.ToLower()
                })
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicateProducts.Any())
                return OperationResult.Failure(ProductDuplicate);

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

            await importInvoiceRepo.AddAsync(importInvoice);

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
                        UnitPrice = detail.UnitPrice
                    };

                    await importInvoiceDetailRepo.AddAsync(invoiceDetail);

                }
                catch
                {
                    return OperationResult.Failure(ErrorMessages.ImportInvoiceDetail.CreationFailure);
                }
            }

            try
            {
                await importInvoiceRepo.SaveChangesAsync();
                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(ErrorMessages.ImportInvoice.CreationFailure);
            }
        }

        public async Task<OperationResult<ImportInvoiceDetailsViewModel>> GetImportInvoiceDetailsAsync(
            Guid warehouseId, Guid invoiceId, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult<ImportInvoiceDetailsViewModel>.Failure(UserNotFound);

            var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(warehouseId, userId);

            if (warehouse == null)
                return OperationResult<ImportInvoiceDetailsViewModel>.Failure(NoPermissionOrWarehouseNotFound);

            var invoice = await importInvoiceRepo.GetInvoiceWithDetailsAsync(invoiceId, warehouseId);

            if (invoice == null)
                return OperationResult<ImportInvoiceDetailsViewModel>.Failure(NoPermissionOrImportInvoiceNotFound);

            var viewModel = new ImportInvoiceDetailsViewModel
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                Date = invoice.Date,
                SupplierName = invoice.Supplier.Name,
                SupplierAddress = invoice.Supplier.Address,
                SupplierPhone = invoice.Supplier.PhoneNumber ?? "N/A",
                SupplierEmail = invoice.Supplier.Email ?? "N/A",
                Products = invoice.ImportInvoicesDetails.Select(iid => new ImportInvoiceDetailDetailsViewModel()
                {
                    ProductName = iid.Product.Name,
                    ProductDescription = iid.Product.Description,
                    CategoryName = iid.Product.Category.Name,
                    CategoryDescription = iid.Product.Category.Description ?? "N/A",
                    Quantity = iid.Quantity,
                    UnitPrice = iid.UnitPrice ?? 0,
                    Total = iid.Quantity * iid.UnitPrice ?? 0
                }).ToList()
            };

            return OperationResult<ImportInvoiceDetailsViewModel>.Ok(viewModel);
        }

        public async Task<OperationResult<EditImportInvoiceInputModel>> GetImportInvoiceForEditingAsync(
            Guid warehouseId, Guid invoiceId, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult<EditImportInvoiceInputModel>.Failure(UserNotFound);

            var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(warehouseId, userId);

            if (warehouse == null)
                return OperationResult<EditImportInvoiceInputModel>.Failure(NoPermissionOrWarehouseNotFound);

            var invoice = await importInvoiceRepo.GetInvoiceWithDetailsAsync(invoiceId, warehouseId);

            if (invoice == null)
                return OperationResult<EditImportInvoiceInputModel>.Failure(NoPermissionOrImportInvoiceNotFound);

            var model = new EditImportInvoiceInputModel
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                Date = invoice.Date,
                WarehouseId = invoice.WarehouseId,
                SupplierName = invoice.Supplier.Name,
                SupplierAddress = invoice.Supplier.Address,
                SupplierEmail = invoice.Supplier.Email,
                SupplierPhoneNumber = invoice.Supplier.PhoneNumber,
                Products = invoice.ImportInvoicesDetails.Select(d => new EditImportInvoiceDetailInputModel
                {
                    Id = d.Id,
                    ProductName = d.Product.Name,
                    ProductDescription = d.Product.Description,
                    CategoryName = d.Product.Category.Name,
                    CategoryDescription = d.Product.Category.Description,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                }).ToList()
            };

            return OperationResult<EditImportInvoiceInputModel>.Ok(model);
        }

        public async Task<OperationResult> UpdateImportInvoiceAsync(EditImportInvoiceInputModel inputModel, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(inputModel.WarehouseId, userId);

            if (warehouse == null)
                return OperationResult.Failure(NoPermissionOrWarehouseNotFound);

            var invoice = await importInvoiceRepo
                .AllTracked()
                .FirstOrDefaultAsync(i => i.Id == inputModel.Id && i.WarehouseId == inputModel.WarehouseId);

            if (invoice == null)
                return OperationResult.Failure(NoPermissionOrImportInvoiceNotFound);

            bool invoiceExists = await importInvoiceRepo.ExistsAsync(i =>
                i.InvoiceNumber == inputModel.InvoiceNumber &&
                i.WarehouseId == inputModel.WarehouseId &&
                i.Id != inputModel.Id);

            if (invoiceExists)
                return OperationResult.Failure(DuplicateInvoice);

            if (inputModel.Products.Count == 0)
            {
                return OperationResult.Failure(CannnotDeleteAllProducts);
            }

            var duplicateProducts = inputModel.Products
                .GroupBy(p => new
                {
                    ProductName = p.ProductName.ToLower(),
                    CategoryName = p.CategoryName.ToLower()
                })
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicateProducts.Any())
                return OperationResult.Failure(ProductDuplicate);

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

            var newImportDate = inputModel.Date;

            var existingDetailsIds = await importInvoiceDetailRepo.All()
                .Where(d => d.ImportInvoiceId == invoice.Id)
                .Select(d => d.Id)
                .ToListAsync();

            var invalidExportDate = await exportInvoiceDetailRepo.All()
                .Where(e => existingDetailsIds.Contains(e.ImportInvoiceDetailId))
                .Include(e => e.ExportInvoice)
                .Include(e => e.ImportInvoiceDetail)
                    .ThenInclude(d => d.Product)
                .Where(e => e.ExportInvoice.Date < newImportDate)
                .OrderBy(e => e.ExportInvoice.Date)
                .FirstOrDefaultAsync();

            if (invalidExportDate != null)
            {
                return OperationResult.Failure(
                    InvalidDate +
                    $"{invalidExportDate.ExportInvoice.Date:yyyy-MM-dd} with product {invalidExportDate.ImportInvoiceDetail.Product.Name}.");
            }

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
                        var invoiceDetail = await importInvoiceDetailRepo.AllTracked()
                            .Include(iid => iid.Product)
                                .ThenInclude(p => p.Category)
                            .FirstOrDefaultAsync(iid => iid.Id == detail.Id);

                        if (invoiceDetail == null)
                            return OperationResult.Failure(ProductNotFound);

                        var exportedQuantity = await exportInvoiceDetailRepo.All()
                            .Where(e => e.ImportInvoiceDetailId == detail.Id)
                            .SumAsync(e => (int?)e.Quantity) ?? 0;

                        if (detail.Quantity < exportedQuantity)
                            return OperationResult.Failure($"Cannot set quantity for product \"{invoiceDetail.Product.Name}\" to {detail.Quantity}, because {exportedQuantity} have already been exported.");


                        invoiceDetail.Quantity = detail.Quantity;
                        invoiceDetail.UnitPrice = detail.UnitPrice;
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
                            UnitPrice = detail.UnitPrice
                        };

                        await importInvoiceDetailRepo.AddAsync(invoiceDetail);
                    }
                }
                catch
                {
                    return OperationResult.Failure(ErrorMessages.ImportInvoiceDetail.CreationFailure);
                }
            }

            try
            {
                var existingDetails = await importInvoiceDetailRepo.All()
                    .Where(d => d.ImportInvoiceId == invoice.Id)
                    .Include(d => d.Product)
                        .ThenInclude(p => p.Category)
                    .ToListAsync();

                var inputDetailIds = inputModel.Products
                    .Where(p => p.Id.HasValue)
                    .Select(p => p.Id!.Value)
                    .ToHashSet();

                var detailsToRemove = await importInvoiceDetailRepo.AllTracked()
                    .Where(d => d.ImportInvoiceId == invoice.Id && !inputDetailIds.Contains(d.Id))
                    .Include(d => d.ExportInvoicesPerProduct)
                    .ToListAsync();

                var undeletableDetails = detailsToRemove
                    .Where(d => d.ExportInvoicesPerProduct != null && d.ExportInvoicesPerProduct.Count > 0)
                    .ToList();

                if (undeletableDetails.Count > 0)
                {
                    var undeletableProductInputs = undeletableDetails.Select(d => new EditImportInvoiceDetailInputModel
                    {
                        Id = d.Id,
                        ProductDescription = d.Product.Description,
                        ProductName = d.Product.Name,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        CategoryDescription = d.Product.Category.Description,
                        CategoryName = d.Product.Category.Name
                    });

                    foreach (var product in undeletableProductInputs)
                    {
                        inputModel.Products.Add(product);
                    }

                    return OperationResult.Failure(ProductDeletionFailure);
                }

                importInvoiceDetailRepo.DeleteRange(detailsToRemove);
            }
            catch
            {
                return OperationResult.Failure(ErrorMessages.ImportInvoiceDetail.DeletionFailure);
            }

            try
            {
                await importInvoiceRepo.SaveChangesAsync();
                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(ErrorMessages.ImportInvoice.CreationFailure);
            }
        }

        public async Task<OperationResult> DeleteImportInvoiceAsync(Guid warehouseId, Guid invoiceId, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(warehouseId, userId);

            if (warehouse == null)
                return OperationResult.Failure(NoPermissionOrWarehouseNotFound);

            ImportInvoice? invoice = await importInvoiceRepo
                .AllTracked()
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.WarehouseId == warehouseId);

            if (invoice == null)
                return OperationResult.Failure(NoPermissionOrImportInvoiceNotFound);

            var exportInvoicesExist = await exportInvoiceDetailRepo.ExistsAsync(
                e => e.ImportInvoiceDetail.ImportInvoiceId == invoice.Id);

            if (exportInvoicesExist)
            {
                return OperationResult.Failure(ExistingExportInvoices);
            }

            importInvoiceRepo
                .Delete(invoice);

            await importInvoiceRepo.SaveChangesAsync();

            return OperationResult.Ok();
        }
    }
}
