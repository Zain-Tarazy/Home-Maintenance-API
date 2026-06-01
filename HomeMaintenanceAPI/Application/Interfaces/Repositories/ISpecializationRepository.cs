using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Interfaces.Repositories
{
    public interface ISpecializationRepository
    {
        Task<List<Specialization>> GetActiveAsync();

        Task<List<Specialization>> GetAllAsync();

        Task<Specialization?> GetByIdAsync(int id);

        Task<bool> NameExistsAsync(string name);

        Task<bool> NameExistsExceptAsync(string name, int id);

        Task<Specialization> AddAsync(Specialization specialization);

        Task UpdateAsync(Specialization specialization);

        Task SaveChangesAsync();
    }
}
