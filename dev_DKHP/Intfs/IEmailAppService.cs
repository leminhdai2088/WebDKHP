using dev_DKHP.CoreModule.Dto.Common;
using dev_DKHP.CoreModule.Helper.EmailSender;

namespace dev_DKHP.Intfs
{
    public interface IEmailAppService
    {
        Task<CommonReturnDto> SendEmailAsync(MailMessageDto mailMessage);
    }
}
