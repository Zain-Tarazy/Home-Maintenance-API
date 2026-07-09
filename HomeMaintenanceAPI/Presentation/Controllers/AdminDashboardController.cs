using AutoMapper;
using HomeMaintenanceAPI.Application.Common;
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
        public async Task<IActionResult> GetUsers([FromQuery] PaginationParams paginationParams)
        {
            var users = await _adminService.GetUsersAsync(paginationParams);

            var response = new PagedResult<AdminUserDto>
            {
                Items = _mapper.Map<List<AdminUserDto>>(users.Items),
                PageNumber = users.PageNumber,
                PageSize = users.PageSize,
                TotalCount = users.TotalCount
            };

            return Ok(response);
        }

        [HttpGet("providers")]
        public async Task<IActionResult> GetProviders([FromQuery] PaginationParams paginationParams)
        {
            var providers = await _adminService.GetProvidersAsync(paginationParams);

            var response = new PagedResult<AdminProviderDto>
            {
                Items = _mapper.Map<List<AdminProviderDto>>(providers.Items),
                PageNumber = providers.PageNumber,
                PageSize = providers.PageSize,
                TotalCount = providers.TotalCount
            };

            return Ok(response);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] PaginationParams paginationParams)
        {
            var orders = await _adminService.GetOrdersAsync(paginationParams);

            var response = new PagedResult<AdminOrderDto>
            {
                Items = _mapper.Map<List<AdminOrderDto>>(orders.Items),
                PageNumber = orders.PageNumber,
                PageSize = orders.PageSize,
                TotalCount = orders.TotalCount
            };

            return Ok(response);
        }
    }
}