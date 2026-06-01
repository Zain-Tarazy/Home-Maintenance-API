using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
    }
}
