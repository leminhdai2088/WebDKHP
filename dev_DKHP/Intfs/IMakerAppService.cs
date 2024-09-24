using dev_DKHP.CoreModule.Dto.Common;
using dev_DKHP.CoreModule.Dto;

namespace dev_DKHP.Intfs
{
    public interface IMakerAppService
    {
        Task<IDictionary<string, object>> CLASS_INS(CLASS_ENTITY input);
        Task<CommonReturnDto> CLASS_UPD(CLASS_ENTITY input);
        Task<CommonReturnDto> CLASS_SEND_APPR(string CLASS_ID);
    }
}
