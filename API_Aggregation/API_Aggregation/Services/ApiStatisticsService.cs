using API_Aggregation.Interfaces;
using API_Aggregation.Models;

namespace API_Aggregation.Services
{
    public class ApiStatisticsService : IApiStatisticsService
    {
        private readonly Dictionary<string, ApiRequestStatistics> _statistics = new();
        private readonly object _lock = new();


        public void RecordRequest(string apiName, double responseTimeMs)
        {
            lock (_lock)
            {
                if (!_statistics.ContainsKey(apiName))
                {
                    _statistics[apiName] = new ApiRequestStatistics
                    {
                        ApiName = apiName
                    };
                }

                var statistics = _statistics[apiName];

                statistics.TotalRequests++;

                statistics.AverageResponseTimeMs = ((statistics.AverageResponseTimeMs * (statistics.TotalRequests - 1)) + responseTimeMs) / statistics.TotalRequests;

                if (responseTimeMs < 100)
                {
                    statistics.FastRequestCount++;
                }
                else if (responseTimeMs <= 200)
                {
                    statistics.AverageRequestCount++;
                }
                else
                {
                    statistics.SlowRequestCount++;
                }
            }
        }


        public List<ApiRequestStatistics> GetStatistics()
        {
            lock (_lock)
            {
                return _statistics.Values
                    .Select(statistics => new ApiRequestStatistics
                    {
                        ApiName = statistics.ApiName,
                        TotalRequests = statistics.TotalRequests,
                        AverageResponseTimeMs = statistics.AverageResponseTimeMs,
                        FastRequestCount = statistics.FastRequestCount,
                        AverageRequestCount = statistics.AverageRequestCount,
                        SlowRequestCount = statistics.SlowRequestCount
                    })
                    .ToList();
            }
        }
    }
}
