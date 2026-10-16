
using System.Text.Json;
using API_Aggregation.Interfaces;
using API_Aggregation.Models;

namespace API_Aggregation.Providers;

public class PokeApiProvider : IApiProvider
{
    private readonly HttpClient _httpClient;

    public PokeApiProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string ApiName => "PokeAPI";

    public async Task<List<ApiResult>> GetDataAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync("pokemon?limit=20", cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        var pokemonList = document.RootElement.GetProperty("results");

        var results = new List<ApiResult>();

        foreach (var pokemon in pokemonList.EnumerateArray())
        {
            results.Add(new ApiResult
            {
                ApiName = ApiName,
                Data = pokemon.Clone()
            });
        }

        return results;
    }
}
