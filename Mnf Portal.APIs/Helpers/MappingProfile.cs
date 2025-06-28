using AutoMapper;
using Mnf_Portal.APIs.DTOs;
using Mnf_Portal.Core.DTOs;
using Mnf_Portal.Core.Entities;

namespace Mnf_Portal.APIs.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PortalNews, NewsDto>()
               .ForMember(d => d.Image, O => O.MapFrom<ImageUrlResolver>())

               .ForMember(dest => dest.Gallaries, opt => opt.MapFrom<GallariesUrlResolver>())

               .ForMember(dest => dest.NewsId, opt => opt.MapFrom(src => src.Id))

               .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))

               .ForMember(dest => dest.Translations, opt => opt.MapFrom(ser => ser.Translations));


            CreateMap<GallaryDto, NewsGallary>().ReverseMap();
            CreateMap<TranslationDto, NewsTranslation>().ReverseMap();

            CreateMap<CreateNewsDto, PortalNews>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(dest => dest.IsFeatured, opt => opt.MapFrom(src => src.IsFeatured))
                .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations));
        }
    }
}
