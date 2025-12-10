using SafeDose.Application.Models.Email;

namespace SafeDose.Application.Contracts.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage email);
    }
}
