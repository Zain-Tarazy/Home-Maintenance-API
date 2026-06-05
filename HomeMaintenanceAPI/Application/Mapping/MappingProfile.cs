using AutoMapper;
using HomeMaintenanceAPI.Application.DTOs.ProviderProfiles;
using HomeMaintenanceAPI.Application.DTOs.ProviderSubscriptions;
using HomeMaintenanceAPI.Application.DTOs.Specialization;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPaymentRequests;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPlans;
using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProviderProfile, ProviderProfileDto>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.User.PhoneNumber))
            .ForMember(dest => dest.SpecializationName,
                opt => opt.MapFrom(src => src.Specialization.Name));

            CreateMap<SubscriptionPlan, SubscriptionPlanDto>();
            CreateMap<Specialization, SpecializationDto>().ReverseMap();

            CreateMap<SubscriptionPaymentRequest, SubscriptionPaymentRequestDto>()
            .ForMember(dest => dest.ProviderName,
                opt => opt.MapFrom(src => src.ProviderProfile.User.FullName))
            .ForMember(dest => dest.PlanName,
                opt => opt.MapFrom(src => src.SubscriptionPlan.Name))
            .ForMember(dest => dest.ReviewedByAdminName,
                opt => opt.MapFrom(src => src.ReviewedByAdmin != null
                        ? src.ReviewedByAdmin.FullName
                        : null));

            CreateMap<ProviderSubscription, ProviderSubscriptionDto>()
            .ForMember(dest => dest.PlanName,
                opt => opt.MapFrom(src => src.SubscriptionPlan.Name))
            .ForMember(dest => dest.PlanPrice,
                opt => opt.MapFrom(src => src.SubscriptionPlan.Price))
            .ForMember(dest => dest.DurationInDays,
                opt => opt.MapFrom(src => src.SubscriptionPlan.DurationInDays))
            .ForMember(dest => dest.IsActive,
                opt => opt.MapFrom(src =>
                        src.StartsAt <= DateTime.UtcNow && src.EndsAt > DateTime.UtcNow));
        }
    }
}
