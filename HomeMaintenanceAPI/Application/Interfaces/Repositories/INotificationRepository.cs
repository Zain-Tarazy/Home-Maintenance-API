using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByUserIdAsync(int userId);

        Task<Notification?> GetByIdAsync(int id);

        Task<List<Notification>> GetUnreadByUserIdAsync(int userId);

        Task<Notification> AddAsync(Notification notification);

        Task UpdateAsync(Notification notification);

        Task UpdateRangeAsync(List<Notification> notifications);

        Task SaveChangesAsync();
    }
}
