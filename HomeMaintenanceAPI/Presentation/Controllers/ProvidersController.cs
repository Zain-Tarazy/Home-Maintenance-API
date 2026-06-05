using HomeMaintenanceAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/providers")]
    public class ProvidersController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public ProvidersController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet("{providerProfileId}/rating-summary")]
        public async Task<IActionResult> GetRatingSummary(int providerProfileId)
        {
            var result = await _ratingService.GetProviderSummaryAsync(providerProfileId);

            if (!result.Succeeded)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
    }
}
