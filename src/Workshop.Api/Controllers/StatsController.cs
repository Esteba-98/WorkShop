using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Workshop.Application.Services;

namespace Workshop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard([FromQuery] string periodo = "mes")
        {
            return Ok(await _statsService.GetDashboardStatsAsync(periodo));
        }
    }
}
