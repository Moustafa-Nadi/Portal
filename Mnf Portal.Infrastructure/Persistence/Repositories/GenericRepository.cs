using Microsoft.EntityFrameworkCore;
using Mnf_Portal.Core.Interfaces;
using System.Linq.Expressions;

namespace Mnf_Portal.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly MnfDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(MnfDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAllAsync(bool tracked = true, params Expression<Func<T, object>>[] Includes)
        {
            var query = _dbSet.AsQueryable();

            query = Includes.Aggregate(query, (current, next) => current.Include(next));

            query = query.AsSplitQuery();

            if (!tracked)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Expression<Func<T, bool>> Criteria = null!, bool tracked = true, params Expression<Func<T, object>>[] Includes)
        {
            var query = _dbSet.AsQueryable();

            if (Criteria is { })
                query = query.Where(Criteria);

            if (Includes is { } && Includes.Length > 0)
            {
                foreach (var include in Includes)
                {
                    query = query.Include(include);
                }
            }
            if (!tracked)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }
    }
}
