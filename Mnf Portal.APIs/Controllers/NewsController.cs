using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Mnf_Portal.APIs.DTOs;
using Mnf_Portal.Core.Entities;
using Mnf_Portal.Core.Interfaces;

namespace Mnf_Portal.APIs.Controllers
{
    public class NewsController : ApiBaseController
    {
        private readonly IGenericRepository<PortalNews> _newsRepo;
        private readonly IMapper _mapper;

        public NewsController(IGenericRepository<PortalNews> newsRepo, IMapper mapper)
        {
            _newsRepo = newsRepo;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsDto>>> GetAll()
        {
            var news = await _newsRepo.GetAllAsync(

                tracked: false,
                n => n.Translations,
                n => n.Gallaries);
            var newsDto = _mapper.Map<IEnumerable<NewsDto>>(news);
            return Ok(newsDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NewsDto>> GetById(int id)
        {
            var news = await _newsRepo.GetByIdAsync(
                n => n.News_Id == id,
                tracked: false,
                n => n.Translations,
                n => n.Gallaries);
            var newsDto = _mapper.Map<NewsDto>(news);
            return Ok(newsDto);
        }
    }
}
