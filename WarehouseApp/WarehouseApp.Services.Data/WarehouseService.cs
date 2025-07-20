using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Web.ViewModels.Warehouse;
using WarehouseApp.Data;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;
using WarehouseApp.Web.ViewModels.Shared;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.Constants.ApplicationConstants;
using static WarehouseApp.Common.Constants.EntityConstants.Warehouse;

namespace WarehouseApp.Services.Data
{
    public class WarehouseService : BaseService, IWarehouseService
    {
        private readonly WarehouseDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public WarehouseService(WarehouseDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<OperationResult> GetWarehousesForUserAsync(
            AllWarehousesSearchFilterViewModel inputModel, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            IQueryable<Warehouse> allWarehousesQuery = dbContext.Warehouses
                .AsNoTracking()
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

            var warhouses = await allWarehousesQuery.Select(w => new WarehouseCardViewModel
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

        public async Task<OperationResult> CreateWarehouseAsync(CreateWarehouseInputModel inputModel, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            bool warehouseExists = await dbContext.UsersWarehouses
                .AnyAsync(uw => uw.ApplicationUserId == userId && uw.Warehouse.Name.ToLower() == inputModel.Name.ToLower());

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

            await dbContext.UsersWarehouses.AddAsync(userWarehouse);
            await dbContext.SaveChangesAsync();

            return OperationResult.Ok();
        }

        public async Task<OperationResult<WarehouseDetailsViewModel>> GetWarehouseDetailsAsync(Guid warehouseId, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult<WarehouseDetailsViewModel>.Failure(UserNotFound);

            Warehouse? warehouse = await dbContext.Warehouses
                .AsNoTracking()
                .Include(w => w.WarehouseUsers)
                .Include(w => w.CreatedByUser)
                .Include(w => w.ImportInvoices)
                .Include(w => w.ExportInvoices)
                .FirstOrDefaultAsync(w => w.Id == warehouseId);

            int totalAvailableGoods = await dbContext.ImportInvoiceDetails
                .AsNoTracking()
                .Where(detail => detail.ImportInvoice.WarehouseId == warehouseId)
                .Select(detail => new
                {
                    ImportQuantity = detail.Quantity,
                    ExportedQuantity = detail.ExportInvoicesPerProduct.Sum(e => (int?)e.Quantity) ?? 0
                })
                .CountAsync(q => (q.ImportQuantity - q.ExportedQuantity) > 0);

            if (warehouse == null || warehouse.IsDeleted)
                return OperationResult<WarehouseDetailsViewModel>.Failure(WarehouseNotFound);

            bool hasPermission = warehouse.CreatedByUserId == userId
                || warehouse.WarehouseUsers.Any(wu => wu.ApplicationUserId == userId);

            if (!hasPermission)
                return OperationResult<WarehouseDetailsViewModel>.Failure(NoPermission);

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

        public async Task<OperationResult<EditWarehouseInputModel>> GetWarehouseForEditingAsync(Guid warehouseId, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult<EditWarehouseInputModel>.Failure(UserNotFound);

            Warehouse? warehouse = await dbContext.Warehouses
               .Where(w => w.Id == warehouseId)
               .AsNoTracking()
               .FirstOrDefaultAsync();

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

        public async Task<OperationResult> UpdateWarehouseAsync(EditWarehouseInputModel inputModel, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            Warehouse? warehouse = await dbContext.Warehouses
               .Where(w => w.Id == inputModel.Id)
               .FirstOrDefaultAsync();

            if (warehouse == null)
                return OperationResult.Failure(WarehouseNotFound);

            if (warehouse.CreatedByUserId != userId)
                return OperationResult.Failure(NoPermission);

            bool warehouseExists = await dbContext.UsersWarehouses
            .AnyAsync(uw => uw.ApplicationUserId == userId &&
            uw.Warehouse.Name.ToLower() == inputModel.Name.ToLower()
            && uw.WarehouseId != inputModel.Id);
            if (warehouseExists)
                return OperationResult.Failure(WarehouseDuplicateName);

            warehouse.Name = inputModel.Name;
            warehouse.Address = inputModel.Address;
            warehouse.Size = inputModel.Size;

            await dbContext.SaveChangesAsync();

            return OperationResult.Ok();
        }

        public async Task<OperationResult> DeleteWarehouseAsync(Guid warehouseId, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult.Failure(UserNotFound);

            Warehouse? warehouse = await dbContext.Warehouses
                .Include(w => w.WarehouseUsers)
                .FirstOrDefaultAsync(w => w.Id == warehouseId);

            if (warehouse == null)
                return OperationResult.Failure(WarehouseNotFound);

            if (warehouse.IsDeleted)
                return OperationResult.Failure(AlreadyDeleted);

            if (warehouse.CreatedByUserId != userId)
                return OperationResult.Failure(NoPermission);

            warehouse.Name += $"/DeletedOn/{DateTime.Now:dd-MM-yyyy/HH:mm:ss}";
            warehouse.IsDeleted = true;

            await dbContext.SaveChangesAsync();

            return OperationResult.Ok();
        }
    }
}
