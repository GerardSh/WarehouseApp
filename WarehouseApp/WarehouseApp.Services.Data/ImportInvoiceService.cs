using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.ImportInvoice;
using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Services.Data.Dtos.ImportInvoices;

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
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IImportInvoiceRepository importInvoiceRepo;
        private readonly IImportInvoiceDetailRepository importInvoiceDetailRepo;
        private readonly IExportInvoiceDetailRepository exportInvoiceDetailRepo;
        private readonly IApplicationUserWarehouseRepository appUserWarehouseRepo;

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
            IProductService productService,
            ILogger<ImportInvoiceService> logger
            )
            : base(logger)
        {
            this.userManager = userManager;
            this.importInvoiceRepo = importInvoiceRepo;
            this.importInvoiceDetailRepo = importInvoiceDetailRepo;
            this.exportInvoiceDetailRepo = exportInvoiceDetailRepo;
            this.appUserWarehouseRepo = appUserWarehouseRepo;
            this.clientService = clientService;
            this.categoryService = categoryService;
            this.productService = productService;
        }

        /// <summary>
        /// Retrieves a filtered and paginated list of import invoices for a warehouse owned by the specified user.
        /// In case of an unexpected exception, logs the error and returns a failure result.
        /// </summary>
        /// <param name="inputModel">
        /// A model containing filtering, sorting, and pagination options such as search query, supplier name, and year.
        /// This object is updated with the result data including invoices, total pages, and current page.
        /// </param>
        /// <param name="userId">
        /// The unique identifier of the user requesting the invoice data.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success if the invoices were retrieved successfully,
        /// or failure with a specific error message if the user is not found, access is denied, or an error occurs.
        /// </returns>
        public async Task<OperationResult> GetInvoicesForWarehouseAsync(
            AllImportInvoicesSearchFilterViewModel inputModel, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(inputModel.WarehouseId, userId);

                if (warehouse == null)
                    return OperationResult.Failure(NoPermissionOrWarehouseNotFound);

                inputModel.WarehouseName = warehouse.Name;

                IQueryable<ImportInvoice> allInvoicesQuery = importInvoiceRepo
                    .All()
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
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.ImportInvoice.RetrievingFailure);
                return OperationResult.Failure(ErrorMessages.ImportInvoice.RetrievingFailure);
            }
        }

        /// <summary>
        /// Creates a new import invoice with associated products for a warehouse owned by the specified user.
        /// In case of an unexpected exception, logs the error and returns a failure result.
        /// </summary>
        /// <param name="inputModel">
        /// The input model containing invoice details, supplier information, and a list of products to include.
        /// </param>
        /// <param name="userId">
        /// The unique identifier of the user creating the invoice.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success if the invoice was created successfully,
        /// or failure with a specific error message if validation fails, a duplicate exists,
        /// the user lacks permission, or an internal error occurs during the creation process.
        /// </returns>
        public async Task<OperationResult> CreateImportInvoiceAsync(CreateImportInvoiceInputModel inputModel, Guid userId)
        {
            try
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
                catch (Exception ex)
                {
                    logger.LogError(ex, ErrorMessages.Client.CreationFailure);
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
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ErrorMessages.Category.CreationFailure);
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
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ErrorMessages.Product.CreationFailure);
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
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ErrorMessages.ImportInvoiceDetail.CreationFailure);
                        return OperationResult.Failure(ErrorMessages.ImportInvoiceDetail.CreationFailure);
                    }
                }

                try
                {
                    await importInvoiceRepo.SaveChangesAsync();
                    return OperationResult.Ok();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ErrorMessages.ImportInvoice.CreationFailure);
                    return OperationResult.Failure(ErrorMessages.ImportInvoice.CreationFailure);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.ImportInvoice.CreationFailure);
                return OperationResult.Failure(ErrorMessages.ImportInvoice.CreationFailure);
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific import invoice, including supplier and product details,
        /// for a warehouse owned by the specified user.
        /// In case of an unexpected exception, logs the error and returns a failure result.
        /// </summary>
        /// <param name="warehouseId">
        /// The unique identifier of the warehouse where the invoice is located.
        /// </param>
        /// <param name="invoiceId">
        /// The unique identifier of the import invoice to retrieve.
        /// </param>
        /// <param name="userId">
        /// The unique identifier of the user requesting the invoice details.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult{T}"/> containing an <see cref="ImportInvoiceDetailsViewModel"/> on success,
        /// or a failure result with a specific error message if the user or invoice is not found,
        /// access is denied, or an error occurs during processing.
        /// </returns>
        public async Task<OperationResult<ImportInvoiceDetailsViewModel>> GetImportInvoiceDetailsAsync(
            Guid warehouseId, Guid invoiceId, Guid userId)
        {
            try
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
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.ImportInvoice.GetModelFailure);
                return OperationResult<ImportInvoiceDetailsViewModel>.Failure(ErrorMessages.ImportInvoice.GetModelFailure);
            }
        }

        /// <summary>
        /// Retrieves an import invoice and its associated product details for editing,
        /// ensuring the invoice belongs to a warehouse owned by the specified user.
        /// In case of an unexpected exception, logs the error and returns a failure result.
        /// </summary>
        /// <param name="warehouseId">
        /// The unique identifier of the warehouse containing the invoice.
        /// </param>
        /// <param name="invoiceId">
        /// The unique identifier of the import invoice to retrieve for editing.
        /// </param>
        /// <param name="userId">
        /// The unique identifier of the user requesting the invoice data.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult{T}"/> containing an <see cref="EditImportInvoiceInputModel"/> on success,
        /// or a failure result with a specific error message if the user or invoice is not found,
        /// access is denied, or an error occurs during retrieval.
        /// </returns>
        public async Task<OperationResult<EditImportInvoiceInputModel>> GetImportInvoiceForEditingAsync(
            Guid warehouseId, Guid invoiceId, Guid userId)
        {
            try
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
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.ImportInvoice.GetModelFailure);
                return OperationResult<EditImportInvoiceInputModel>.Failure(ErrorMessages.ImportInvoice.GetModelFailure);
            }
        }

        /// <summary>
        /// Updates an existing import invoice and its associated product details for a warehouse owned by the specified user.
        /// Performs validation for duplicates, product exports, and ensures data consistency.
        /// In case of an unexpected exception, logs the error and returns a failure result.
        /// </summary>
        /// <param name="inputModel">
        /// The input model containing updated invoice and product details.
        /// </param>
        /// <param name="userId">
        /// The unique identifier of the user performing the update.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success if the update is completed successfully,
        /// or failure with a specific error message if the user or invoice is not found, access is denied,
        /// data is invalid, products cannot be removed due to prior exports, or an internal error occurs.
        /// </returns>
        public async Task<OperationResult> UpdateImportInvoiceAsync(EditImportInvoiceInputModel inputModel, Guid userId)
        {
            try
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
                catch (Exception ex)
                {
                    logger.LogError(ex, ErrorMessages.Client.CreationFailure);
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
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ErrorMessages.Category.CreationFailure);
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
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ErrorMessages.Product.CreationFailure);
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
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ErrorMessages.ImportInvoiceDetail.CreationFailure);
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
                catch (Exception ex)
                {
                    logger.LogError(ex, ErrorMessages.ImportInvoiceDetail.DeletionFailure);
                    return OperationResult.Failure(ErrorMessages.ImportInvoiceDetail.DeletionFailure);
                }

                await importInvoiceRepo.SaveChangesAsync();
                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.ImportInvoice.EditingFailure);
                return OperationResult.Failure(ErrorMessages.ImportInvoice.EditingFailure);
            }
        }

        /// <summary>
        /// Deletes a specific import invoice from a warehouse owned by the specified user,
        /// only if there are no associated export invoices.
        /// In case of an unexpected exception, logs the error and returns a failure result.
        /// </summary>
        /// <param name="warehouseId">
        /// The unique identifier of the warehouse containing the invoice.
        /// </param>
        /// <param name="invoiceId">
        /// The unique identifier of the import invoice to be deleted.
        /// </param>
        /// <param name="userId">
        /// The unique identifier of the user performing the deletion.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success if the invoice was deleted,
        /// or failure with a specific error message if the user or invoice is not found, access is denied,
        /// export records exist that prevent deletion, or an error occurs during the process.
        /// </returns>
        public async Task<OperationResult> DeleteImportInvoiceAsync(Guid warehouseId, Guid invoiceId, Guid userId)
        {
            try
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
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.ImportInvoice.DeletionFailure);
                return OperationResult.Failure(ErrorMessages.ImportInvoice.DeletionFailure);
            }
        }

        /// <summary>
        /// Retrieves a summarized list of import invoices for a specific warehouse,
        /// including only their basic identifiers and associated import detail IDs.
        /// This method is optimized for lightweight listing and display scenarios,
        /// without loading full navigation properties.
        /// In case of an unexpected exception, logs the error and returns a failure result.
        /// </summary>
        /// <param name="warehouseId">
        /// The unique identifier of the warehouse whose invoices should be retrieved.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult{T}"/> containing:
        /// - A collection of <see cref="ImportInvoiceSummaryDto"/> if invoices are found.
        /// - An empty collection if none exist.
        /// - An error message if the operation fails.
        /// </returns>
        public async Task<OperationResult<IEnumerable<ImportInvoiceSummaryDto>>> GetInvoicesByWarehouseIdAsync(Guid warehouseId)
        {
            try
            {
                var invoices = await importInvoiceRepo.All()
                        .Where(i => i.WarehouseId == warehouseId)
                        .OrderByDescending(i => i.Date)
                        .Select(i => new ImportInvoiceSummaryDto
                        {
                            Id = i.Id,
                            InvoiceNumber = i.InvoiceNumber,
                            ImportDetails = i.ImportInvoicesDetails.Select(iid => iid.Id)
                        })
                        .ToListAsync();

                if (!invoices.Any())
                    return OperationResult<IEnumerable<ImportInvoiceSummaryDto>>.Failure(NoInvoicesFound);

                return OperationResult<IEnumerable<ImportInvoiceSummaryDto>>.Ok(invoices);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.ImportInvoice.RetrievingFailure);
                return OperationResult<IEnumerable<ImportInvoiceSummaryDto>>.Failure(ErrorMessages.ImportInvoice.RetrievingFailure);
            }
        }

        /// <summary>
        /// Retrieves a full import invoice with all associated import details, products, and product categories,
        /// based on the invoice number and warehouse ID.
        /// This method is intended for detailed inspection or processing scenarios
        /// where complete data is needed (e.g., for linking export operations).
        /// In case of an unexpected exception, logs the error and returns a failure result.
        /// </summary>
        /// <param name="warehouseId">
        /// The unique identifier of the warehouse that owns the invoice.
        /// </param>
        /// <param name="invoiceNumber">
        /// The invoice number of the import invoice to retrieve.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult{T}"/> containing:
        /// - The fully loaded <see cref="ImportInvoice"/> entity if found.
        /// - An error message if the invoice is not found or if the operation fails.
        /// </returns>
        public async Task<OperationResult<ImportInvoice>> GetInvoiceByNumberAsync(Guid warehouseId, string invoiceNumber)
        {
            try
            {
                var invoice = await importInvoiceRepo
                    .All()
                    .Include(i => i.ImportInvoicesDetails)
                        .ThenInclude(d => d.Product)
                            .ThenInclude(p => p.Category)
                    .Where(i => i.WarehouseId == warehouseId && i.InvoiceNumber == invoiceNumber)
                    .FirstOrDefaultAsync();

                if (invoice == null)
                    return OperationResult<ImportInvoice>.Failure(NoPermissionOrImportInvoiceNotFound);

                return OperationResult<ImportInvoice>.Ok(invoice);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.ImportInvoice.GetModelFailure);
                return OperationResult<ImportInvoice>.Failure(ErrorMessages.ImportInvoice.GetModelFailure);
            }
        }
    }
}
