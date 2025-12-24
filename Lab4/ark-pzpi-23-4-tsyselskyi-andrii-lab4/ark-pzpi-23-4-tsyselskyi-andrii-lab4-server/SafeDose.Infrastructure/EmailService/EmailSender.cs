using Microsoft.Extensions.Options;
using SafeDose.Application.Contracts.Email;
using SafeDose.Application.Exceptions;
using SafeDose.Application.Models.Email;
using System.Net;
using System.Net.Mail;

namespace SafeDose.Infrastructure.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task SendEmailAsync(EmailMessage email)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = email.IsBodyHtml
            };
            mail.To.Add(email.To);

            using (var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network
            })
            {
                try
                {
                    await smtp.SendMailAsync(mail);
                }
                catch (SmtpException)
                {
                    throw new EmailNotSentException("The message hasn't been sent!");
                }
                catch (Exception)
                {
                    throw new InternalServerException();
                }

            }
        }
    }
}
