using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeMaintenanceAPI.Infrastructure.Repositories
{
    public class SpecializationRepository : ISpecializationRepository
    {
        private readonly AppDbContext _context;

        public SpecializationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Specialization>> GetActiveAsync()
        {
            return await _context.Specializations
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<List<Specialization>> GetAllAsync()
        {
            return await _context.Specializations
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Specialization?> GetByIdAsync(int id)
        {
            return await _context.Specializations
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _context.Specializations
                .AnyAsync(s => s.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> NameExistsExceptAsync(string name, int id)
        {
            return await _context.Specializations
                .AnyAsync(s => s.Id != id && s.Name.ToLower() == name.ToLower());
        }

        public async Task<Specialization> AddAsync(Specialization specialization)
        {
            await _context.Specializations.AddAsync(specialization);
            return specialization;
        }

        public Task UpdateAsync(Specialization specialization)
        {
            _context.Specializations.Update(specialization);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
