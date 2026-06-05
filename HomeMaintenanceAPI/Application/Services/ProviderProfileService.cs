using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.ProviderProfiles;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Services
{
    public class ProviderProfileService : IProviderProfileService
    {
        private readonly IProviderProfileRepository _providerProfileRepository;
        private readonly ISpecializationRepository _specializationRepository;

        public ProviderProfileService(
            IProviderProfileRepository providerProfileRepository,
            ISpecializationRepository specializationRepository)
        {
            _providerProfileRepository = providerProfileRepository;
            _specializationRepository = specializationRepository;
        }

        public async Task<ServiceResult<ProviderProfile>> CreateAsync(int userId, CreateProviderProfileDto dto)
        {
            if (await _providerProfileRepository.ExistsForUserAsync(userId))
                return ServiceResult<ProviderProfile>.Failure("You already have a provider profile.");

            var specialization = await _specializationRepository.GetByIdAsync(dto.SpecializationId);

            if (specialization == null)
                return ServiceResult<ProviderProfile>.Failure("Specialization not found.");

            if (!specialization.IsActive)
                return ServiceResult<ProviderProfile>.Failure("Specialization is not active.");

            var providerProfile = new ProviderProfile
            {
                UserId = userId,
                SpecializationId = dto.SpecializationId,
                Bio = string.IsNullOrWhiteSpace(dto.Bio) ? null : dto.Bio.Trim()
            };

            await _providerProfileRepository.AddAsync(providerProfile);
            await _providerProfileRepository.SaveChangesAsync();

            var createdProfile = await _providerProfileRepository.GetByUserIdAsync(userId);

            return ServiceResult<ProviderProfile>.Success(createdProfile!);
        }

        public async Task<ServiceResult<ProviderProfile>> GetMineAsync(int userId)
        {
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(userId);

            if (providerProfile == null)
                return ServiceResult<ProviderProfile>.Failure("Provider profile not found.");

            return ServiceResult<ProviderProfile>.Success(providerProfile);
        }

        public async Task<ServiceResult<ProviderProfile>> UpdateMineAsync(int userId, UpdateProviderProfileDto dto)
        {
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(userId);

            if (providerProfile == null)
                return ServiceResult<ProviderProfile>.Failure("Provider profile not found.");

            providerProfile.Bio = string.IsNullOrWhiteSpace(dto.Bio) ? null : dto.Bio.Trim();

            await _providerProfileRepository.UpdateAsync(providerProfile);
            await _providerProfileRepository.SaveChangesAsync();

            return ServiceResult<ProviderProfile>.Success(providerProfile);
        }

        public async Task<ServiceResult<ProviderSubscriptionStatusDto>> GetSubscriptionStatusAsync(int userId)
        {
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(userId);

            if (providerProfile == null)
                return ServiceResult<ProviderSubscriptionStatusDto>.Failure("Provider profile not found.");

            var hasActiveSubscription =
                await _providerProfileRepository.HasActiveSubscriptionAsync(providerProfile.Id);

            var endsAt =
                await _providerProfileRepository.GetActiveSubscriptionEndDateAsync(providerProfile.Id);

            return ServiceResult<ProviderSubscriptionStatusDto>.Success(new ProviderSubscriptionStatusDto
            {
                HasActiveSubscription = hasActiveSubscription,
                ActiveSubscriptionEndsAt = endsAt
            });
        }
    }
}
