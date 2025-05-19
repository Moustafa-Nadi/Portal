using AutoMapper;
using Mnf_Portal.APIs.DTOs;
using Mnf_Portal.Core.Entities;

namespace Mnf_Portal.APIs.Helpers
{
    public class GallariesUrlResolver : IValueResolver<PortalNews, NewsDto, IReadOnlyList<string>>
    {
        private readonly IConfiguration _configuration;

        public GallariesUrlResolver(IConfiguration configuration) => _configuration = configuration;
        public IReadOnlyList<string> Resolve(PortalNews source, NewsDto destination, IReadOnlyList<string> destMember, ResolutionContext context)
        {
            return source.Gallaries?
            .Select(g => $"{_configuration["ApiBaseUrl"]}{g.ImageUrl}")
            .ToList()
            .AsReadOnly() ?? new List<string>().AsReadOnly();
        }
    }
}
