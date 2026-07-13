using HomeMaintenanceAPI.Domain.Entities;

namespace HomeMaintenanceAPI.Presentation.Controllers
{
    public class CreateSubscriptionPaymentRequestFormDto
    {
        public int SubscriptionPlanId { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string TransactionId { get; set; } = string.Empty;

        public IFormFile ProofImage { get; set; } = null!;
    }
}
