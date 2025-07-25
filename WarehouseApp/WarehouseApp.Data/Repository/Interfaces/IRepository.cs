﻿using System.Linq.Expressions;

namespace WarehouseApp.Data.Repository.Interfaces
{
    public interface IRepository<TEntity, TId>
        where TEntity : class
    {
        IQueryable<TEntity> All();

        Task<TEntity?> GetByIdAsync(TId id);

        Task<TEntity?> GetByIdAsNoTrackingAsync(TId id);

        Task AddAsync(TEntity entity);

        Task AddRangeAsync(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);

        Task SaveChangesAsync();
    }
}
