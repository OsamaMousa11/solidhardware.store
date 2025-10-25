using Microsoft.Extensions.Options;
using MimeKit;
using solidhardware.storeCore.ServiceContract;


using Microsoft.AspNetCore.Http;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using solidhardware.storeCore.DTO.AuthenticationDTO;



namespace solidhardware.storeCore.Service
{
    public class MailingService : IMailingService
    {
        private readonly MailSettings _mailSettings;
        public MailingService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
            Console.WriteLine($"Loaded MailSettings: Email='{_mailSettings.Email}', DisplayName='{_mailSettings.DisplayName}'");
        }
        public async Task SendMessageAsync(string mailTo, string subject, string body, IList<IFormFile>? attach)
        {
            var email = new MimeMessage();

            // From / Sender
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));
            email.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email);

            // To
            email.To.Add(MailboxAddress.Parse(mailTo));

            // Subject
            email.Subject = subject;

            // Body + Attachments
            var builder = new BodyBuilder();

            if (attach != null)
            {
                foreach (var file in attach)
                {
                    if (file.Length > 0)
                    {
                        using var stream = new MemoryStream();
                        await file.CopyToAsync(stream);
                        var fileBytes = stream.ToArray();
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();

            // Send
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }


   }
