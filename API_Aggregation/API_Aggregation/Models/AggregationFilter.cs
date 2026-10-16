namespace API_Aggregation.Models
{
    public class AggregationFilter
    {
        public string? ApiName { get; set; }

        public string? SearchTerm { get; set; }

        public string SortBy { get; set; } = "ApiName";

        public bool SortDescending { get; set; }
    }
}
