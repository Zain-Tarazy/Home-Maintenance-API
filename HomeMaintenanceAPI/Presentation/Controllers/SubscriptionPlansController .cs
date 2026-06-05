using AutoMapper;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPlans;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/subscription-plans")]
    public class SubscriptionPlansController : ControllerBase
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly IMapper _mapper;

        public SubscriptionPlansController(
            ISubscriptionPlanService subscriptionPlanService,
            IMapper mapper)
        {
            _subscriptionPlanService = subscriptionPlanService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetActive()
        {
            var plans = await _subscriptionPlanService.GetActiveAsync();

            var dto = _mapper.Map<List<SubscriptionPlanDto>>(plans);

            return Ok(dto);
        }
    }
}
