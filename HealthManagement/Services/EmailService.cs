using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HealthManagement.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var enabled = _configuration.GetValue<bool>("EmailSettings:Enabled", true);
            if (!enabled)
            {
                _logger.LogWarning("Email sending is disabled by configuration.");
                return;
            }

            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = _configuration.GetValue<int>("EmailSettings:SmtpPort", 587);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];
            var appPassword = _configuration["EmailSettings:AppPassword"];
            var useSsl = _configuration.GetValue<bool>("EmailSettings:UseSsl", true);

            if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(appPassword))
            {
                throw new InvalidOperationException("Email settings are missing. Please configure EmailSettings in appsettings.json or environment variables.");
            }

            var normalizedPassword = appPassword.Replace(" ", string.Empty);

            using var message = new MailMessage();
            message.From = new MailAddress(senderEmail, string.IsNullOrWhiteSpace(senderName) ? senderEmail : senderName);
            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = useSsl,
                Credentials = new NetworkCredential(senderEmail, normalizedPassword)
            };

            await client.SendMailAsync(message);
            _logger.LogInformation("Reminder email sent to {Email} with subject {Subject}", to, subject);
        }
    }
}
