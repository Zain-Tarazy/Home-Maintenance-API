using AutoMapper;
using HomeMaintenanceAPI.Application.DTOs;
using HomeMaintenanceAPI.Application.DTOs.Orders;
using HomeMaintenanceAPI.Application.DTOs.ProviderProfiles;
using HomeMaintenanceAPI.Application.DTOs.ProviderSubscriptions;
using HomeMaintenanceAPI.Application.DTOs.Specialization;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPaymentRequests;
using HomeMaintenanceAPI.Application.DTOs.SubscriptionPlans;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;

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

            CreateMap<Order, OrderDto>()
           .ForMember(dest => dest.CustomerName,
                opt => opt.MapFrom(src => src.Customer.FullName))
            .ForMember(dest => dest.CustomerPhoneNumber,
                opt => opt.MapFrom(src => src.Customer.PhoneNumber))
            .ForMember(dest => dest.SpecializationName,
                opt => opt.MapFrom(src => src.Specialization.Name))
            .ForMember(dest => dest.SelectedProviderName,
                opt => opt.MapFrom(src => src.SelectedProviderProfile != null
            ? src.SelectedProviderProfile.User.FullName
            : null))
            .ForMember(dest => dest.SelectedProviderPhoneNumber,
                opt => opt.MapFrom(src => src.SelectedProviderProfile != null
                        ? src.SelectedProviderProfile.User.PhoneNumber
                        : null));

            CreateMap<ProviderOffer, OfferDto>()
            .ForMember(dest => dest.OrderDescription,
                opt => opt.MapFrom(src => src.Order.Description))
            .ForMember(dest => dest.OrderStatus,
                opt => opt.MapFrom(src => src.Order.Status))
            .ForMember(dest => dest.ProviderName,
                opt => opt.MapFrom(src => src.ProviderProfile.User.FullName))
            .ForMember(dest => dest.ProviderPhoneNumber,
                opt => opt.MapFrom(src => src.ProviderProfile.User.PhoneNumber))
            .ForMember(dest => dest.SpecializationName,
                opt => opt.MapFrom(src => src.ProviderProfile.Specialization.Name))
            .ForMember(dest => dest.CustomerName,
                opt => opt.MapFrom(src => src.Order.Customer.FullName))
            .ForMember(dest => dest.CustomerPhoneNumber,
                opt => opt.MapFrom(src => src.Order.Customer.PhoneNumber))
            .ForMember(dest => dest.RatingsCount,
                opt => opt.MapFrom(src => src.ProviderProfile.Ratings.Count))
            .ForMember(dest => dest.AverageRating,
                opt => opt.MapFrom(src => src.ProviderProfile.Ratings.Any()
                    ? src.ProviderProfile.Ratings.Average(r => r.Value)
                    : 0))
            .ForMember(dest => dest.CompletedOrdersCount,
                opt => opt.MapFrom(src => src.ProviderProfile.SelectedOrders
                        .Count(o => o.Status == OrderStatus.Completed)));
        }
    }
}
