namespace Mnf_Portal.Core.Specification
{
    public class NewsParams
    {
        const int MaxPageSize = 10;
        private int pageSize = 5;

        public int PageSize { get => pageSize; set { pageSize = value > MaxPageSize ? MaxPageSize : value; } }

        public int PageIndex { get; set; } = 1;

        private string? search;

        public string? Search { get => search; set => search = value.Trim().ToLower(); }

        public DateTime? InitialDate { get; set; }

        public DateTime? FinalDate { get; set; }
    }
}
