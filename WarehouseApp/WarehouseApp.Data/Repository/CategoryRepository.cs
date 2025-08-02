using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository;

namespace WarehouseApp.Data.Repositories
{
    public class CategoryRepository : BaseRepository<Category, Guid>, ICategoryRepository
    {
        public CategoryRepository(WarehouseDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
