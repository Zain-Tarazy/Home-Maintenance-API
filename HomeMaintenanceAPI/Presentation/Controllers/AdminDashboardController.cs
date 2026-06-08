using AutoMapper;
using HomeMaintenanceAPI.Application.DTOs.Admin;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;

        public AdminDashboardController(
            IAdminService adminService,
            IMapper mapper)
        {
            _adminService = adminService;
            _mapper = mapper;
        }

        [HttpGet("dashboard/summary")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _adminService.GetDashboardSummaryAsync();

            return Ok(summary);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _adminService.GetUsersAsync();

            var response = _mapper.Map<List<AdminUserDto>>(users);

            return Ok(response);
        }

        [HttpGet("providers")]
        public async Task<IActionResult> GetProviders()
        {
            var providers = await _adminService.GetProvidersAsync();

            var response = _mapper.Map<List<AdminProviderDto>>(providers);

            return Ok(response);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _adminService.GetOrdersAsync();

            var response = _mapper.Map<List<AdminOrderDto>>(orders);

            return Ok(response);
        }
    }
}