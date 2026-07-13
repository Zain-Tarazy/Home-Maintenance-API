using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPaymentRequests;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Application.Services;
using HomeMaintenanceAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/subscription-payment-requests")]
    public class SubscriptionPaymentRequestsController : ControllerBase
    {
        private readonly ISubscriptionPaymentRequestService _requestService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateSubscriptionPaymentRequestDto> _createValidator;
        private readonly IFileStorageService _fileStorageService;
        private readonly ISubscriptionPaymentRequestService _subscriptionPaymentRequestService;

        public SubscriptionPaymentRequestsController(
            ISubscriptionPaymentRequestService requestService,
            IMapper mapper,
            IValidator<CreateSubscriptionPaymentRequestDto> createValidator,
            IFileStorageService fileStorageService,
            ISubscriptionPaymentRequestService subscriptionPaymentRequestService)
        {
            _requestService = requestService;
            _mapper = mapper;
            _createValidator = createValidator;
            _fileStorageService = fileStorageService;
            _subscriptionPaymentRequestService = subscriptionPaymentRequestService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateSubscriptionPaymentRequestFormDto formDto)
        {
            if (formDto.ProofImage == null || formDto.ProofImage.Length == 0)
                return BadRequest("Payment proof image is required.");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            string proofImageUrl;

            try
            {
                proofImageUrl = await _fileStorageService.SaveImageAsync(
                    formDto.ProofImage.OpenReadStream(),
                    formDto.ProofImage.FileName,
                    formDto.ProofImage.ContentType,
                    formDto.ProofImage.Length,
                    "subscription-proofs");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            var dto = new CreateSubscriptionPaymentRequestDto
            {
                SubscriptionPlanId = formDto.SubscriptionPlanId,
                PaymentMethod = formDto.PaymentMethod,
                TransactionId = formDto.TransactionId,
                ProofImageUrl = proofImageUrl
            };

            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                _fileStorageService.DeleteFile(proofImageUrl);
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await _subscriptionPaymentRequestService.CreateAsync(userId, dto);

            if (!result.Succeeded)
            {
                _fileStorageService.DeleteFile(proofImageUrl);
                return BadRequest(result.Error);
            }

            return Ok(result.Data);
        }
   

        [HttpGet("me")]
        public async Task<IActionResult> GetMine()
        {
            var userId = GetCurrentUserId();

            var result = await _requestService.GetMineAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<List<SubscriptionPaymentRequestDto>>(result.Data!);

            return Ok(response);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!);
        }
    }
}
