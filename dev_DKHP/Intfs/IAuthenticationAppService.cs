using dev_DKHP.CoreModule.Dto.Common;
using dev_DKHP.CoreModule.Dto;

namespace dev_DKHP.Intfs
{
    public interface IAuthenticationAppService
    {
        Task<CommonReturnDto> CreateUserAsync(TL_USER_ENTITY user);
        Task<CommonReturnDto> ChangePasswordAsync(string userName, string oldPassword, string newPassword);
        Task<CommonReturnDto> ForgotPasswordAsync(string email);
        Task<CommonReturnDto> RecoveryPasswordAsync(RecoveryPasswordDto body);
    }
}
