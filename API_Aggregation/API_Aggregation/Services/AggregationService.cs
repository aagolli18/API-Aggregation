using API_Aggregation.Interfaces;
using API_Aggregation.Models;

namespace API_Aggregation.Services;

public class AggregationService
{
    private readonly IEnumerable<IApiProvider> _providers;

    public AggregationService(IEnumerable<IApiProvider> providers)
    {
        _providers = providers;
    }

    public async Task<AggregationResponse> GetAllDataAsync(AggregationFilter? filter = null, CancellationToken cancellationToken = default)
    {
        var providers = _providers.ToList();

        var tasks = new List<Task<(bool Success, List<ApiResult> Results)>>();

        foreach (var provider in providers)
        {
            tasks.Add(ExecuteProviderAsync(provider, cancellationToken));
        }

        var outcomes = await Task.WhenAll(tasks);

        var combinedResults = outcomes.SelectMany(outcome => outcome.Results).ToList();

        var filteredResults = ApplyFiltersAndSorting(combinedResults, filter);

        return new AggregationResponse
        {
            Results = filteredResults,
            SuccessfulProviders = outcomes.Count(outcome => outcome.Success),
            FailedProviders = outcomes.Count(outcome => !outcome.Success)
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
