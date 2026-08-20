using HMS.ServicesAbstraction;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Shared.Message;

namespace HMS.Infrastructure.ExternalServices
{
    public class EmailServices : IEmailServices
    {
        private readonly IOptions<EmailSetting> _options;

        public EmailServices(IOptions<EmailSetting> options)
        {
            _options = options;
        }

        public async Task SendEmail(Email email)
        {
            var mail = new MimeMessage()
            {
                Sender = MailboxAddress.Parse(_options.Value.Email),
                Subject = email.Subject,
            };
            mail.To.Add(MailboxAddress.Parse(email.To));
            mail.From.Add(new MailboxAddress(_options.Value.DisplayName, _options.Value.Email));

            var builder = new BodyBuilder
            {
                TextBody = email.Body
            };
            mail.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            try
            {
                await smtp.ConnectAsync(
                    _options.Value.Host,
                    _options.Value.Port,
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(_options.Value.Email, _options.Value.Password);
                await smtp.SendAsync(mail);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}