using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace dev_DKHP.CoreModule.Helper.EmailSender
{
    public class MailMessageDto
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public MailMessageDto(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("email", x)));
            Subject = subject;
            Content = content;
        }
    }

    public class EmailConfiguration
    {
        public string? From { get; set; }
        public string? SmtpServer { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
