namespace Mnf_Portal.Core.Entities
{
    public class PortalNews : BaseEntity
    {
        public DateTime Date { get; set; }

        public Guid OwnerId { get; set; }

        public string Image { get; set; }

        public bool IsFeatured { get; set; }

        public ICollection<NewsTranslation?> Translations { get; set; } = [];

        public ICollection<NewsGallary> Gallaries { get; set; }
    }
}
