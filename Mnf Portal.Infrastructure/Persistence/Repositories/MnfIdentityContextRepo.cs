using Microsoft.EntityFrameworkCore;
using Mnf_Portal.Core.Entities;
using Mnf_Portal.Core.Interfaces;
using Mnf_Portal.Infrastructure.Identity;
using System.Linq.Expressions;

namespace Mnf_Portal.Infrastructure.Persistence.Repositories
{
    public class MnfIdentityContextRepo<T> : IMnfIdentityContextRepo<T> where T : BaseEntity
    {
        private readonly MnfIdentityDbContext _context;
        private readonly DbSet<T> _dbSet;

        public MnfIdentityContextRepo(MnfIdentityDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
            await SaveAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await SaveAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> Criteria = null!,
            bool tracked = true,
            int pageSize = 0,
            int pageNumber = 0,
            params Expression<Func<T, object>>[] Includes)
        {
            var query = _dbSet.AsQueryable();

            if (Criteria is { })
                query = query.Where(Criteria);

            if (Includes is { Length: > 0 })
            {
                foreach (var include in Includes)
                {
                    query = query.Include(include);
                }
            }

            query = query.AsSplitQuery();

            if (!tracked)
                query = query.AsNoTracking();


            if (pageSize > 0 && pageNumber > 0)
                query = query.OrderBy(e => e.Id).Skip(pageSize * (pageNumber - 1)).Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(
            Expression<Func<T, bool>> Criteria = null!,
            bool tracked = true,
            params Expression<Func<T, object>>[] Includes)
        {
            var query = _dbSet.AsQueryable();

            if (Criteria is { })
                query = query.Where(Criteria);

            if (Includes is { Length: > 0 })
            {
                foreach (var include in Includes)
                {
                    query = query.Include(include);
                }
            }
            query = query.AsSplitQuery();
            if (!tracked)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            _context.Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();

    }
}
