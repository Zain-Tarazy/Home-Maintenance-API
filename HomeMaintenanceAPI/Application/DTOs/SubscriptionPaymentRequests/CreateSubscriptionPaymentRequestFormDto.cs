using HomeMaintenanceAPI.Domain.Enums;

namespace HomeMaintenanceAPI.Application.DTOs.SubscriptionPaymentRequests
{
    public class CreateSubscriptionPaymentRequestFormDto
    {
        public int SubscriptionPlanId { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string TransactionId { get; set; } = string.Empty;

        public IFormFile ProofImage { get; set; } = null!;
    }
}
