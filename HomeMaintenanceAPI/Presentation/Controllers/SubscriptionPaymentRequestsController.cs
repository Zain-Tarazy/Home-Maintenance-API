using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPaymentRequests;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/subscription-payment-requests")]
    public class SubscriptionPaymentRequestsController : ControllerBase
    {
        private readonly ISubscriptionPaymentRequestService _requestService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateSubscriptionPaymentRequestDto> _createValidator;

        public SubscriptionPaymentRequestsController(
            ISubscriptionPaymentRequestService requestService,
            IMapper mapper,
            IValidator<CreateSubscriptionPaymentRequestDto> createValidator)
        {
            _requestService = requestService;
            _mapper = mapper;
            _createValidator = createValidator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSubscriptionPaymentRequestDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));


            var userId = GetCurrentUserId();

            var result = await _requestService.CreateAsync(userId, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<SubscriptionPaymentRequestDto>(result.Data!);

            return Ok(response);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMine()
        {
            var userId = GetCurrentUserId();

            var result = await _requestService.GetMineAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<List<SubscriptionPaymentRequestDto>>(result.Data!);

            return Ok(response);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!);
        }
    }
}
