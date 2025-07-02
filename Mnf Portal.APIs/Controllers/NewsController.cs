using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Mnf_Portal.APIs.DTOs;
using Mnf_Portal.APIs.Errors;
using Mnf_Portal.APIs.Helpers;
using Mnf_Portal.Core.DTOs;
using Mnf_Portal.Core.Entities;
using Mnf_Portal.Core.Interfaces;
using Mnf_Portal.Core.Specification;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace Mnf_Portal.APIs.Controllers
{
    public class NewsController : ApiBaseController
    {
        private readonly INewsService _newsSevices;
        private readonly IMapper _mapper;
        private readonly IMnfContextRepo<PortalNews> _newsRepo;
        readonly IMnfContextRepo<NewsTranslation> _translation;
        readonly IMnfContextRepo<NewsGallary> _Gallary;

        public NewsController(INewsService newsSevices, IMapper mapper, IMnfContextRepo<PortalNews> newsRepo, IMnfContextRepo<NewsTranslation> translation, IMnfContextRepo<NewsGallary> gallary)
        {
            _newsSevices = newsSevices;
            _mapper = mapper;
            _newsRepo = newsRepo;
            _translation = translation;
            _Gallary = gallary;
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
        public async Task<ActionResult<PortalNews>> CreateNews([FromForm] CreateNewsDto newsDto)
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

            return Ok(news);
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
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
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
        public async Task<ActionResult<PortalNews>> UpdateNews(int id, int langId, [FromForm] UpdateNewsDto newsDto)
        {
            if (newsDto is null)
                return BadRequest(new ApiResponse(400));

            var oldNews = await _newsSevices.GetNewsByIdWithSpecificLang(id, langId);

            if (oldNews is null)
                return NotFound(new ApiResponse(404, "Resource Not Found"));

            _mapper.Map(newsDto, oldNews);
            oldNews.Date = DateTime.Now;

            oldNews.Translations.Clear();
            oldNews.Gallaries.Clear();

            await UpdateTranslations(newsDto.Translations, oldNews, langId);
            await UpdateGallary(newsDto.Gallary, oldNews);

            string newImageName = await UpdateRootImage(newsDto.Image, oldNews);
            oldNews.Image = newImageName;
            await _newsRepo.UpdateAsync(oldNews);

            return Ok(oldNews);
        }

        async Task UpdateTranslations(ICollection<TranslationDto> newTranslation, PortalNews updateNews, int langId)
        {
            var currentTranslations = (await _translation.GetAllAsync(t => t.NewsId == updateNews.Id && t.LanguageId == langId)).FirstOrDefault();

            if (currentTranslations is not null)
            {
                await _translation.RemoveAsync(currentTranslations);
            }

            foreach (var item in newTranslation)
            {
                var translation = _mapper.Map<NewsTranslation>(item);
                translation.NewsId = updateNews.Id;
                updateNews.Translations.Add(translation);
            }
        }

        async Task<string> UpdateRootImage(IFormFile image, PortalNews news)
        {
            string oldImage = news.Image;
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", oldImage);

            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }

            string newImage = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var newPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", newImage);

            using (var stream = new FileStream(newPath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return newImage;
        }

        async Task UpdateGallary(ICollection<IFormFile> newGallary, PortalNews updateNews)
        {
            var currentGallary = await _Gallary.GetAllAsync(g => g.NewsId == updateNews.Id);

            foreach (var item in currentGallary)
            {
                await _Gallary.RemoveAsync(item);

                string image = item.ImageUrl;
                var oldpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Gallary", image);
                System.IO.File.Delete(oldpath);
            }

            foreach (var item in newGallary)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Gallary");

                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                var filePath = Path.Combine(uploadPath, imageName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await item.CopyToAsync(stream);
                }

                NewsGallary newsGallary = new()
                {
                    ImageUrl = imageName
                };

                updateNews.Gallaries.Add(newsGallary);
            }
        }
    }
}
