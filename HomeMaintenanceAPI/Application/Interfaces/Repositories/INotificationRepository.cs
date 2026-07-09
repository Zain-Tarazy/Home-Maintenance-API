using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace HomeMaintenanceAPI.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<PagedResult<Notification>> GetByUserIdAsync(int userId, PaginationParams paginationParams);

        Task<Notification?> GetByIdAsync(int id);

        Task<List<Notification>> GetUnreadByUserIdAsync(int userId);

        Task<Notification> AddAsync(Notification notification);

        Task UpdateAsync(Notification notification);

        Task UpdateRangeAsync(List<Notification> notifications);

        Task SaveChangesAsync();
    }
}
