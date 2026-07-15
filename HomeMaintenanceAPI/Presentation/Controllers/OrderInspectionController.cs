using System.Security.Claims;
using HomeMaintenanceAPI.Application.DTOs.OrderInspection;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/orders")]
    public class OrderInspectionController : ControllerBase
    {
        private readonly IOrderInspectionService _inspectionService;

        public OrderInspectionController(IOrderInspectionService inspectionService)
        {
            _inspectionService = inspectionService;
        }

        [HttpPost("{id}/inspection-qr")]
        public async Task<IActionResult> GenerateInspectionQr(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _inspectionService.GenerateInspectionQrAsync(userId, id);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }

        [HttpPost("confirm-inspection-by-qr")]
        public async Task<IActionResult> ConfirmInspectionByQr(ConfirmInspectionByQrDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _inspectionService.ConfirmInspectionByQrAsync(userId, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Inspection started successfully.");
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}
