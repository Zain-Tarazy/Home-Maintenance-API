using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.ProviderProfiles;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/provider-profile")]
    public class ProviderProfileController : ControllerBase
    {
        private readonly IProviderProfileService _providerProfileService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateProviderProfileDto> _createValidator;
        private readonly IValidator<UpdateProviderProfileDto> _updateValidator;

        public ProviderProfileController(
            IProviderProfileService providerProfileService,
            IMapper mapper,
            IValidator<CreateProviderProfileDto> createValidator,
            IValidator<UpdateProviderProfileDto> updateValidator)
        {
            _providerProfileService = providerProfileService;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProviderProfileDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));


            var userId = GetCurrentUserId();

            var result = await _providerProfileService.CreateAsync(userId, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<ProviderProfileDto>(result.Data!);

            return Ok(response);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMine()
        {
            var userId = GetCurrentUserId();

            var result = await _providerProfileService.GetMineAsync(userId);

            if (!result.Succeeded)
                return NotFound(result.Error);

            var response = _mapper.Map<ProviderProfileDto>(result.Data!);

            return Ok(response);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMine(UpdateProviderProfileDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));


            var userId = GetCurrentUserId();

            var result = await _providerProfileService.UpdateMineAsync(userId, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<ProviderProfileDto>(result.Data!);

            return Ok(response);
        }

        [HttpGet("me/subscription-status")]
        public async Task<IActionResult> GetSubscriptionStatus()
        {
            var userId = GetCurrentUserId();

            var result = await _providerProfileService.GetSubscriptionStatusAsync(userId);

            if (!result.Succeeded)
                return NotFound(result.Error);

            return Ok(result.Data);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.Parse(userIdClaim!);
        }



        
    }
}
