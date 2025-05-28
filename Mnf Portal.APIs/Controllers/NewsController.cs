using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Mnf_Portal.APIs.Errors;
using Mnf_Portal.APIs.Helpers;
using Mnf_Portal.Core.DTOs;
using Mnf_Portal.Core.Entities;
using Mnf_Portal.Core.Interfaces;
using Mnf_Portal.Core.Specification;

namespace Mnf_Portal.APIs.Controllers
{
    public class NewsController : ApiBaseController
    {
        private readonly INewsService _newsSevices;
        private readonly IMapper _mapper;
        private readonly IMnfContextRepo<PortalNews> _newsRepo;

        public NewsController(INewsService newsSevices, IMapper mapper, IMnfContextRepo<PortalNews> newsRepo)
        {
            _newsSevices = newsSevices;
            _mapper = mapper;
            _newsRepo = newsRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Pagination<NewsDto>>>> GetAll([FromQuery] NewsParams newsParams)
        {
            var news = await _newsSevices.GetAllNews(newsParams);


            var newsDto = _mapper.Map<IReadOnlyList<NewsDto>>(news);
            return Ok(new Pagination<NewsDto>(newsParams.PageIndex, newsParams.PageSize, newsDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NewsDto>> GetById(int id)
        {
            var news = await _newsSevices.GetNewsById(id);
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
            var removedNews = await _newsSevices.GetNewsById(id);
            if (removedNews is null)
                return NotFound(new ApiResponse(404, "Resource Not Found"));

            await _newsRepo.RemoveAsync(removedNews);
            return Ok(true);
        }

        [HttpPost]  // POST : api/News 
        public async Task<IActionResult> CreateNews([FromBody] NewsDto newsDto)
        {
            if (newsDto == null)
                return BadRequest("News data is required");

            var news = _mapper.Map<PortalNews>(newsDto);
            news.Date = DateTime.UtcNow;

            await _newsRepo.CreateAsync(news);
            await _newsRepo.SaveAsync();

            return CreatedAtAction(nameof(GetById), new { id = news.Id }, news);
        }

        [HttpPut("{id}")]// PUT : api/News/{id}// UpdateNews
        public async Task<ActionResult<PortalNews>> UpdateNews(int id, [FromBody] NewsDto newsDto)
        {
            if (newsDto is null)
                return BadRequest(new ApiResponse(400));

            var oldNews = await _newsSevices.GetNewsById(id);
            if (oldNews is null)
                return NotFound(new ApiResponse(404, "Resource Not Found"));

            var updatedDto = _mapper.Map(newsDto, oldNews);

            oldNews.Date = DateTime.Parse(DateTime.Now.ToShortDateString());
            await _newsRepo.UpdateAsync(oldNews);
            await _newsRepo.SaveAsync();

            return Ok(updatedDto);
        }
    }
}
