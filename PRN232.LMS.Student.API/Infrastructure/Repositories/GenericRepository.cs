using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRN232.LMS.Student.API.Infrastructure.Data;

namespace PRN232.LMS.Student.API.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly StudentDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(StudentDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await GetByIdAsync(id);
            if (existingEntity != null)
            {
                _context.Remove(existingEntity);
            }
        }

        public IQueryable<T> GetQueryable() => _dbSet.AsQueryable();
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return entity;
        }
    }
}