namespace Mnf_Portal.Core.Entities
{
    public class NewsGallary
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public int NewsId { get; set; }
        public PortalNews PortalNews { get; set; }
    }
}
