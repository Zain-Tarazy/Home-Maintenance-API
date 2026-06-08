using AutoMapper;
using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPlans;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin/subscription-plans")]
    public class AdminSubscriptionPlansController : ControllerBase
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateSubscriptionPlanDto> _createValidator;
        private readonly IValidator<UpdateSubscriptionPlanDto> _updateValidator;

        public AdminSubscriptionPlansController(
            ISubscriptionPlanService subscriptionPlanService,
            IMapper mapper,
            IValidator<CreateSubscriptionPlanDto> createValidator,
            IValidator<UpdateSubscriptionPlanDto> updateValidator)
        {
            _subscriptionPlanService = subscriptionPlanService;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var plans = await _subscriptionPlanService.GetAllAsync();

            var dto = _mapper.Map<List<SubscriptionPlanDto>>(plans);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSubscriptionPlanDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));


            var result = await _subscriptionPlanService.CreateAsync(dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<SubscriptionPlanDto>(result.Data!);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSubscriptionPlanDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));


            var result = await _subscriptionPlanService.UpdateAsync(id, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<SubscriptionPlanDto>(result.Data!);

            return Ok(response);
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await _subscriptionPlanService.ActivateAsync(id);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Subscription plan activated successfully.");
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _subscriptionPlanService.DeactivateAsync(id);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Subscription plan deactivated successfully.");
        }
    }
}
