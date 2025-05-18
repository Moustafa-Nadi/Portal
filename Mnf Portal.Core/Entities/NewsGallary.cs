using System.Text.Json.Serialization;

namespace Mnf_Portal.Core.Entities
{
    public class NewsGallary
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public int NewsId { get; set; }
        [JsonIgnore]
        public PortalNews PortalNews { get; set; }
    }
}
