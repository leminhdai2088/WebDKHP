using dev_DKHP.CoreModule.Dto.Common;
using dev_DKHP.CoreModule.Dto;

namespace dev_DKHP.Intfs
{
    public interface IMakerAppService
    {
        Task<CommonReturnDto> CLASS_INS(CLASS_ENTITY newClass);
        Task<CommonReturnDto> CLASS_UPD(CLASS_ENTITY classEntity);
    }
}
