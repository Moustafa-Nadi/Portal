using Mnf_Portal.Core.Entities;
using System.Linq.Expressions;

namespace Mnf_Portal.Core.Interfaces
{
    public interface IMnfContextRepo<T> : IGenericRepository<T> where T : BaseEntity
    {

        Task<int> GetCountAsync(Expression<Func<T, bool>> Criteria = null!, bool tracked = true);
    }
}
