using AutoMapper;
using HomeMaintenanceAPI.Application.DTOs.Specialization;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPlans;
using HomeMaintenanceAPI.Application.DTOs.ProviderProfiles;

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
        }
    }
}
