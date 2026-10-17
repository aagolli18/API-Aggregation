using API_Aggregation.Models;
using API_Aggregation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Aggregation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AggregationController : ControllerBase
    {
        private readonly AggregationService _aggregationService;

        public AggregationController(AggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetData([FromQuery] AggregationFilter filter, CancellationToken cancellationToken)
        {
            var result = await _aggregationService.GetAllDataAsync(filter, cancellationToken);
            return Ok(result);
        }
    }
}
