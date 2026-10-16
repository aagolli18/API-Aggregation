using API_Aggregation.Interfaces;
using API_Aggregation.Models;
using System.Text.Json;

namespace API_Aggregation.Providers
{
    public class DummyJsonApiProvider : IApiProvider
    {
        private readonly HttpClient _httpClient;

        public DummyJsonApiProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string ApiName => "DummyJSON";

        public async Task<List<ApiResult>> GetDataAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync("products", cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var products = document.RootElement.GetProperty("products");

            var results = new List<ApiResult>();

            foreach (var product in products.EnumerateArray())
            {
                results.Add(new ApiResult
                {
                    ApiName = ApiName,
                    Data = product.Clone()
                });
            }

            return results;
        }
    }
}
