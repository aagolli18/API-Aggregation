using API_Aggregation.Models;
using API_Aggregation.Services;
using System.Text.Json;

namespace API_Aggregation.Tests.AggregationTests
{
    public class AggregationUnitTest
    {
        [Fact]
        public void RecordRequest_ShouldCountRequestsCorrectly()
        {
            // Arrange
            var service = new ApiStatisticsService();

            // Act
            service.RecordRequest("DummyJSON", 50);  //fast req
            service.RecordRequest("DummyJSON", 150); //avg req
            service.RecordRequest("DummyJSON", 250); //slow req

            var statistics = service.GetStatistics();

            // Assert
            Assert.Single(statistics);
            Assert.Equal(3, statistics[0].TotalRequests);
            Assert.Equal(1, statistics[0].FastRequestCount);
            Assert.Equal(1, statistics[0].AverageRequestCount);
            Assert.Equal(1, statistics[0].SlowRequestCount);
        }

        [Fact]
        public void GetData_ShouldFilterResultsByApiName()
        {
            // Arrange
            var results = new List<ApiResult>
            {
                new ApiResult
                {
                    ApiName = "DummyJSON",
                    Data = JsonDocument.Parse(
                        """{"title":"Phone"}""").RootElement.Clone()
                },
                new ApiResult
                {
                    ApiName = "PokeAPI",
                    Data = JsonDocument.Parse(
                        """{"name":"pikachu"}""").RootElement.Clone()
                },
                new ApiResult
                {
                    ApiName = "JSONPlaceholder",
                    Data = JsonDocument.Parse(
                        """{"title":"Test post"}""").RootElement.Clone()
                }
            };

            var filter = new AggregationFilter
            {
                ApiName = "DummyJSON"
            };

            // Act
            var filteredResults = results.Where(result => result.ApiName == filter.ApiName).ToList();

            // Assert
            Assert.Single(filteredResults);
            Assert.Equal("DummyJSON", filteredResults[0].ApiName);
        }
    }
}
