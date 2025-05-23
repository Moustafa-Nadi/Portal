using Mnf_Portal.Core.Entities;
using Mnf_Portal.Core.Interfaces;
using Mnf_Portal.Core.Specification;

namespace Mnf_Portal.Services
{
    public class NewsService : INewsService
    {
        private readonly IGenericRepository<PortalNews> _newsRepo;

        public NewsService(IGenericRepository<PortalNews> newsRepo) { _newsRepo = newsRepo; }

        public async Task<IEnumerable<PortalNews>> GetAllNews(NewsParams newsParams)
        {
            var news = await _newsRepo.GetAllAsync(
                N => (!newsParams.DateTime1.HasValue || N.Date.Date >= newsParams.DateTime1.Value.Date) &&
                    (!newsParams.DateTime2.HasValue || N.Date.Date <= newsParams.DateTime2.Value.Date) &&
                    (string.IsNullOrEmpty(newsParams.Search) ||
                        N.Translations.Any(T => T.Header.ToLower().Contains(newsParams.Search.ToLower()))),

                tracked: false,
                newsParams.PageSize,
                newsParams.PageIndex,
                n => n.Translations,
                n => n.Gallaries);

            return news;
        }

        public async Task<PortalNews> GetNewsById(int id)
        {
            var news = await _newsRepo.GetByIdAsync(
                n => n.News_Id == id,
                tracked: false,
                n => n.Translations,
                n => n.Gallaries);
            return news;
        }
    }
}
