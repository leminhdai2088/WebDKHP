using dev_DKHP.CoreModule.Const;
using dev_DKHP.CoreModule.Dto;
using dev_DKHP.CoreModule.Helper.Procedure;
using dev_DKHP.CoreModule.Model;
using dev_DKHP.Intfs;
namespace dev_DKHP.Impls
{
    public class ClassSubjectAppService: IClassSubjectAppService
    {
        private readonly IConfiguration _configuration;
        private string? ConnectionString;
        private readonly IStoredProcedureProvider _storedProcedureProvider;
        private readonly IBaseAppService _baseAppService;
        private readonly DKHPDbContext _dbContext;
        public ClassSubjectAppService(
            IConfiguration configuration,
            IStoredProcedureProvider storedProcedureProvider,
            IBaseAppService baseAppService,
            DKHPDbContext dbContext
            )
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetValue<string>("ConnectionStrings:Default");
            _storedProcedureProvider = storedProcedureProvider;
            _baseAppService = baseAppService;
            _dbContext = dbContext;
        }

        public async Task<List<CLASS_ENTITY>> CLASS_SEARCH(CLASS_ENTITY filterInput)
        {
            var currentUser = await _baseAppService.GetCurrentUserAsync();
            filterInput.MAKER_ID = currentUser.Id;
            return await _storedProcedureProvider.GetDataFromStoredProcedure<CLASS_ENTITY>
                (StoredProcedureConst.CLASS_SEARCH, filterInput);
        }
    }
}
