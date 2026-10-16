using API_Aggregation.Models;

namespace API_Aggregation.Interfaces
{
    public interface IApiProvider
    {
        string ApiName { get; }

        Task<List<ApiResult>> GetDataAsync(CancellationToken cancellationToken = default);
    }
}
