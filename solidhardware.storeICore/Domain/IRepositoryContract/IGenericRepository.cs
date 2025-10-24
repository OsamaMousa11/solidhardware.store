﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Domain.IRepositoryContract
{
    public interface IGenericRepository<T> 
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string includeProperties = "", Expression<Func<T, object>>? orderBy = null, int pageIndex = 1, int pageSize = 10);
        Task<T> GetByAsync(Expression<Func<T, bool>>? filter = null, bool isTracked = true, string includeProperties = "");
        Task<T> CreateAsync(T model);
        Task<int> CountAsync(Expression<Func<T, bool>> filter);
        Task AddRangeAsync(IEnumerable<T> model);
        Task RemoveRangeAsync(IEnumerable<T> model);
        Task<bool> DeleteAsync(T model);
        void Detach(T entity);
        Task SaveAsync();

    }
}
