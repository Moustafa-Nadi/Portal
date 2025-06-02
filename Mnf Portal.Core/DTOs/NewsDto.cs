using Mnf_Portal.Core.Entities;

namespace Mnf_Portal.Core.DTOs
{
    public class NewsDto
    {
        public int NewsId { get; set; }   // PK

        public DateTime Date { get; set; }

        public Guid OwnerId { get; set; }

        public string Image { get; set; }

        public bool IsFeatured { get; set; }

        public IReadOnlyList<string> Gallaries { get; set; }
        public IReadOnlyList<NewsTranslation> Translations { get; set; } = [];

        //public int LanguageId { get; set; } = 1;

        //public string Header { get; set; }

        //public string Abbreviation { get; set; }

        //public string Body { get; set; }

        //public string Source { get; set; }
    }
}
