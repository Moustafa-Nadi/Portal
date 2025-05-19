using System.Text.Json.Serialization;

namespace Mnf_Portal.Core.Entities
{
    public class NewsTranslation
    {
        public int Id { get; set; }

        public string Header { get; set; }

        public string Abbreviation { get; set; }

        public string Body { get; set; }

        public string Source { get; set; }

        public int LanguageId { get; set; }

        public int? NewsId { get; set; }
        [JsonIgnore]
        public PortalNews News { get; set; }
    }
}
