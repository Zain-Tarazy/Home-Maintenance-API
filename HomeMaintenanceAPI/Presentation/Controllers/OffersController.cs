using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs;
using HomeMaintenanceAPI.Application.DTOs.Offers;
using HomeMaintenanceAPI.Application.Helpers;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/offers")]
    public class OffersController : ControllerBase
    {
        private readonly IProviderOfferService _offerService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateOfferDto> _createOfferValidator;
        private readonly IValidator<UpdateOfferDto> _updateOfferValidator;

        public OffersController(
            IProviderOfferService offerService,
            IMapper mapper,
            IValidator<CreateOfferDto> createOfferValidator,
            IValidator<UpdateOfferDto> updateOfferValidator)
        {
            _offerService = offerService;
            _mapper = mapper;
            _createOfferValidator = createOfferValidator;
            _updateOfferValidator = updateOfferValidator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOfferDto dto)
        {
            var validationResult = await _createOfferValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));


            var userId = GetCurrentUserId();

            var result = await _offerService.CreateAsync(userId, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<OfferDto>(result.Data!);
            ApplyPhoneVisibility(response, result.Data!, userId);

            return Ok(response);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMine([FromQuery] PaginationParams paginationParams)
        {
            var userId = GetCurrentUserId();

            var result = await _offerService.GetMineAsync(userId, paginationParams);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = new PagedResult<OfferDto>
            {
                Items = _mapper.Map<List<OfferDto>>(result.Data!.Items),
                PageNumber = result.Data.PageNumber,
                PageSize = result.Data.PageSize,
                TotalCount = result.Data.TotalCount
            };

            foreach (var offer in response.Items)
            {
                PhoneVisibilityHelper.ApplyForProviderViewingOwnOffer(offer);
            }

            return Ok(response);
        }

        [HttpGet("~/api/orders/{orderId}/offers")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var userId = GetCurrentUserId();

            var result = await _offerService.GetByOrderIdAsync(userId, orderId);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<List<OfferDto>>(result.Data!);

            for (int i = 0; i < response.Count; i++)
            {
                ApplyPhoneVisibility(response[i], result.Data![i], userId);
            }

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateOfferDto dto)
        {
            var validationResult = await _updateOfferValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));


            var userId = GetCurrentUserId();

            var result = await _offerService.UpdateAsync(userId, id, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<OfferDto>(result.Data!);
            ApplyPhoneVisibility(response, result.Data!, userId);

            return Ok(response);
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetCurrentUserId();

            var result = await _offerService.CancelAsync(userId, id);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Offer cancelled successfully.");
        }

        [HttpPatch("{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var userId = GetCurrentUserId();

            var result = await _offerService.RejectAsync(userId, id);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Offer rejected successfully.");
        }

        [HttpPatch("{id}/accept-inspection")]
        public async Task<IActionResult> AcceptForInspection(int id)
        {
            var userId = GetCurrentUserId();

            var result = await _offerService.AcceptForInspectionAsync(userId, id);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<OfferDto>(result.Data!);
            ApplyPhoneVisibility(response, result.Data!, userId);

            return Ok(response);
        }

        [HttpPatch("{id}/reject-after-inspection")]
        public async Task<IActionResult> RejectAfterInspection(int id)
        {
            var userId = GetCurrentUserId();

            var result = await _offerService.RejectAfterInspectionAsync(userId, id);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<OfferDto>(result.Data!);
            ApplyPhoneVisibility(response, result.Data!, userId);

            return Ok(response);
        }

        [HttpPatch("{id}/continue-work")]
        public async Task<IActionResult> ContinueWork(int id)
        {
            var userId = GetCurrentUserId();

            var result = await _offerService.ContinueWorkAsync(userId, id);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<OfferDto>(result.Data!);
            ApplyPhoneVisibility(response, result.Data!, userId);

            return Ok(response);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!);
        }

        private void ApplyPhoneVisibility(OfferDto dto, ProviderOffer offer, int currentUserId)
        {
            var statusAllowsPhone =
                offer.Order.Status == OrderStatus.InspectionAccepted ||
                offer.Order.Status == OrderStatus.InProgress ||
                offer.Order.Status == OrderStatus.CompletionPending ||
                offer.Order.Status == OrderStatus.Completed;

            var isSelectedProviderForOrder =
                offer.Order.SelectedProviderProfileId == offer.ProviderProfileId;

            if (!statusAllowsPhone || !isSelectedProviderForOrder)
            {
                dto.CustomerPhoneNumber = null;
                dto.ProviderPhoneNumber = null;
                return;
            }

            var isCustomer = offer.Order.CustomerId == currentUserId;
            var isProvider = offer.ProviderProfile.UserId == currentUserId;

            if (isCustomer)
            {
                dto.CustomerPhoneNumber = null;
                return;
            }

            if (isProvider)
            {
                dto.ProviderPhoneNumber = null;
                return;
            }

            dto.CustomerPhoneNumber = null;
            dto.ProviderPhoneNumber = null;
        }
    }
}
