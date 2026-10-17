namespace API_Aggregation.Models
{
    public class ApiRequestStatistics
    {
        public string ApiName { get; set; } = string.Empty;

        public long TotalRequests { get; set; }

        public double AverageResponseTimeMs { get; set; }

        public long FastRequestCount { get; set; }

        public long AverageRequestCount { get; set; }

        public long SlowRequestCount { get; set; }
    }
}
