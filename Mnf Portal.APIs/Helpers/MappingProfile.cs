using AutoMapper;
using Mnf_Portal.APIs.DTOs;
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

                .ForMember(
                    dest => dest.Header,
                    opt => opt.MapFrom(src => src.Translations.Select(T => T!.Header).FirstOrDefault()))
                .ForMember(
                    dest => dest.Abbreviation,
                    opt => opt.MapFrom(src => src.Translations.Select(T => T!.Abbreviation).FirstOrDefault()))
                //.ForMember(dest => dest.NewsId, opt => opt.MapFrom(src => src.News_Id))
                .ForMember(
                    dest => dest.Body,
                    opt => opt.MapFrom(src => src.Translations.Select(T => T!.Body).FirstOrDefault()))
                .ForMember(
                    dest => dest.Source,
                    opt => opt.MapFrom(src => src.Translations.Select(T => T!.Source).FirstOrDefault()))
                .ForMember(
                    dest => dest.LanguageId,
                    opt => opt.MapFrom(src => src.Translations.Select(T => T!.LanguageId).FirstOrDefault())).ReverseMap();

            //        CreateMap<NewsDto, PortalNews>()
            //.ForMember(dest => dest.Gallaries,
            //    opt => opt.MapFrom(src => src.Gallaries.Select(url => new NewsGallary { ImageUrl = url }).ToList())).ForMember(
            //                dest => dest.Translations,
            //                opt => opt.MapFrom(src => new
            //                {
            //                    src.Header,
            //                    src.Body,
            //                    src.Abbreviation,
            //                    src.Source
            //                }));
            //    }
        }
    }
}
