using System.Linq.Expressions;

namespace Mnf_Portal.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(bool tracked = true, params Expression<Func<T, object>>[] Includes);
        Task<T?> GetByIdAsync(Expression<Func<T, bool>> Criteria = null!, bool tracked = true, params Expression<Func<T, object>>[] Includes);
    }
}
