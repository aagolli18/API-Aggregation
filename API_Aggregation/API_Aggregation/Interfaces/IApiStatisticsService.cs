using API_Aggregation.Models;

namespace API_Aggregation.Interfaces
{
    public interface IApiStatisticsService
    {
        void RecordRequest(string apiName, double responseTimeMs);

        List<ApiRequestStatistics> GetStatistics();
    }
}
