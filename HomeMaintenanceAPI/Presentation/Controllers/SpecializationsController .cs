using AutoMapper;
using HomeMaintenanceAPI.Application.DTOs.Specialization;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/specializations")]
    public class SpecializationsController : ControllerBase
    {
        private readonly ISpecializationService _specializationService;
        private readonly IMapper _mapper;

        public SpecializationsController(
            ISpecializationService specializationService,
            IMapper mapper)
        {
            _specializationService = specializationService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetActive()
        {
            var specializations = await _specializationService.GetActiveAsync();

            var dto = _mapper.Map<List<SpecializationDto>>(specializations);

            return Ok(dto);
        }
    }
}
