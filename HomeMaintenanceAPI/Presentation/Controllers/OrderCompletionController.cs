using System.Security.Claims;
using HomeMaintenanceAPI.Application.DTOs.OrderCompletion;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/orders")]
    public class OrderCompletionController : ControllerBase
    {
        private readonly IOrderCompletionService _orderCompletionService;

        public OrderCompletionController(IOrderCompletionService orderCompletionService)
        {
            _orderCompletionService = orderCompletionService;
        }

        [HttpPost("{id}/completion-qr")]
        public async Task<IActionResult> GenerateCompletionQr(int id)
        {
            var userId = GetCurrentUserId();

            var result = await _orderCompletionService.GenerateCompletionQrAsync(userId, id);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }

        [HttpPost("complete-by-qr")]
        public async Task<IActionResult> CompleteByQr(CompleteOrderByQrDto dto)
        {
            var userId = GetCurrentUserId();

            var result = await _orderCompletionService.CompleteByQrAsync(userId, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Order completed successfully.");
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!);
        }
    }
}
