using dev_DKHP.CoreModule.Dto;

namespace dev_DKHP.Intfs
{
    public interface IClassSubjectAppService
    {
        Task<List<CLASS_ENTITY>> CLASS_SEARCH(CLASS_ENTITY filterInput);
    }
}
