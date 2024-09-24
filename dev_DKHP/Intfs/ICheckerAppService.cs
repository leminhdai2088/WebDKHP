using dev_DKHP.CoreModule.Dto.Common;

namespace dev_DKHP.Intfs
{
    public interface ICheckerAppService
    {
        Task<CommonReturnDto> CLASS_APPR(string CLASS_ID);
        Task<CommonReturnDto> CLASS_REJ(string CLASS_ID);
    }
}
