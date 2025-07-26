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
        private readonly IWarehouseRepository warehouseRepo;
        private readonly IApplicationUserWarehouseRepository appUserWarehouseRepo;
        private readonly IImportInvoiceDetailRepository importInvoiceDetailRepo;

        public WarehouseService(
            UserManager<ApplicationUser> userManager,
            IWarehouseRepository warehouseRepository,
            IApplicationUserWarehouseRepository appUserWarehouseRepo,
            IImportInvoiceDetailRepository importInvoiceDetailRepository)
        {
            this.userManager = userManager;
            this.warehouseRepo = warehouseRepository;
            this.appUserWarehouseRepo = appUserWarehouseRepo;
            this.importInvoiceDetailRepo = importInvoiceDetailRepository;
        }

        /// <summary>
        /// Retrieves a filtered and paginated list of warehouses for the specified user.
        /// Returns failure if the user is not found.
        /// </summary>
        /// <param name="inputModel">The search and pagination filter model.</param>
        /// <param name="userId">The ID of the user whose warehouses are queried.</param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success or failure of the retrieval operation.
        /// On success, the input model is populated with the filtered warehouse list and pagination data.
        /// </returns>
        public async Task<OperationResult> GetWarehousesForUserAsync(
            AllWarehousesSearchFilterViewModel inputModel, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                IQueryable<Warehouse> allWarehousesQuery = warehouseRepo
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

        /// <summary>
        /// Creates a new warehouse for the specified user.
        /// Returns failure if the user is not found or if a warehouse with the same name already exists for the user.
        /// </summary>
        /// <param name="inputModel">The input model containing warehouse details.</param>
        /// <param name="userId">The ID of the user creating the warehouse.</param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating success or failure of the creation operation.
        /// </returns>
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

        /// <summary>
        /// Retrieves detailed information about a warehouse if it exists and is accessible by the specified user.
        /// Returns failure if the user is not found, the warehouse does not exist or is deleted,
        /// or if the user lacks permission to view the warehouse.
        /// </summary>
        /// <param name="warehouseId">The ID of the warehouse to retrieve details for.</param>
        /// <param name="userId">The ID of the user requesting the warehouse details.</param>
        /// <returns>
        /// An <see cref="OperationResult{WarehouseDetailsViewModel}"/> containing detailed warehouse information
        /// on success, or failure information otherwise.
        /// </returns>
        public async Task<OperationResult<WarehouseDetailsViewModel>> GetWarehouseDetailsAsync(Guid warehouseId, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult<WarehouseDetailsViewModel>.Failure(UserNotFound);

                Warehouse? warehouse = await warehouseRepo.GetWarehouseDetailsByIdAsync(warehouseId);

                if (warehouse == null || warehouse.IsDeleted)
                    return OperationResult<WarehouseDetailsViewModel>.Failure(WarehouseNotFound);

                bool hasPermission = warehouse.CreatedByUserId == userId
                    || warehouse.WarehouseUsers.Any(wu => wu.ApplicationUserId == userId);

                if (!hasPermission)
                    return OperationResult<WarehouseDetailsViewModel>.Failure(NoPermission);

                var products = await importInvoiceDetailRepo
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

        /// <summary>
        /// Retrieves a warehouse for editing if it exists and is owned by the specified user.
        /// Returns failure if the user is not found, the warehouse does not exist,
        /// or the user does not have permission to access it.
        /// </summary>
        /// <param name="warehouseId">The ID of the warehouse to retrieve.</param>
        /// <param name="userId">The ID of the user requesting the warehouse.</param>
        /// <returns>
        /// An <see cref="OperationResult{EditWarehouseInputModel}"/> containing the warehouse data
        /// for editing on success, or failure information otherwise.
        /// </returns>
        public async Task<OperationResult<EditWarehouseInputModel>> GetWarehouseForEditingAsync(Guid warehouseId, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult<EditWarehouseInputModel>.Failure(UserNotFound);

                Warehouse? warehouse = await warehouseRepo.GetByIdAsNoTrackingAsync(warehouseId);

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

        /// <summary>
        /// Updates the details of a warehouse if the specified user is the owner.
        /// Returns failure if the user is not found, the warehouse does not exist,
        /// the user lacks permission, or if a warehouse with the same name already exists.
        /// </summary>
        /// <param name="inputModel">The input model containing updated warehouse data.</param>
        /// <param name="userId">The ID of the user attempting the update.</param>
        /// <returns>An <see cref="OperationResult"/> indicating success or failure.</returns>
        public async Task<OperationResult> UpdateWarehouseAsync(EditWarehouseInputModel inputModel, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                Warehouse? warehouse = await warehouseRepo.GetByIdAsync(inputModel.Id);

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

                await warehouseRepo.SaveChangesAsync();

                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(EditingFailure);
            }
        }

        /// Deletes a warehouse by marking it as deleted if the user is the owner.
        /// Returns failure if the user or warehouse is not found, or if already deleted.
        /// /// <summary>
        /// Deletes a warehouse by marking it as deleted if the specified user is the owner.
        /// Returns failure if the user or warehouse is not found, if the warehouse is already deleted,
        /// or if the user does not have permission to delete the warehouse.
        /// </summary>
        /// <param name="warehouseId">The ID of the warehouse to delete.</param>
        /// <param name="userId">The ID of the user attempting the deletion.</param>
        /// <returns>An <see cref="OperationResult"/> indicating success or failure.</returns>
        public async Task<OperationResult> DeleteWarehouseAsync(Guid warehouseId, Guid userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return OperationResult.Failure(UserNotFound);

                Warehouse? warehouse = await warehouseRepo.GetByIdAsync(warehouseId);

                if (warehouse == null)
                    return OperationResult.Failure(WarehouseNotFound);

                if (warehouse.IsDeleted)
                    return OperationResult.Failure(AlreadyDeleted);

                if (warehouse.CreatedByUserId != userId)
                    return OperationResult.Failure(NoPermission);

                warehouse.Name += $"/DeletedOn/{DateTime.Now:dd-MM-yyyy/HH:mm:ss}";
                warehouse.IsDeleted = true;

                await warehouseRepo.SaveChangesAsync();

                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(DeletionFailure);
            }
        }

        /// <summary>
        /// Forcefully marks a warehouse as deleted without checking ownership.
        /// This is intended for internal system use (e.g., during user cleanup).
        /// Returns failure if the warehouse is not found or is already marked as deleted.
        /// </summary>
        /// <param name="warehouseId">The ID of the warehouse to mark as deleted.</param>
        /// <returns>An <see cref="OperationResult"/> indicating success or failure.</returns>
        public async Task<OperationResult> MarkAsDeletedWithoutSavingAsync(Guid warehouseId)
        {
            try
            {
                var warehouse = await warehouseRepo.GetByIdAsync(warehouseId);

                if (warehouse == null)
                    return OperationResult.Failure(WarehouseNotFound);

                if (warehouse.IsDeleted)
                    return OperationResult.Failure(AlreadyDeleted);

                warehouse.Name += $"/DeletedOn/{DateTime.Now:dd-MM-yyyy/HH:mm:ss}";
                warehouse.IsDeleted = true;

                return OperationResult.Ok();
            }
            catch
            {
                return OperationResult.Failure(DeletionFailure);
            }
        }
    }
}
