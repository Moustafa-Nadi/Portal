using AutoMapper;
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

                //.ForMember(
                //    dest => dest.Header,
                //    opt => opt.MapFrom(src => src.Translations.Select(T => T!.Header).FirstOrDefault()))

                //.ForMember(
                //    dest => dest.Abbreviation,
                //    opt => opt.MapFrom(src => src.Translations.Select(T => T!.Abbreviation).FirstOrDefault()))

                //.ForMember(
                //    dest => dest.Body,
                //    opt => opt.MapFrom(src => src.Translations.Select(T => T!.Body).FirstOrDefault()))

                //.ForMember(
                //    dest => dest.Source,
                //    opt => opt.MapFrom(src => src.Translations.Select(T => T!.Source).FirstOrDefault()))

                //.ForMember(
                //    dest => dest.LanguageId,
                //    opt => opt.MapFrom(src => src.Translations.Select(T => T!.LanguageId).FirstOrDefault()));

            CreateMap<NewsDto, PortalNews>()
            .ForMember(dest => dest.Gallaries, opt => opt.MapFrom(src =>
                src.Gallaries.Select(url => new NewsGallary { ImageUrl = url }).ToList()
            ));
        }
    }
}
