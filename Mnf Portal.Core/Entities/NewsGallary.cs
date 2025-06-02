using System.Text.Json.Serialization;

namespace Mnf_Portal.Core.Entities
{
    public class NewsGallary : BaseEntity
    {
        public string ImageUrl { get; set; }

        public int NewsId { get; set; }
        [JsonIgnore]
        public PortalNews PortalNews { get; set; }
    }
}
