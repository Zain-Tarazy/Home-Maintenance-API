using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPaymentRequests;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin/subscription-payment-requests")]
    public class AdminSubscriptionPaymentRequestsController : ControllerBase
    {
        private readonly ISubscriptionPaymentRequestService _requestService;
        private readonly IMapper _mapper;
        private readonly IValidator<RejectSubscriptionPaymentRequestDto> _rejectValidator;

        public AdminSubscriptionPaymentRequestsController(
            ISubscriptionPaymentRequestService requestService,
            IMapper mapper,
            IValidator<RejectSubscriptionPaymentRequestDto> rejectValidator)
        {
            _requestService = requestService;
            _mapper = mapper;
            _rejectValidator = rejectValidator;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var requests = await _requestService.GetPendingAsync();

            var response = _mapper.Map<List<SubscriptionPaymentRequestDto>>(requests);

            return Ok(response);
        }

        [HttpPatch("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var adminId = GetCurrentUserId();

            var result = await _requestService.ApproveAsync(id, adminId);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<SubscriptionPaymentRequestDto>(result.Data!);

            return Ok(response);
        }

        [HttpPatch("{id}/reject")]
        public async Task<IActionResult> Reject(int id, RejectSubscriptionPaymentRequestDto dto)
        {
            var validationResult = await _rejectValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));


            var adminId = GetCurrentUserId();

            var result = await _requestService.RejectAsync(id, adminId, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<SubscriptionPaymentRequestDto>(result.Data!);

            return Ok(response);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _requestService.GetAllAsync();

            var response = _mapper.Map<List<SubscriptionPaymentRequestDto>>(requests);

            return Ok(response);
        }
    }
}
