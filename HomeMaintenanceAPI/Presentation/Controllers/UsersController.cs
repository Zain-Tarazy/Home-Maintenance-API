using System.Security.Claims;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserProfileImageService _userProfileImageService;

        public UsersController(IUserProfileImageService userProfileImageService)
        {
            _userProfileImageService = userProfileImageService;
        }

        [HttpPost("me/profile-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadMyProfileImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Image file is required.");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _userProfileImageService.UploadProfileImageAsync(
                userId,
                file.OpenReadStream(),
                file.FileName,
                file.ContentType,
                file.Length);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
    }
}
