using HMS.Core.Contracts;
using HMS.Core.Entities;
using HMS.Infrastructure.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HotelDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        public UnitOfWork(HotelDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Dispose()
        {
           _dbContext.Dispose();
        }

        public IGenaricRepo<T, TKey> GetRepository<T, TKey>() where T : BaseEntity<TKey>
        {
            var type = typeof(T);
            if ( _repositories.TryGetValue(type, out var repository))
            {
                return (IGenaricRepo<T, TKey>)repository;
            }
            var newRepository = new GenaricRepo<T, TKey>(_dbContext);
            _repositories[type] = newRepository;
            return newRepository;

        }

        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}
