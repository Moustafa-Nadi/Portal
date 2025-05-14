namespace Mnf_Portal.Core.Entities
{
    public class PortalNews
    {
        public int News_Id { get; set; }   // PK

        public DateTime Date { get; set; }

        public Guid OwnerId { get; set; }

        public string Image { get; set; }

        public bool IsFeatured { get; set; }

        public ICollection<NewsTranslation?> Translations { get; set; } = [];

        public ICollection<NewsGallary> Gallaries { get; set; }
    }
}
