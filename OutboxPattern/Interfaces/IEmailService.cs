using System.Threading.Tasks;

namespace OutboxPattern.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string email, string subject, string body);
    }
}
