using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository;

namespace WarehouseApp.Data.Repositories
{
    public class AdminRequestRepository : BaseRepository<AdminRequest, Guid>, IAdminRequestRepository
    {
        public AdminRequestRepository(WarehouseDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
