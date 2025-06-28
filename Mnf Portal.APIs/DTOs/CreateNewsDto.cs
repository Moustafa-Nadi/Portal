using Mnf_Portal.Core.DTOs;

namespace Mnf_Portal.APIs.DTOs
{
    public class CreateNewsDto
    {
        public DateTime Date { get; set; }
        public Guid OwnerId { get; set; } = Guid.NewGuid();
        public IFormFile Image { get; set; }

        public bool IsFeatured { get; set; }

        public ICollection<TranslationDto> Translations { get; set; }
        public ICollection<IFormFile> Gallary { get; set; }
    }
}
