using AutoMapper;
using Mnf_Portal.Core.DTOs;
using Mnf_Portal.Core.Entities;

namespace Mnf_Portal.APIs.Helpers
{
    public class ImageUrlResolver : IValueResolver<PortalNews, NewsDto, string>
    {
        private readonly IConfiguration _configuration;

        public ImageUrlResolver(IConfiguration configuration) => _configuration = configuration;

        public string Resolve(PortalNews source, NewsDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Image))
            {
                return $"{_configuration["ApiBaseUrl"]}Images/{source.Image}";
            }
            return string.Empty;
        }
    }
}
