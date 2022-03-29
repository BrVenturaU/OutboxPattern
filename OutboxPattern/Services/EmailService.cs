using OutboxPattern.Interfaces;
using OutboxPattern.Settings;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OutboxPattern.Services
{
    public class EmailService: IEmailService
    {
        private readonly IEmailSettings _emailSettings;

        public EmailService(IEmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public async Task SendEmail(string email, string subject, string body)
        {

            var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true
            };

            await client.SendMailAsync(_emailSettings.Sender, email, subject, body);
        }
    }
}
