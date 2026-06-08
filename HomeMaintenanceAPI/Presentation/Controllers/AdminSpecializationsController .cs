using AutoMapper;
using FluentValidation;
using HomeMaintenanceAPI.Application.DTOs.Specialization;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin/specializations")]
    public class AdminSpecializationsController : ControllerBase
    {
        private readonly ISpecializationService _specializationService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateSpecializationDto> _createValidator;
        private readonly IValidator<UpdateSpecializationDto> _updateValidator;
        public AdminSpecializationsController(
            ISpecializationService specializationService,
            IMapper mapper,
            IValidator<CreateSpecializationDto> createValidator,
            IValidator<UpdateSpecializationDto> updateValidator)
        {
            _specializationService = specializationService;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var specializations = await _specializationService.GetAllAsync();

            var dto = _mapper.Map<List<SpecializationDto>>(specializations);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSpecializationDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));


            var result = await _specializationService.CreateAsync(dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<SpecializationDto>(result.Data!);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSpecializationDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));


            var result = await _specializationService.UpdateAsync(id, dto);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            var response = _mapper.Map<SpecializationDto>(result.Data!);

            return Ok(response);
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await _specializationService.ActivateAsync(id);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Specialization activated successfully.");
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _specializationService.DeactivateAsync(id);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok("Specialization deactivated successfully.");
        }
    }
}
