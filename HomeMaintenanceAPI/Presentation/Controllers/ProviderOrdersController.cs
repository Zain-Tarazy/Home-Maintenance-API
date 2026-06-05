using System.Security.Claims;
using AutoMapper;
using HomeMaintenanceAPI.Application.DTOs.Orders;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/provider/orders")]
    public class ProviderOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public ProviderOrdersController(
            IOrderService orderService,
            IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable()
        {
            var userId = GetCurrentUserId();

            var result = await _orderService.GetAvailableForProviderAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<List<OrderDto>>(result.Data!);

            foreach (var order in response)
            {
                order.CustomerName = null;
                order.CustomerPhoneNumber = null;
                order.SelectedProviderName = null;
                order.SelectedProviderPhoneNumber = null;
            }

            return Ok(response);
        }

        [HttpGet("assigned")]
        public async Task<IActionResult> GetAssigned()
        {
            var userId = GetCurrentUserId();

            var result = await _orderService.GetAssignedForProviderAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<List<OrderDto>>(result.Data!);

            for (int i = 0; i < response.Count; i++)
            {
                ApplyPhoneVisibility(response[i], result.Data![i], userId);
            }

            return Ok(response);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!);
        }

        private void ApplyPhoneVisibility(OrderDto dto, Order order, int currentUserId)
        {
            var statusAllowsPhone =
                order.Status == OrderStatus.InspectionAccepted ||
                order.Status == OrderStatus.InProgress ||
                order.Status == OrderStatus.CompletionPending ||
                order.Status == OrderStatus.Completed;

            if (!statusAllowsPhone)
            {
                dto.CustomerPhoneNumber = null;
                dto.SelectedProviderPhoneNumber = null;
                return;
            }

            var isSelectedProvider =
                order.SelectedProviderProfile != null &&
                order.SelectedProviderProfile.UserId == currentUserId;

            if (isSelectedProvider)
            {
                dto.SelectedProviderPhoneNumber = null;
                return;
            }

            dto.CustomerPhoneNumber = null;
            dto.SelectedProviderPhoneNumber = null;
        }
    }
}
