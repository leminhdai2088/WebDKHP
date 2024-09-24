using dev_DKHP.CoreModule.Dto;

namespace dev_DKHP.Intfs
{
    public interface IBaseAppService
    {
        Task<TL_USER_ENTITY?> GetCurrentUserAsync();
    }
}
