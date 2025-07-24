using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

using WarehouseApp.Web.ViewModels.Warehouse;
using WarehouseApp.Web.ViewModels.Shared;
using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Services.Data.Interfaces;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.Constants.ApplicationConstants;
using static WarehouseApp.Common.Constants.EntityConstants.Warehouse;

namespace WarehouseApp.Services.Data
{
    public class WarehouseService : BaseService, IWarehouseService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWarehouseRepository warehouseRepository;
        private readonly IApplicationUserWarehouseRepository appUserWarehouseRepo;
        private readonly IImportInvoiceDetailRepository importInvoiceDetailRepository;

        public WarehouseService(
            UserManager<ApplicationUser> userManager,
            IWarehouseRepository warehouseRepository,
            IApplicationUserWarehouseRepository appUserWarehouseRepo,
            IImportInvoiceDetailRepository importInvoiceDetailRepository)
        {
            this.userManager = userManager;
            this.warehouseRepository = warehouseRepository;
            this.appUserWarehouseRepo = appUserWarehouseRepo;
            this.importInvoiceDetailRepository = importInvoiceDetailRepository;
        }

        /// Retrieves a filtered list of warehouses for the specified user.
        /// Returns failure if the user is not found.
        public async Task<OperationResult> GetWarehousesForUserAsync(
            AllWarehousesSearchFilterViewModel inputModel, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                IQueryable<Warehouse> allWarehousesQuery = warehouseRepository
                    .All()
                    .Where(w => w.WarehouseUsers.Any(uw => uw.ApplicationUserId == userId));

                inputModel.TotalUserWarehouses = await allWarehousesQuery.CountAsync();

                if (!string.IsNullOrWhiteSpace(inputModel.SearchQuery))
                {
                    allWarehousesQuery = allWarehousesQuery
                        .Where(w => w.Name.ToLower().Contains(inputModel.SearchQuery.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(inputModel.YearFilter))
                {
                    Match rangeMatch = Regex.Match(inputModel.YearFilter, YearFilterRangeRegex);
                    if (rangeMatch.Success)
                    {
                        int startYear = int.Parse(rangeMatch.Groups[1].Value);
                        int endYear = int.Parse(rangeMatch.Groups[2].Value);

                        allWarehousesQuery = allWarehousesQuery
                            .Where(w => w.CreatedDate.Year >= startYear &&
                                        w.CreatedDate.Year <= endYear);
                    }
                    else
                    {
                        bool isValidNumber = int.TryParse(inputModel.YearFilter, out int year);
                        if (isValidNumber)
                        {
                            allWarehousesQuery = allWarehousesQuery
                                .Where(w => w.CreatedDate.Year == year);
                        }
                    }
                }

                allWarehousesQuery = allWarehousesQuery.OrderBy(w => w.Name);

                inputModel.TotalItemsBeforePagination = await allWarehousesQuery.CountAsync();

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

                allWarehousesQuery = allWarehousesQuery
                        .Skip(inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage!.Value - 1))
                        .Take(inputModel.EntitiesPerPage.Value);

                var warhouses = await allWarehousesQuery
                    .Select(w => new WarehouseCardViewModel
                    {
                        Id = w.Id.ToString(),
                        Name = w.Name,
                        Address = w.Address,
                        CreatedDate = w.CreatedDate.ToString(DateFormat),
                        Size = w.Size.ToString(),
                    })
                    .ToArrayAsync();

                inputModel.Warehouses = warhouses;

                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(RetrievingFailure);
            }
        }

