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

        public async Task<IEnumerable<WarehouseCardViewModel>> GetWarehousesForUserAsync(AllWarehousesSearchFilterViewModel inputModel, Guid userId)
        {
            IQueryable<Warehouse> allWarehousesQuery = dbContext.Warehouses
                .Where(w => w.WarehouseUsers.Any(uw => uw.ApplicationUserId == userId));

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

            inputModel.TotalItemsBeforePagination = await allWarehousesQuery.CountAsync();

            if (inputModel.CurrentPage.HasValue &&
                inputModel.EntitiesPerPage.HasValue)
            {
                allWarehousesQuery = allWarehousesQuery
                    .Skip(inputModel.EntitiesPerPage.Value * (inputModel.CurrentPage.Value - 1))
                    .Take(inputModel.EntitiesPerPage.Value);
            }

            return await allWarehousesQuery.Select(w => new WarehouseCardViewModel
            {
                Id = w.Id.ToString(),
                Name = w.Name,
                Address = w.Address,
                CreatedDate = w.CreatedDate.ToString(DateFormat),
                Size = w.Size.ToString(),
            })
                .OrderBy(w => w.Name)
                .ToArrayAsync();
        }

        public async Task<OperationResult<Guid>> CreateWarehouseAsync(CreateWarehouseInputModel inputModel, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return OperationResult<Guid>.Failure(UserNotFound);

            bool warehouseExists = await dbContext.UsersWarehouses
                .AnyAsync(uw => uw.ApplicationUserId == userId && uw.Warehouse.Name == inputModel.Name);

            if (warehouseExists)
                return OperationResult<Guid>.Failure(WarehouseDuplicateName);

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

            return OperationResult<Guid>.Ok();
        }
    }
}
