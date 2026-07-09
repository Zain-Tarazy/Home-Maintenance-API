using System.Security.Claims;
using AutoMapper;
using HomeMaintenanceAPI.Application.Common;
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
        public async Task<IActionResult> GetAvailableOrders([FromQuery] PaginationParams paginationParams)
        {
            var userId = GetCurrentUserId();

            var result = await _orderService.GetAvailableForProviderAsync(userId, paginationParams);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = new PagedResult<OrderDto>
            {
                Items = _mapper.Map<List<OrderDto>>(result.Data!.Items),
                PageNumber = result.Data.PageNumber,
                PageSize = result.Data.PageSize,
                TotalCount = result.Data.TotalCount
            };

            foreach (var order in response.Items)
            {
                order.CustomerPhoneNumber = null;
                order.SelectedProviderPhoneNumber = null;
            }

            return Ok(response);
        }

        [HttpGet("assigned")]
        public async Task<IActionResult> GetAssignedOrders([FromQuery] PaginationParams paginationParams)
        {
            var userId = GetCurrentUserId();

            var result = await _orderService.GetAssignedForProviderAsync(userId, paginationParams);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = new PagedResult<OrderDto>
            {
                Items = _mapper.Map<List<OrderDto>>(result.Data!.Items),
                PageNumber = result.Data.PageNumber,
                PageSize = result.Data.PageSize,
                TotalCount = result.Data.TotalCount
            };

           
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
