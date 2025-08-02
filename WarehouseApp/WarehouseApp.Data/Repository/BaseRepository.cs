using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using WarehouseApp.Data.Repository.Interfaces;

namespace WarehouseApp.Data.Repository
{
    public class BaseRepository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : class
    {
        protected readonly WarehouseDbContext dbContext;
        protected readonly DbSet<TEntity> dbSet;

        public BaseRepository(WarehouseDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> All()
        {
            return dbSet.AsNoTracking();
        }

        public IQueryable<TEntity> AllTracked()
        {
            return dbSet;
        }

        public async Task<TEntity?> GetByIdAsync(TId id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<TEntity?> GetByIdAsNoTrackingAsync(TId id)
        {
            return await dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id")!.Equals(id));
        }

        public async Task AddAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }

        public void Update(TEntity entity)
        {
            dbSet.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            dbContext.Set<TEntity>().RemoveRange(entities);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbSet.AnyAsync(predicate);
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public TEntity? GetTrackedLocal(Func<TEntity, bool> predicate)
        {
            return dbContext.ChangeTracker
                .Entries<TEntity>()
                .Select(e => e.Entity)
                .FirstOrDefault(predicate);
        }
    }
}
