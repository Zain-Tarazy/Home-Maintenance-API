using System.Net;
using System.Net.Mail;
using HomeMaintenanceAPI.Application.Interfaces.Services;

namespace HomeMaintenanceAPI.Infrastructure.Services
{
    public class SMTPEmailService:IEmailService
    {
        private readonly IConfiguration _configuration;

        public SMTPEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var host = _configuration["Email:SmtpHost"];
            var port = int.Parse(_configuration["Email:SmtpPort"]!);
            var username = _configuration["Email:Username"];
            var password = _configuration["Email:Password"];
            var from = _configuration["Email:From"];

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(from!),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}
