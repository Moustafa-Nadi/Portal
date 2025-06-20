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
                .ForMember(
                    dest => dest.Gallaries,
                    opt => opt.MapFrom(src => src.Gallaries.Select(g => g.ImageUrl).ToList()))

                .ForMember(d => d.Image, O => O.MapFrom<ImageUrlResolver>())

                .ForMember(dest => dest.Gallaries, opt => opt.MapFrom<GallariesUrlResolver>())

                .ForMember(dest => dest.NewsId, opt => opt.MapFrom(src => src.Id))

                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToShortDateString()))

                .ForMember(dest => dest.Translations, opt => opt.MapFrom(ser => ser.Translations));


            CreateMap<GallaryDto, NewsGallary>().ReverseMap();
            CreateMap<TranslationDto, NewsTranslation>().ReverseMap();
            CreateMap<CreateNewsDto, PortalNews>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.IsFeatured, opt => opt.MapFrom(src => src.IsFeatured))

                .ForMember(dest => dest.Gallaries, opt => opt.MapFrom(src => src.Gallaries))
                .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations));
        }
    }
}
