using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Application.DTOs.SubscriptionPaymentRequests
{
    public class CreateSubscriptionPaymentRequestDto
    {
        public int SubscriptionPlanId { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string TransactionId { get; set; } = string.Empty;

        public string? ProofImageUrl { get; set; }
    }
}
