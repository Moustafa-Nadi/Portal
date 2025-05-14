using AutoMapper;
using Mnf_Portal.APIs.DTOs;
using Mnf_Portal.Core.Entities;

namespace Mnf_Portal.APIs.Helpers
{
    public class GallariesUrlResolver : IValueResolver<PortalNews, NewsDto, List<string>>
    {
        private readonly IConfiguration _configuration;

        public GallariesUrlResolver(IConfiguration configuration) => _configuration = configuration;
        public List<string> Resolve(
            PortalNews source,
            NewsDto destination,
            List<string> destMember,
            ResolutionContext context)
        { return source.Gallaries.Select(g => $"{_configuration["ApiBaseUrl"]}{g.ImageUrl}").ToList(); }
    }
}
