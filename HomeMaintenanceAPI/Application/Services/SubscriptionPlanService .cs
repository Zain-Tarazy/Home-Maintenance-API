using HomeMaintenanceAPI.Application.Common;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPlans;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;

        public SubscriptionPlanService(ISubscriptionPlanRepository subscriptionPlanRepository)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
        }

        public async Task<List<SubscriptionPlan>> GetActiveAsync()
        {
            return await _subscriptionPlanRepository.GetActiveAsync();
        }

        public async Task<List<SubscriptionPlan>> GetAllAsync()
        {
            return await _subscriptionPlanRepository.GetAllAsync();
        }

        public async Task<ServiceResult<SubscriptionPlan>> CreateAsync(CreateSubscriptionPlanDto dto)
        {
            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return ServiceResult<SubscriptionPlan>.Failure("Plan name is required.");

            if (dto.Price < 0)
                return ServiceResult<SubscriptionPlan>.Failure("Price cannot be negative.");

            if (dto.DurationInDays <= 0)
                return ServiceResult<SubscriptionPlan>.Failure("Duration must be greater than zero.");

            if (await _subscriptionPlanRepository.NameExistsAsync(name))
                return ServiceResult<SubscriptionPlan>.Failure("Plan name already exists.");

            var plan = new SubscriptionPlan
            {
                Name = name,
                Price = dto.Price,
                DurationInDays = dto.DurationInDays,
                IsActive = true
            };

            await _subscriptionPlanRepository.AddAsync(plan);
            await _subscriptionPlanRepository.SaveChangesAsync();

            return ServiceResult<SubscriptionPlan>.Success(plan);
        }

        public async Task<ServiceResult<SubscriptionPlan>> UpdateAsync(int id, UpdateSubscriptionPlanDto dto)
        {
            var plan = await _subscriptionPlanRepository.GetByIdAsync(id);

            if (plan == null)
                return ServiceResult<SubscriptionPlan>.Failure("Subscription plan not found.");

            if (await _subscriptionPlanRepository.IsUsedAsync(id))
            {
                return ServiceResult<SubscriptionPlan>.Failure(
                    "This plan has already been used. Deactivate it and create a new plan instead.");
            }

            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return ServiceResult<SubscriptionPlan>.Failure("Plan name is required.");
             
            if (dto.Price < 0)
                return ServiceResult<SubscriptionPlan>.Failure("Price cannot be negative.");

            if (dto.DurationInDays <= 0)
                return ServiceResult<SubscriptionPlan>.Failure("Duration must be greater than zero.");

            if (await _subscriptionPlanRepository.NameExistsExceptAsync(name, id))
                return ServiceResult<SubscriptionPlan>.Failure("Plan name already exists.");

            plan.Name = name;
            plan.Price = dto.Price;
            plan.DurationInDays = dto.DurationInDays;

            await _subscriptionPlanRepository.UpdateAsync(plan);
            await _subscriptionPlanRepository.SaveChangesAsync();

            return ServiceResult<SubscriptionPlan>.Success(plan);
        }

        public async Task<ServiceResult> ActivateAsync(int id)
        {
            var plan = await _subscriptionPlanRepository.GetByIdAsync(id);

            if (plan == null)
                return ServiceResult.Failure("Subscription plan not found.");

            plan.IsActive = true;

            await _subscriptionPlanRepository.UpdateAsync(plan);
            await _subscriptionPlanRepository.SaveChangesAsync();

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeactivateAsync(int id)
        {
            var plan = await _subscriptionPlanRepository.GetByIdAsync(id);

            if (plan == null)
                return ServiceResult.Failure("Subscription plan not found.");

            plan.IsActive = false;

            await _subscriptionPlanRepository.UpdateAsync(plan);
            await _subscriptionPlanRepository.SaveChangesAsync();

            return ServiceResult.Success();
        }
    }
}
