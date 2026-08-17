using HMS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HMS.Core.Contracts
{
    public interface IGenaricRepo <T, TKey> where T : BaseEntity<TKey> 
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter ,Expression<Func<T, object>>? orderBy = null ,Expression<Func<T, object>>? OrderByDesc = null) ;
        Task<T?> GetByIdAsync(TKey id);
        Task<T?> GetByIdAsync(TKey id ,Expression<Func<T, bool>>? filter = null,List< Expression<Func<T, object>>>? Includes = null);
        Task AddAsync(T entity) ;
        void UpdateAsync(T entity) ;
        void DeleteAsync(T entity) ;
    }
}
