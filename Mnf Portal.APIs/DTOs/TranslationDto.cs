namespace Mnf_Portal.Core.DTOs
{
    public class TranslationDto
    {
        public int Id { get; set; }

        public string Header { get; set; }

        public string Abbreviation { get; set; }

        public string Body { get; set; }

        public string Source { get; set; }

        public int LanguageId { get; set; }
    }
}