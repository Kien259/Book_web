using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();  
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }
        public IEnumerable<T> GetAllAsync(Expression<Func<T, bool>>? filter=null, string? includeProperties = "Category, CoverType")
        {
            IQueryable<T> query = dbSet;
            if(filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return  query.ToList(); 
        }

		public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true)
        {
            IQueryable<T> query;

            if (tracked)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }


            query = query.Where(filter);

            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return await Task.Run(() => query.FirstOrDefault());
        }

        public async Task RemoveAsync(T entity)
        {
            await Task.Run(() => dbSet.Remove(entity));
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entity)
        {
            await Task.Run(() => dbSet.RemoveRange(entity));
        }
    }
}
