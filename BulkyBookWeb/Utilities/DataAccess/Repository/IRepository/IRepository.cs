﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter,string? includeProperties = null, bool tracked = true);
        IEnumerable<T> GetAllAsync(Expression<Func<T, bool>>? filter=null, string? includeProperties = null);
		Task AddAsync(T entity);
        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entity);
    
    }
}
