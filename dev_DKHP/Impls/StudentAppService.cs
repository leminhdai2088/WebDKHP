using dev_DKHP.Intfs;
using Dapper;
using dev_DKHP.CoreModule.Dto.Procedure;
using Microsoft.Data.SqlClient;
using dev_DKHP.CoreModule.Helper.Procedure;
using dev_DKHP.CoreModule.Dto;
using dev_DKHP.CoreModule.Const;
namespace dev_DKHP.Impls
{
    public class StudentAppService : IStudentAppService
    {
        private readonly IConfiguration _configuration;
        private string? ConnectionString;
        private readonly IStoredProcedureProvider _storedProcedureProvider;
        private readonly IBaseAppService _baseAppService;
        public StudentAppService(
            IConfiguration configuration,
            IStoredProcedureProvider storedProcedureProvider,
            IBaseAppService baseAppService
            )
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetValue<string>("ConnectionStrings:Default");
            _storedProcedureProvider = storedProcedureProvider;
            _baseAppService = baseAppService;
        }
        public async Task<List<object>> TestApi()
        {
            string queryString = "select PARAMETER_NAME, PARAMETER_MODE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH from information_schema.parameters where specific_name = @storeProcName";
            using (var conn = new SqlConnection(ConnectionString))
            {
                var result = await conn.QueryAsync<object>(queryString, new { storeProcName = "ProcTest" });
                return result.ToList();
            }
        }

        public async Task<List<ENROLLED_STUDENT_ENTITY>> ENROLLED_STUDENT_SEARCH(ENROLLED_STUDENT_ENTITY finterInput)
        {
            var ENROLLED_STUDENT = await _storedProcedureProvider.GetDataFromStoredProcedure<ENROLLED_STUDENT_ENTITY>
                (StoredProcedureConst.ENROLLED_STUDENT_SEARCH, finterInput);
            foreach(var item in ENROLLED_STUDENT)
            {
                item.SUBJECT = (await _storedProcedureProvider.GetDataFromStoredProcedure<SUBJECT_ENTITY>
                    (StoredProcedureConst.SUBJECT_BY_ID, new {P_SUBJECT_ID = item.SUBJECT_ID })).FirstOrDefault();

                item.CLASS = (await _storedProcedureProvider.GetDataFromStoredProcedure<CLASS_ENTITY>
                    (StoredProcedureConst.CLASS_BY_ID, new { P_CLASS_ID = item.CLASS_ID })).FirstOrDefault();
            }
            return ENROLLED_STUDENT;
        }

        public async Task<IDictionary<string, object>> ENROLLED_STUDENT_INS(ENROLLED_STUDENT_ENTITY input)
        {
            return await _storedProcedureProvider.GetResultValueFromStore(StoredProcedureConst.ENROLLED_STUDENT_INS, input);
        }
    }
}
