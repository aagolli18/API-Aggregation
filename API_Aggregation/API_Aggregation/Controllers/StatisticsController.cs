using API_Aggregation.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_Aggregation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IApiStatisticsService _statisticsService;

        public StatisticsController(IApiStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public IActionResult GetStatistics()
        {
            return Ok(_statisticsService.GetStatistics());
        }
    }
}
