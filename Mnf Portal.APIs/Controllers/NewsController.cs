using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Mnf_Portal.APIs.DTOs;
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
        readonly IMnfContextRepo<NewsTranslation> _translation;

        public NewsController(INewsService newsSevices, IMapper mapper, IMnfContextRepo<PortalNews> newsRepo, IMnfContextRepo<NewsTranslation> translation)
        {
            _newsSevices = newsSevices;
            _mapper = mapper;
            _newsRepo = newsRepo;
            _translation = translation;
        }



        [HttpGet]
        public async Task<ActionResult<Pagination<NewsDto>>> GetAll([FromQuery] NewsParams newsParams)
        {
            var news = await _newsSevices.GetAllNews(newsParams);

            var data = _mapper.Map<IReadOnlyList<NewsDto>>(news);

            var count = await _newsSevices.GetCount(newsParams);
            //var count = await _newsRepo.GetCountAsync();
            // return Ok(newsDto);
            return Ok(new Pagination<NewsDto>(newsParams.PageIndex, newsParams.PageSize, count, data));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<NewsDto>> GetById(int id, int langId)
        {
            var news = await _newsSevices.GetNewsByIdWithSpecificLang(id, langId);

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
        public async Task<ActionResult<CreateNewsDto>> CreateNews([FromForm] CreateNewsDto newsDto)
        {
            if (newsDto is null)
                return BadRequest(new ApiResponse(404, "News data is required"));

            if (newsDto.Image is null || newsDto.Image.Length == 0)
                return BadRequest(new ApiResponse(404, "main image is required"));

            if (newsDto.Translations is null || newsDto.Translations.Count == 0)
                return BadRequest(new ApiResponse(400, "Translations is required"));

            string uniqueFileName = await UploadToImages(newsDto.Image);
            ICollection<string> gallary = await UploadToGallery(newsDto.Gallary);

            var news = _mapper.Map<PortalNews>(newsDto);
            news.Image = uniqueFileName;

            foreach (var image in gallary)
            {
                var newsGallary = new NewsGallary()
                {
                    ImageUrl = image
                };

                news.Gallaries.Add(newsGallary);
            }

            await _newsRepo.CreateAsync(news);

            return Ok(newsDto);
        }
        async Task<string> UploadToImages(IFormFile image)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filepath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return uniqueFileName;
        }
        async Task<ICollection<string>> UploadToGallery(ICollection<IFormFile> gallary)
        {
            ICollection<string> images = new List<string>();
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Gallary");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            foreach (var image in gallary)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                images.Add(uniqueFileName);
            }

            return images;
        }


        [HttpPut("{id}")]// PUT : api/News/{id}// UpdateNews
        public async Task<ActionResult<PortalNews>> UpdateNews(int id, int langId, [FromBody] UpdateNewsDto newsDto)
        {
            if (newsDto is null)
                return BadRequest(new ApiResponse(400));

            var oldNews = await _newsSevices.GetNewsByIdWithSpecificLang(id, langId);

            if (oldNews is null)
                return NotFound(new ApiResponse(404, "Resource Not Found"));

            _mapper.Map(newsDto, oldNews);
            oldNews.Date = DateTime.Now;

            var currentTranslations = await _translation.GetAllAsync(t => t.NewsId == oldNews.Id && t.LanguageId == langId);

            oldNews.Translations.Clear();

            foreach (var item in currentTranslations)
            {
                await _translation.RemoveAsync(item);
            }

            foreach (var translationDto in newsDto.Translations)
            {
                var translation = _mapper.Map<NewsTranslation>(translationDto);
                translation.NewsId = oldNews.Id;
                oldNews.Translations.Add(translation);
            }

            //foreach (var translationDto in newsDto.Translations)
            //{
            //    var existing = currentTranslations.FirstOrDefault(t => t.LanguageId == translationDto.LanguageId);
            //    _mapper.Map(translationDto, existing);
            //}

            //await _newsRepo.SaveAsync();

            await _newsRepo.UpdateAsync(oldNews);

            return Ok(oldNews);
        }
    }
}
