using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.DTOs.SubscriptionPaymentRequests
{
    public class SubscriptionPaymentRequestDto
    {
        public int Id { get; set; }

        public int ProviderProfileId { get; set; }

        public string ProviderName { get; set; } = string.Empty;

        public int SubscriptionPlanId { get; set; }

        public string PlanName { get; set; } = string.Empty;

        //public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string TransactionId { get; set; } = string.Empty;

        public string? ProofImageUrl { get; set; }

        public SubscriptionPaymentRequestStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public int? ReviewedByAdminId { get; set; }

        public string? ReviewedByAdminName { get; set; }

        public string? AdminNote { get; set; }
    }
}
