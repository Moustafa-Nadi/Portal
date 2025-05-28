using Mnf_Portal.Core.Entities;
using System.Linq.Expressions;

namespace Mnf_Portal.Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> Criteria = null!, bool tracked = true, int pageSize = 0, int pageNumber = 0,
       params Expression<Func<T, object>>[] Includes);

        Task<T?> GetByIdAsync(
            Expression<Func<T, bool>> Criteria = null!,
            bool tracked = true,
            params Expression<Func<T, object>>[] Includes);

        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
        Task SaveAsync();
    }
}
