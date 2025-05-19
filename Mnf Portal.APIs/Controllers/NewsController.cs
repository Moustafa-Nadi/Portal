using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Mnf_Portal.APIs.DTOs;
using Mnf_Portal.APIs.Errors;
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

            if (news is null)
            {
                return NotFound(new ApiResponse(404, "Resource Not Found"));
            }
            var newsDto = _mapper.Map<NewsDto>(news);
            return Ok(newsDto);
        }


        [HttpDelete]   // DELETE : api/News
        public async Task<ActionResult<bool>> DeleteNews(int id)
        {
            var removedNews = await _newsRepo.GetByIdAsync(n => n.News_Id == id);
            if (removedNews is null)
                return NotFound(new ApiResponse(404, "Resource Not Found"));

            await _newsRepo.RemoveAsync(removedNews);
            return Ok(true);
        }

        [HttpPost]  // POST : api/News  //CreateNews
        public async Task<IActionResult> CreateNews([FromBody] NewsDto newsDto)
        {
            if (newsDto == null)
                return BadRequest("News data is required");

            var news = _mapper.Map<PortalNews>(newsDto);
            news.Date = DateTime.UtcNow;

            await _newsRepo.CreateAsync(news);
            await _newsRepo.SaveAsync();

            return CreatedAtAction(nameof(GetById), new { id = news.News_Id }, news);
        }

        [HttpPut("{id}")]// PUT : api/News/{id}// UpdateNews
        public async Task<IActionResult> UpdateNews(int id, [FromBody] NewsDto newsDto)
        {
            var news = await _newsRepo.GetByIdAsync(n => n.News_Id == id);
            if (news is null)
                return NotFound(new ApiResponse(404, "Resource Not Found"));

            _mapper.Map(newsDto, news);
            await _newsRepo.UpdateAsync(news);
            await _newsRepo.SaveAsync();

            return NoContent();
        }
    }
}
