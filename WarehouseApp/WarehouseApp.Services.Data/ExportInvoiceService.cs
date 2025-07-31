using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Data;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.ExportInvoice;
using WarehouseApp.Data.Repository.Interfaces;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.Constants.ApplicationConstants;
using static WarehouseApp.Common.Constants.EntityConstants.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoiceDetail;

namespace WarehouseApp.Services.Data
{
    public class ExportInvoiceService : BaseService, IExportInvoiceService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IImportInvoiceRepository importInvoiceRepo;
        private readonly IImportInvoiceDetailRepository importInvoiceDetailRepo;
        private readonly IExportInvoiceRepository exportInvoiceRepo;
        private readonly IExportInvoiceDetailRepository exportInvoiceDetailRepo;
        private readonly IApplicationUserWarehouseRepository appUserWarehouseRepo;

        private readonly IClientService clientService;
        private readonly IStockService stockService;

        public ExportInvoiceService(
            UserManager<ApplicationUser> userManager,
            IImportInvoiceRepository importInvoiceRepo,
            IImportInvoiceDetailRepository importInvoiceDetailRepo,
            IExportInvoiceRepository exportInvoiceRepo,
            IExportInvoiceDetailRepository exportInvoiceDetailRepo,
            IApplicationUserWarehouseRepository appUserWarehouseRepo,
            IClientService clientService,
            IStockService stockService)
        {
            this.userManager = userManager;
            this.importInvoiceRepo = importInvoiceRepo;
            this.importInvoiceDetailRepo = importInvoiceDetailRepo;
            this.exportInvoiceRepo = exportInvoiceRepo;
            this.exportInvoiceDetailRepo = exportInvoiceDetailRepo;
            this.appUserWarehouseRepo = appUserWarehouseRepo;
            this.clientService = clientService;
            this.stockService = stockService;
        }

        /// <summary>
        /// Retrieves a filtered and paginated list of export invoices for a warehouse owned by the specified user.
        /// </summary>
        /// <param name="inputModel">
        /// A model containing filtering, sorting, and pagination options such as invoice number, client name, and year.
        /// This object is updated with the result data including invoices, total items, total pages, and current page.
        /// </param>
        /// <param name="userId">
        /// The unique identifier of the user requesting the export invoice data.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success if the invoices were retrieved successfully,
        /// or failure with a specific error message if the user is not found, access is denied, or the warehouse is not found.
        /// </returns>
        public async Task<OperationResult> GetInvoicesForWarehouseAsync(
            AllExportInvoicesSearchFilterViewModel inputModel, Guid userId)
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

