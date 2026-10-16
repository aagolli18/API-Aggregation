
using System.Text.Json;
using API_Aggregation.Interfaces;
using API_Aggregation.Models;

namespace API_Aggregation.Providers;

public class JsonPlaceholderApiProvider : IApiProvider
{
    private readonly HttpClient _httpClient;

    public JsonPlaceholderApiProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string ApiName => "JSONPlaceholder";

    public async Task<List<ApiResult>> GetDataAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync("posts", cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        var posts = document.RootElement;

        var results = new List<ApiResult>();

        foreach (var post in posts.EnumerateArray())
        {
            results.Add(new ApiResult
            {
                ApiName = ApiName,
                Data = post.Clone()
            });
        }

        return results;
    }
}
