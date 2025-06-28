namespace Mnf_Portal.Core.Specification
{
    public class NewsParams
    {
        public int LangId { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int PageIndex { get; set; } = 1;

        public string? Search { get; set; }

        public DateTime? DateTime1 { get; set; }

        public DateTime? DateTime2 { get; set; }
    }
}
