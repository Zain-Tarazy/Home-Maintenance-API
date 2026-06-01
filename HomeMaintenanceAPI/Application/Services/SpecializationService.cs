using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.Specialization;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Services
{
    public class SpecializationService : ISpecializationService
    {
        private readonly ISpecializationRepository _specializationRepository;

        public SpecializationService(ISpecializationRepository specializationRepository)
        {
            _specializationRepository = specializationRepository;
        }

        public async Task<List<Specialization>> GetActiveAsync()
        {
            return await _specializationRepository.GetActiveAsync();
        }

        public async Task<List<Specialization>> GetAllAsync()
        {
            return await _specializationRepository.GetAllAsync();
        }

        public async Task<ServiceResult<Specialization>> CreateAsync(CreateSpecializationDto dto)
        {
            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return ServiceResult<Specialization>.Failure("Specialization name is required.");

            if (await _specializationRepository.NameExistsAsync(name))
                return ServiceResult<Specialization>.Failure("Specialization name already exists.");

            var specialization = new Specialization
            {
                Name = name,
                Description = string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim(),
                IsActive = true
            };

            await _specializationRepository.AddAsync(specialization);
            await _specializationRepository.SaveChangesAsync();

            return ServiceResult<Specialization>.Success(specialization);
        }

        public async Task<ServiceResult<Specialization>> UpdateAsync(int id, UpdateSpecializationDto dto)
        {
            var specialization = await _specializationRepository.GetByIdAsync(id);

            if (specialization == null)
                return ServiceResult<Specialization>.Failure("Specialization not found.");

            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return ServiceResult<Specialization>.Failure("Specialization name is required.");

            if (await _specializationRepository.NameExistsExceptAsync(name, id))
                return ServiceResult<Specialization>.Failure("Specialization name already exists.");

            specialization.Name = name;
            specialization.Description = string.IsNullOrWhiteSpace(dto.Description)
                ? null
                : dto.Description.Trim();

            await _specializationRepository.UpdateAsync(specialization);
            await _specializationRepository.SaveChangesAsync();

            return ServiceResult<Specialization>.Success(specialization);
        }

        public async Task<ServiceResult> ActivateAsync(int id)
        {
            var specialization = await _specializationRepository.GetByIdAsync(id);

            if (specialization == null)
                return ServiceResult.Failure("Specialization not found.");

            specialization.IsActive = true;

            await _specializationRepository.UpdateAsync(specialization);
            await _specializationRepository.SaveChangesAsync();

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeactivateAsync(int id)
        {
            var specialization = await _specializationRepository.GetByIdAsync(id);

            if (specialization == null)
                return ServiceResult.Failure("Specialization not found.");

            specialization.IsActive = false;

            await _specializationRepository.UpdateAsync(specialization);
            await _specializationRepository.SaveChangesAsync();

            return ServiceResult.Success();
        }
    }
}
