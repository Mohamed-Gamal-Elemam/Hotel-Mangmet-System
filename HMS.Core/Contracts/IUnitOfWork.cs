using HMS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Core.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenaricRepo<T, TKey> GetRepository<T, TKey>() where T : BaseEntity<TKey>;
        public Task<int> SaveChangesAsync();
        
    }
}
