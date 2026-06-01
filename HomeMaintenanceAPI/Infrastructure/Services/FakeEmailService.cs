using HomeMaintenanceAPI.Application.Interfaces.Services;


namespace HomeMaintenanceAPI.Infrastructure.Services
{
    public class FakeEmailService:IEmailService
    {
        private readonly ILogger<FakeEmailService> _logger;

        public FakeEmailService(ILogger<FakeEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            _logger.LogInformation(
                "FAKE EMAIL SENT\nTo: {To}\nSubject: {Subject}\nBody: {Body}",
                to,
                subject,
                body);

            return Task.CompletedTask;
        }
    }
}
