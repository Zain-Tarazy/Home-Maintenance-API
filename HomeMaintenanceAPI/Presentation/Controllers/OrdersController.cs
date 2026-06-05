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
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(
            IOrderService orderService,
            IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            var userId = GetCurrentUserId();

            var result = await _orderService.CreateAsync(userId, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<OrderDto>(result.Data!);
            ApplyPhoneVisibility(response, result.Data!, userId);

            return Ok(response);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMine()
        {
            var userId = GetCurrentUserId();

            var result = await _orderService.GetMineAsync(userId);

            var response = _mapper.Map<List<OrderDto>>(result.Data!);

            for (int i = 0; i < response.Count; i++)
            {
                ApplyPhoneVisibility(response[i], result.Data![i], userId);
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetCurrentUserId();

            var result = await _orderService.GetByIdAsync(userId, id);

            if (!result.Succeeded)
                return NotFound(result.Error);

            var response = _mapper.Map<OrderDto>(result.Data!);
            ApplyPhoneVisibility(response, result.Data!, userId);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateOrderDto dto)
        {
            var userId = GetCurrentUserId();

            var result = await _orderService.UpdateAsync(userId, id, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<OrderDto>(result.Data!);
            ApplyPhoneVisibility(response, result.Data!, userId);

            return Ok(response);
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetCurrentUserId();

            var result = await _orderService.CancelAsync(userId, id);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Order cancelled successfully.");
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

            var isCustomer = order.CustomerId == currentUserId;
            var isSelectedProvider =
                order.SelectedProviderProfile != null &&
                order.SelectedProviderProfile.UserId == currentUserId;

            if (isCustomer)
            {
                dto.CustomerPhoneNumber = null;
                return;
            }

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
