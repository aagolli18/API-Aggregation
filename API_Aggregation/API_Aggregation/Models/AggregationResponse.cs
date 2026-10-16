namespace API_Aggregation.Models
{
    public class AggregationResponse
    {
        public List<ApiResult> Results { get; set; } = new();

        public int SuccessfulProviders { get; set; }

        public int FailedProviders { get; set; }
    }
}
