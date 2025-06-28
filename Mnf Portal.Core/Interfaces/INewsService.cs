using Mnf_Portal.Core.Entities;
using Mnf_Portal.Core.Specification;

namespace Mnf_Portal.Core.Interfaces
{
    public interface INewsService
    {
        Task<IEnumerable<PortalNews>> GetAllNews(NewsParams newsParams);
        Task<PortalNews> GetNewsById(int id);

        Task<int> GetCount(NewsParams newsParams);

        public Task<PortalNews> GetNewsByIdWithSpecificLang(int id, int langId);
    }
}
