using API_Aggregation.Interfaces;

namespace API_Aggregation.BackgroundServices
{
    public class ApiMonitoringService : BackgroundService
    {
        private readonly IApiStatisticsService _statisticsService;
        private readonly ILogger<ApiMonitoringService> _logger;

        public ApiMonitoringService(IApiStatisticsService statisticsService, ILogger<ApiMonitoringService> logger)
        {
            _statisticsService = statisticsService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var statistics = _statisticsService.GetStatistics();

                foreach (var api in statistics)
                {
                    if (api.TotalRequests > 0 && api.SlowRequestCount * 2 >= api.TotalRequests)
                    {
                        _logger.LogWarning(
                            "API {ApiName} has a high number of slow requests. " +
                            "Slow: {SlowRequests}, Total: {TotalRequests}",
                            api.ApiName,
                            api.SlowRequestCount,
                            api.TotalRequests);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
