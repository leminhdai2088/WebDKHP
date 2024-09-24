using dev_DKHP.CoreModule.Dto;

namespace dev_DKHP.Intfs
{
    public interface IStudentAppService
    {
        string TestApi();
        Task<List<ENROLLED_STUDENT_ENTITY>> ENROLLED_STUDENT_SEARCH(ENROLLED_STUDENT_ENTITY finterInput);
        Task<IDictionary<string, object>> ENROLLED_STUDENT_INS(ENROLLED_STUDENT_ENTITY input);

    }
}
