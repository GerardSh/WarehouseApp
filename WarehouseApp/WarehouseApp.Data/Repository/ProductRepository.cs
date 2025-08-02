using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository;

namespace WarehouseApp.Data.Repositories
{
    public class ProductRepository : BaseRepository<Product, Guid>, IProductRepository
    {
        public ProductRepository(WarehouseDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