                IQueryable<ExportInvoice> allInvoicesQuery = exportInvoiceRepo
                    .All()
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
            catch
            {
                return OperationResult.Failure(ErrorMessages.ExportInvoice.RetrievingFailure);
            }
        }

        /// <summary>
        /// Creates a new export invoice for a specified warehouse and user, including validations, client handling,
        /// stock verification, and invoice detail creation.
        /// </summary>
        /// <param name="inputModel">
        /// A model containing export invoice data such as invoice number, date, warehouse ID, client details,
        /// and a list of exported products with quantity and price information.
        /// </param>
        /// <param name="userId">
        /// The unique identifier of the user creating the export invoice.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success if the export invoice was created successfully,
        /// or failure with a specific error message if the user is not found, the warehouse is inaccessible,
        /// the invoice already exists, input is invalid, stock is insufficient, or an internal error occurs during processing.
        /// </returns>
        public async Task<OperationResult> CreateExportInvoiceAsync(CreateExportInvoiceInputModel inputModel, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(inputModel.WarehouseId, userId);

                bool invoiceExists = await exportInvoiceRepo.ExistsAsync(
                    i => i.InvoiceNumber == inputModel.InvoiceNumber &&
                    i.WarehouseId == inputModel.WarehouseId);

                if (invoiceExists)
                    return OperationResult.Failure(DuplicateInvoice);

                if (inputModel.ExportedProducts.Count == 0)
                    return OperationResult.Failure(CannotCreateExportInvoiceWithoutExports);

                var duplicateProducts = inputModel.ExportedProducts
                    .GroupBy(p => new
                    {
                        ImportInvoiceNumber = p.ImportInvoiceNumber.ToLower(),
                        ProductName = p.ProductName.ToLower(),
                        CategoryName = p.CategoryName.ToLower()
                    })
                    .Where(g => g.Count() > 1)
                    .ToList();

                if (duplicateProducts.Any())
                    return OperationResult.Failure(ExportDuplicate);

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

                await exportInvoiceRepo.AddAsync(exportInvoice);

                foreach (var detail in inputModel.ExportedProducts)
                {
                    var importInvoice = await importInvoiceRepo.All()
                        .Include(ii => ii.ImportInvoicesDetails)
                        .ThenInclude(iid => iid.Product)
                        .ThenInclude(p => p.Category)
                        .FirstOrDefaultAsync(ii =>
                            ii.InvoiceNumber == detail.ImportInvoiceNumber &&
                            ii.WarehouseId == inputModel.WarehouseId);

                    if (importInvoice == null)
                        return OperationResult.Failure(NoPermissionOrImportInvoiceNotFound);

                    var importDetail = importInvoice.ImportInvoicesDetails.FirstOrDefault(iid =>
                        iid.Product.Name == detail.ProductName &&
                        iid.Product.Category.Name == detail.CategoryName);

                    if (importDetail == null)
                        return OperationResult.Failure(ProductNotFoundInImportInvoice);

                    if (importInvoice.Date > inputModel.Date)
                    {
                        return OperationResult.Failure(CannotExportBeforeImportDate);
                    }

                    var stockResult = await stockService.GetAvailableQuantityAsync(importDetail.Id);

                    var availableQuantity = stockResult.Data;

                    if (availableQuantity < detail.Quantity)
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

                        await exportInvoiceDetailRepo.AddAsync(exportDetail);
                    }
                    catch
                    {
                        return OperationResult.Failure(ErrorMessages.ExportInvoiceDetail.CreationFailure);
                    }
                }

                await exportInvoiceRepo.SaveChangesAsync();
                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(ErrorMessages.ExportInvoice.CreationFailure);
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific export invoice belonging to a warehouse owned by the specified user.
        /// </summary>
        /// <param name="warehouseId">
        /// The unique identifier of the warehouse where the export invoice is located.
        /// </param>
        /// <param name="invoiceId">
        /// The unique identifier of the export invoice to retrieve.
        /// </param>
        /// <param name="userId">
        /// The unique identifier of the user requesting the invoice details.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult{ExportInvoiceDetailsViewModel}"/> containing the detailed invoice data if successful,
        /// or a failure result with a specific error message if the user is not found, the warehouse is inaccessible,
        /// or the invoice does not exist or is not accessible by the user.
        /// </returns>
        public async Task<OperationResult<ExportInvoiceDetailsViewModel>> GetExportInvoiceDetailsAsync(
           Guid warehouseId, Guid invoiceId, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult<ExportInvoiceDetailsViewModel>.Failure(UserNotFound);

                var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(inputModel.WarehouseId, userId);

                if (warehouse == null)
                    return OperationResult<ExportInvoiceDetailsViewModel>.Failure(NoPermissionOrWarehouseNotFound);

                var exportInvoice = await exportInvoiceRepo
                    .GetExportInvoiceWithDetailsAsync(invoiceId, warehouseId);

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
            catch
            {
                return OperationResult<ExportInvoiceDetailsViewModel>.Failure(ErrorMessages.ExportInvoice.GetModelFailure);
            }
        }

        /// <summary>
        /// Retrieves an export invoice and its associated data for editing, ensuring the user has access to the specified warehouse.
        /// </summary>
        /// <param name="warehouseId">
        /// The unique identifier of the warehouse where the export invoice is located.
        /// </param>
        /// <param name="invoiceId">
        /// The unique identifier of the export invoice to retrieve for editing.
        /// </param>
        /// <param name="userId">
        /// The unique identifier of the user requesting the export invoice for editing.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult{EditExportInvoiceInputModel}"/> containing the export invoice data formatted for editing
        /// if successful, or a failure result with a specific error message if the user is not found, the warehouse is inaccessible,
        /// or the invoice does not exist or is not accessible by the user.
        /// </returns>
        public async Task<OperationResult<EditExportInvoiceInputModel>> GetExportInvoiceForEditingAsync(Guid warehouseId, Guid invoiceId, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult<EditExportInvoiceInputModel>.Failure(UserNotFound);

                var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(warehouseId, userId);

                if (warehouse == null)
                    return OperationResult<EditExportInvoiceInputModel>.Failure(NoPermissionOrWarehouseNotFound);

                var exportInvoice = await exportInvoiceRepo
                    .GetExportInvoiceWithDetailsAsync(invoiceId, warehouseId);

                if (exportInvoice == null)
                    return OperationResult<EditExportInvoiceInputModel>.Failure(NoPermissionOrExportInvoiceNotFound);

                var model = new EditExportInvoiceInputModel
                {
                    Id = exportInvoice.Id,
                    InvoiceNumber = exportInvoice.InvoiceNumber,
                    Date = exportInvoice.Date,
                    WarehouseId = exportInvoice.WarehouseId,
                    ClientName = exportInvoice.Client.Name,
                    ClientAddress = exportInvoice.Client.Address,
                    ClientEmail = exportInvoice.Client.Email,
                    ClientPhoneNumber = exportInvoice.Client.PhoneNumber,
                    ExportedProducts = exportInvoice.ExportInvoicesDetails.Select(d => new EditExportInvoiceDetailInputModel
                    {
                        Id = d.Id,
                        ImportInvoiceNumber = d.ImportInvoiceDetail.ImportInvoice.InvoiceNumber,
                        ProductName = d.ImportInvoiceDetail.Product.Name,
                        CategoryName = d.ImportInvoiceDetail.Product.Category.Name,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice
                    }).ToList()
                };

                return OperationResult<EditExportInvoiceInputModel>.Ok(model);
            }
            catch
            {
                return OperationResult<EditExportInvoiceInputModel>.Failure(ErrorMessages.ExportInvoice.GetModelFailure);
            }
        }

        /// <summary>
        /// Updates an existing export invoice and its related export details for a warehouse owned by the specified user,
        /// including validations, client updates, stock checks, and handling additions, updates, and deletions of products.
        /// </summary>
        /// <param name="inputModel">
        /// A model containing updated export invoice data such as invoice number, date, client info, and exported product details.
        /// </param>
        /// <param name="userId">
        /// The unique identifier of the user requesting the update.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success if the invoice was updated successfully,
        /// or failure with a specific error message if the user is not found, access is denied,
        /// the invoice or product is not found, stock is insufficient, or a validation or save error occurs.
        /// </returns>
        public async Task<OperationResult> UpdateExportInvoiceAsync(EditExportInvoiceInputModel inputModel, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(inputModel.WarehouseId, userId);

                if (warehouse == null)
                    return OperationResult.Failure(NoPermissionOrWarehouseNotFound);

                var exportInvoice = await exportInvoiceRepo
                    .AllTracked()
                    .Include(e => e.ExportInvoicesDetails)
                    .FirstOrDefaultAsync(e => e.Id == inputModel.Id && e.WarehouseId == inputModel.WarehouseId);

                if (exportInvoice == null)
                    return OperationResult.Failure(NoPermissionOrExportInvoiceNotFound);

                bool invoiceExists = await exportInvoiceRepo.ExistsAsync(i =>
                    i.InvoiceNumber == inputModel.InvoiceNumber &&
                    i.WarehouseId == inputModel.WarehouseId &&
                    i.Id != inputModel.Id);

                if (invoiceExists)
                    return OperationResult.Failure(DuplicateInvoice);

                if (inputModel.ExportedProducts.Count == 0)
                    return OperationResult.Failure(CannnotDeleteAllExports);

                var duplicateProducts = inputModel.ExportedProducts
                    .GroupBy(p => new
                    {
                        ImportInvoiceNumber = p.ImportInvoiceNumber.ToLower(),
                        ProductName = p.ProductName.ToLower(),
                        CategoryName = p.CategoryName.ToLower()
                    })
                    .Where(g => g.Count() > 1)
                    .ToList();

                if (duplicateProducts.Any())
                    return OperationResult.Failure(ExportDuplicate);

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

                exportInvoice.InvoiceNumber = inputModel.InvoiceNumber;

                exportInvoice.Client = client;

                foreach (var detail in inputModel.ExportedProducts)
                {
                    var importInvoiceDetail = await importInvoiceDetailRepo
                        .AllTracked()
                        .Include(iid => iid.Product)
                            .ThenInclude(p => p.Category)
                        .Include(iid => iid.ImportInvoice)
                        .FirstOrDefaultAsync(iid =>
                            iid.ImportInvoice.WarehouseId == warehouse.Id &&
                            iid.ImportInvoice.InvoiceNumber == detail.ImportInvoiceNumber &&
                            iid.Product.Name == detail.ProductName &&
                            iid.Product.Category.Name == detail.CategoryName);

                    if (importInvoiceDetail == null)
                        return OperationResult.Failure(ProductNotFoundInImportInvoice);

                    var newExportDate = inputModel.Date;

                    var importDate = importInvoiceDetail.ImportInvoice.Date;

                    if (newExportDate < importDate)
                        return OperationResult.Failure(
                            $"{InvalidDate} Import on {importDate:yyyy-MM-dd} for product {importInvoiceDetail.Product.Name} is after the export date.");

                    var stockResult = await stockService.GetAvailableQuantityAsync(
                        importInvoiceDetail!.Id,
                        detail.Id);

                    var availableQuantity = stockResult.Data;

                    if (availableQuantity < detail.Quantity)
                        return OperationResult.Failure(InsufficientStock);

                    if (detail.Id.HasValue)
                    {
                        var exportDetail = await exportInvoiceDetailRepo.GetByIdAsync(detail.Id.Value);

                        if (exportDetail == null)
                            return OperationResult.Failure(ExportNotFound);

                        exportDetail.ImportInvoiceDetailId = importInvoiceDetail.Id;
                        exportDetail.Quantity = detail.Quantity;
                        exportDetail.UnitPrice = detail.UnitPrice;
                    }
                    else
                    {
                        var newDetail = new ExportInvoiceDetail
                        {
                            ExportInvoiceId = exportInvoice.Id,
                            ImportInvoiceDetailId = importInvoiceDetail.Id,
                            Quantity = detail.Quantity,
                            UnitPrice = detail.UnitPrice,
                        };

                        await exportInvoiceDetailRepo.AddAsync(newDetail);
                    }
                }

                exportInvoice.Date = inputModel.Date;

                try
                {
                    var existingDetails = await exportInvoiceDetailRepo
                        .AllTracked()
                        .Where(e => e.ExportInvoiceId == exportInvoice.Id)
                        .ToListAsync();

                    var inputDetailIds = inputModel.ExportedProducts
                        .Where(p => p.Id.HasValue)
                        .Select(p => p.Id!.Value)
                        .ToHashSet();

                    var detailsToRemove = existingDetails
                        .Where(e => !inputDetailIds.Contains(e.Id))
                        .ToList();

                    exportInvoiceDetailRepo.DeleteRange(detailsToRemove);
                }
                catch
                {
                    return OperationResult.Failure(ErrorMessages.ExportInvoiceDetail.DeletionFailure);
                }

                await exportInvoiceRepo.SaveChangesAsync();
                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(ErrorMessages.ExportInvoice.EditingFailure);
            }
        }

        /// <summary>
        /// Deletes a specific export invoice and its associated export details for a warehouse owned by the specified user.
        /// </summary>
        /// <param name="warehouseId">
        /// The unique identifier of the warehouse where the export invoice is located.
        /// </param>
        /// <param name="invoiceId">
        /// The unique identifier of the export invoice to be deleted.
        /// </param>
        /// <param name="userId">
        /// The unique identifier of the user requesting the deletion.
        /// </param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success if the export invoice was deleted successfully,
        /// or failure with a specific error message if the user is not found, the warehouse or invoice is inaccessible,
        /// or an error occurs during deletion.
        /// </returns>
        public async Task<OperationResult> DeleteExportInvoiceAsync(Guid warehouseId, Guid invoiceId, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                var warehouse = await appUserWarehouseRepo.GetWarehouseOwnedByUserAsync(warehouseId, userId);

                if (warehouse == null)
                    return OperationResult.Failure(NoPermissionOrWarehouseNotFound);

                var exportInvoice = await exportInvoiceRepo
                    .AllTracked()
                    .Include(e => e.ExportInvoicesDetails)
                    .FirstOrDefaultAsync(i => i.Id == invoiceId && i.WarehouseId == warehouseId);

                if (exportInvoice == null)
                    return OperationResult.Failure(NoPermissionOrExportInvoiceNotFound);

                exportInvoiceDetailRepo.DeleteRange(exportInvoice.ExportInvoicesDetails);

                exportInvoiceRepo.Delete(exportInvoice);


                await exportInvoiceRepo.SaveChangesAsync();
                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(ErrorMessages.ExportInvoice.DeletionFailure);
            }
        }
    }
}
