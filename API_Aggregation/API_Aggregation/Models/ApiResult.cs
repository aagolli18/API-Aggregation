using System.Text.Json;

namespace API_Aggregation.Models
{
    public class ApiResult
    {
        public string ApiName { get; set; } = string.Empty;
        public JsonElement Data { get; set; }
    }
}
