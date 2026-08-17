using HMS.Core.Contracts;
using HMS.Core.Entities;
using HMS.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HMS.Infrastructure.Data.Repository
{
    public class GenaricRepo<T, TKey> : IGenaricRepo<T, TKey> where T : BaseEntity<TKey>
    {
        private readonly HotelDbContext _dbContext;

        public GenaricRepo(HotelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(T entity)
        {
             await _dbContext.Set<T>().AddAsync(entity);
        }

        public void DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }
        

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter, Expression<Func<T, object>>? orderBy = null, Expression<Func<T, object>>? orderByDesc = null)
        {
            var query = _dbContext.Set<T>().AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }
            if (orderByDesc != null)
            {
                query = query.OrderByDescending(orderByDesc);
            }
            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(TKey id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetByIdAsync(TKey id, Expression<Func<T, bool>>? filter = null,List <Expression<Func<T, object>>>? Includes = null)
        {
            var query = _dbContext.Set<T>().AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (Includes is not null)
            {
                foreach (var include in Includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(q=>q.Id!.Equals(id));
        }

        public void UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }
    }
}
