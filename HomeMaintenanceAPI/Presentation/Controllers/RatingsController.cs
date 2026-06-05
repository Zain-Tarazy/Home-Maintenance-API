using System.Security.Claims;
using AutoMapper;
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

        public RatingsController(
            IRatingService ratingService,
            IMapper mapper)
        {
            _ratingService = ratingService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRatingDto dto)
        {
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
