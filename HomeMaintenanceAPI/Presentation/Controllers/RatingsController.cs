using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.Ratings;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/ratings")]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateRatingDto> _createValidator;

        public RatingsController(
            IRatingService ratingService,
            IMapper mapper,
            IValidator<CreateRatingDto> createValidator)
        {
            _ratingService = ratingService;
            _mapper = mapper;
            _createValidator = createValidator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRatingDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));


            var userId = GetCurrentUserId();

            var result = await _ratingService.CreateAsync(userId, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<RatingDto>(result.Data!);

            return Ok(response);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!);
        }
    }
}
