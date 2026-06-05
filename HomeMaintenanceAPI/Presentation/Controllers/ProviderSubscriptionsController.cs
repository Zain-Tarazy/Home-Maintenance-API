using System.Security.Claims;
using AutoMapper;
using HomeMaintenanceAPI.Application.DTOs.ProviderSubscriptions;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/provider-subscriptions")]
    public class ProviderSubscriptionsController : ControllerBase
    {
        private readonly IProviderSubscriptionService _providerSubscriptionService;
        private readonly IMapper _mapper;

        public ProviderSubscriptionsController(
            IProviderSubscriptionService providerSubscriptionService,
            IMapper mapper)
        {
            _providerSubscriptionService = providerSubscriptionService;
            _mapper = mapper;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMine()
        {
            var userId = GetCurrentUserId();

            var result = await _providerSubscriptionService.GetMineAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<List<ProviderSubscriptionDto>>(result.Data!);

            return Ok(response);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!);
        }
    }
}
