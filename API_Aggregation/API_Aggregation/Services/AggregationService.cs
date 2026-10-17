using API_Aggregation.Interfaces;
using API_Aggregation.Models;
using Microsoft.Extensions.Caching.Memory;

namespace API_Aggregation.Services;

public class AggregationService
{
    private readonly IEnumerable<IApiProvider> _providers;
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;
    public AggregationService(IEnumerable<IApiProvider> providers, IMemoryCache memoryCache, IConfiguration configuration)
    {
        _providers = providers;
        _memoryCache = memoryCache;
        _configuration = configuration;
    }


    public async Task<AggregationResponse> GetAllDataAsync(AggregationFilter? filter = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = _configuration["CacheSettings:AggregationCacheKey"] ?? "aggregated-api-data";

        int durationInMinutes = _configuration.GetValue<int?>("CacheSettings:DurationInMinutes") ?? 5;
      
        var cachedResponse = _memoryCache.Get<AggregationResponse>(cacheKey);

        //Return cached data if available
        if (cachedResponse != null)
        {
            return new AggregationResponse
            {
                Results = ApplyFiltersAndSorting(cachedResponse.Results, filter),
                SuccessfulProviders = cachedResponse.SuccessfulProviders,
                FailedProviders = cachedResponse.FailedProviders
            };
        }

        var providers = _providers.ToList();

        var tasks = new List<Task<(bool Success, List<ApiResult> Results)>>();

        foreach (var provider in providers)
        {
            tasks.Add(ExecuteProviderAsync(provider, cancellationToken));
        }

        //Wait for all API requests to finish
        var outcomes = await Task.WhenAll(tasks);

        //Combine results from all providers
        var combinedResults = outcomes.SelectMany(outcome => outcome.Results).ToList();

        //Create the response with unfiltered data
        var response = new AggregationResponse
        {
            Results = combinedResults,
            SuccessfulProviders = outcomes.Count(outcome => outcome.Success),
            FailedProviders = outcomes.Count(outcome => !outcome.Success)
        };

        // Store the response in cache
        _memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(durationInMinutes));

        return new AggregationResponse
        {
            Results = ApplyFiltersAndSorting(response.Results, filter),
            SuccessfulProviders = response.SuccessfulProviders,
            FailedProviders = response.FailedProviders
        };
    }


    private async Task<(bool Success, List<ApiResult> Results)> ExecuteProviderAsync(IApiProvider provider, CancellationToken cancellationToken)
    {
        try
        {
            var results = await provider.GetDataAsync(cancellationToken);

            return (true, results);
        }
        catch (Exception)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return (false, new List<ApiResult>());
        }
    }



    private List<ApiResult> ApplyFiltersAndSorting(List<ApiResult> results, AggregationFilter? filter)
    {
        var filteredResults = results;

        if (filter != null)
        {
            //Filter by API name
            if (!string.IsNullOrWhiteSpace(filter.ApiName))
            {
                filteredResults = filteredResults.Where(result => result.ApiName == filter.ApiName).ToList();
            }

            //Search in the JSON data
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                filteredResults = filteredResults.Where(result => result.Data.GetRawText().Contains(filter.SearchTerm)).ToList();
            }
        }

        //Sort by ApiName
        if (filter != null && filter.SortBy == "ApiName")
        {
            if (filter.SortDescending)
            {
                filteredResults = filteredResults.OrderByDescending(result => result.ApiName).ToList();
            }
            else
            {
                filteredResults = filteredResults.OrderBy(result => result.ApiName).ToList();
            }
        }
        else
        {
            if (filter != null && filter.SortDescending)
            {
                filteredResults = filteredResults.OrderByDescending(result => result.Data.GetRawText()).ToList();
            }
            else
            {
                filteredResults = filteredResults.OrderBy(result => result.Data.GetRawText()).ToList();
            }
        }

        return filteredResults;
    }

}