        /// Creates a new warehouse for the specified user.
        /// Returns failure if the user is not found or a warehouse with the same name already exists.
        public async Task<OperationResult> CreateWarehouseAsync(CreateWarehouseInputModel inputModel, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                bool warehouseExists = await appUserWarehouseRepo
                    .UserHasWarehouseWithNameAsync(userId, inputModel.Name);

                if (warehouseExists)
                    return OperationResult.Failure(WarehouseDuplicateName);

                Warehouse warehouse = new Warehouse()
                {
                    Name = inputModel.Name,
                    Address = inputModel.Address,
                    Size = inputModel?.Size,
                    CreatedByUserId = userId
                };

                ApplicationUserWarehouse userWarehouse = new ApplicationUserWarehouse()
                {
                    ApplicationUser = user,
                    Warehouse = warehouse
                };

                await appUserWarehouseRepo.AddAsync(userWarehouse);
                await appUserWarehouseRepo.SaveChangesAsync();

                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(CreationFailure);
            }
        }

        /// Retrieves detailed information about a warehouse if it exists and is owned by the user.
        /// Returns failure if the user is not found, the warehouse doesn't exist, or access is denied.
        public async Task<OperationResult<WarehouseDetailsViewModel>> GetWarehouseDetailsAsync(Guid warehouseId, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult<WarehouseDetailsViewModel>.Failure(UserNotFound);

                Warehouse? warehouse = await warehouseRepository.GetWarehouseDetailsByIdAsync(warehouseId);

                if (warehouse == null || warehouse.IsDeleted)
                    return OperationResult<WarehouseDetailsViewModel>.Failure(WarehouseNotFound);

                bool hasPermission = warehouse.CreatedByUserId == userId
                    || warehouse.WarehouseUsers.Any(wu => wu.ApplicationUserId == userId);

                if (!hasPermission)
                    return OperationResult<WarehouseDetailsViewModel>.Failure(NoPermission);

                var products = await importInvoiceDetailRepository
                     .GetAvailableProductsByWarehouseIdAsync(warehouseId);

                var totalAvailableGoods = products
                    .GroupBy(iid => new
                    {
                        iid.ProductId,
                        ProductName = iid.Product.Name,
                        CategoryName = iid.Product.Category.Name
                    })
                    .Where(g =>
                        g.Sum(iid => iid.Quantity) >
                        g.Sum(iid => iid.ExportInvoicesPerProduct.Sum(e => e.Quantity)))
                    .Count();

                var viewModel = new WarehouseDetailsViewModel
                {
                    Id = warehouse.Id.ToString(),
                    Name = warehouse.Name,
                    Address = warehouse.Address,
                    CreatedByUser = warehouse.CreatedByUser?.Email!,
                    CreatedDate = warehouse.CreatedDate.ToString(DateFormat),
                    IsUserOwner = warehouse.CreatedByUser?.Email == user.Email,
                    Size = warehouse.Size.ToString(),
                    TotalImportInvoices = warehouse.ImportInvoices.Count,
                    TotalExportInvoices = warehouse.ExportInvoices.Count,
                    TotalAvailableGoods = totalAvailableGoods
                };

                return OperationResult<WarehouseDetailsViewModel>.Ok(viewModel);
            }
            catch
            {
                return OperationResult<WarehouseDetailsViewModel>.Failure(GetModelFailure);
            }
        }

        /// Retrieves a warehouse for editing if it exists and is owned by the user.
        /// Returns failure if the user is not found, the warehouse doesn't exist, or access is denied.
        public async Task<OperationResult<EditWarehouseInputModel>> GetWarehouseForEditingAsync(Guid warehouseId, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult<EditWarehouseInputModel>.Failure(UserNotFound);

                Warehouse? warehouse = await warehouseRepository.GetByIdAsNoTrackingAsync(warehouseId);

                if (warehouse == null)
                    return OperationResult<EditWarehouseInputModel>.Failure(WarehouseNotFound);

                if (warehouse.CreatedByUserId != userId)
                    return OperationResult<EditWarehouseInputModel>.Failure(NoPermission);

                EditWarehouseInputModel editModel = new EditWarehouseInputModel()
                {
                    Id = warehouse.Id,
                    Name = warehouse.Name,
                    Address = warehouse.Address,
                    Size = warehouse.Size
                };

                return OperationResult<EditWarehouseInputModel>.Ok(editModel);
            }
            catch
            {
                return OperationResult<EditWarehouseInputModel>.Failure(GetModelFailure);
            }
        }

        /// Updates a warehouse's details if the user is the owner.
        /// Returns failure if the user is unauthorized or the warehouse doesn't exist.
        public async Task<OperationResult> UpdateWarehouseAsync(EditWarehouseInputModel inputModel, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                Warehouse? warehouse = await warehouseRepository.GetByIdAsync(inputModel.Id);

                if (warehouse == null)
                    return OperationResult.Failure(WarehouseNotFound);

                if (warehouse.CreatedByUserId != userId)
                    return OperationResult.Failure(NoPermission);

                bool warehouseExists = await appUserWarehouseRepo
                     .UserHasWarehouseWithNameAsync(userId, inputModel.Name, inputModel.Id);

                if (warehouseExists)
                    return OperationResult.Failure(WarehouseDuplicateName);

                warehouse.Name = inputModel.Name;
                warehouse.Address = inputModel.Address;
                warehouse.Size = inputModel.Size;

                await warehouseRepository.SaveChangesAsync();

                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(EditingFailure);
            }
        }

        /// Deletes a warehouse by marking it as deleted if the user is the owner.
        /// Returns failure if the user or warehouse is not found, or if already deleted.
        public async Task<OperationResult> DeleteWarehouseAsync(Guid warehouseId, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                Warehouse? warehouse = await warehouseRepository.GetByIdAsync(warehouseId);

                if (warehouse == null)
                    return OperationResult.Failure(WarehouseNotFound);

                if (warehouse.IsDeleted)
                    return OperationResult.Failure(AlreadyDeleted);

                if (warehouse.CreatedByUserId != userId)
                    return OperationResult.Failure(NoPermission);

                warehouse.Name += $"/DeletedOn/{DateTime.Now:dd-MM-yyyy/HH:mm:ss}";
                warehouse.IsDeleted = true;

                await warehouseRepository.SaveChangesAsync();

                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(DeletionFailure);
            }
        }
    }
}
