namespace Mnf_Portal.Core.Entities
{
    public class PortalNews : BaseEntity
    {
        public DateTime Date { get; set; }

        public Guid OwnerId { get; set; } = Guid.NewGuid();

        public string Image { get; set; }

        public bool IsFeatured { get; set; }

        public ICollection<NewsTranslation> Translations { get; set; } = new List<NewsTranslation>();

        public ICollection<NewsGallary> Gallaries { get; set; } = new List<NewsGallary>();
    }
}
