using System.Security.Claims;
using AutoMapper;
using FluentValidation;
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
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateOrderDto> _createOrderValidator;
        private readonly IValidator<UpdateOrderDto> _updateOrderValidator;

        public OrdersController(
            IOrderService orderService,
            IMapper mapper,
            IValidator<CreateOrderDto> createOrderValidator,
            IValidator<UpdateOrderDto> updateOrderValidator)
        {
            _orderService = orderService;
            _mapper = mapper;
            _createOrderValidator = createOrderValidator;
            _updateOrderValidator = updateOrderValidator;   
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            var validationResult = await _createOrderValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));


            var userId = GetCurrentUserId();

            var result = await _orderService.CreateAsync(userId, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<OrderDto>(result.Data!);
            ApplyPhoneVisibility(response, result.Data!, userId);

            return Ok(response);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMine([FromQuery] PaginationParams paginationParams)
        {
            var userId = GetCurrentUserId();

            var result = await _orderService.GetMineAsync(userId, paginationParams);

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
            var validationResult = await _updateOrderValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));


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
                order.Status == OrderStatus.InspectionInProgress ||
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
