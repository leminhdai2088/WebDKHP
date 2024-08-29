using dev_DKHP.CoreModule.Dto.Common;
using dev_DKHP.CoreModule.Helper.EmailSender;
using dev_DKHP.Intfs;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
namespace dev_DKHP.Impls
{
    public class EmailAppService: IEmailAppService
    {
        private readonly EmailConfiguration _emailConfiguration;
        public EmailAppService(EmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }
        public async Task<CommonReturnDto> SendEmailAsync(MailMessageDto mailMessage)
        {
            var emailMessage = CreateMailMessage(mailMessage);
            bool result = await SendAync(emailMessage);
            if (result) return new CommonReturnDto
            {
                STATUS_CODE = 0,
                ERROR_MESSAGE = "Send successfully!"
            };
            else return new CommonReturnDto
            {
                STATUS_CODE = -1,
                ERROR_MESSAGE = "Send failed!"
            };
        }
        private MimeMessage CreateMailMessage(MailMessageDto mailMessage)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfiguration.From));
            emailMessage.To.AddRange(mailMessage.To);
            emailMessage.Subject = mailMessage.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = mailMessage.Content
            };
            return emailMessage;
        }

        private async Task<bool> SendAync(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                await client.AuthenticateAsync(_emailConfiguration.Username, _emailConfiguration.Password);
                await client.SendAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
    }
}
