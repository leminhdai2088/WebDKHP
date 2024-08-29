using dev_DKHP.CoreModule.Const;
using dev_DKHP.CoreModule.Dto;
using dev_DKHP.CoreModule.Dto.Common;
using dev_DKHP.CoreModule.Helper.Authorization;
using dev_DKHP.CoreModule.Helper.Procedure;
using dev_DKHP.CoreModule.Model;
using dev_DKHP.Intfs;

namespace dev_DKHP.Impls
{
    public class MakerAppService: IMakerAppService
    {
        private readonly IConfiguration _configuration;
        private string? ConnectionString;
        private readonly IStoredProcedureProvider _storedProcedureProvider;
        private readonly IBaseAppService _baseAppService;
        private readonly DKHPDbContext _dbContext;
        public MakerAppService(
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

        public async Task<CommonReturnDto> CLASS_INS(CLASS_ENTITY newClass)
        {
            var isExistsClass = _dbContext.ClassEntities.Where(e => e.CLASS_CODE == newClass.CLASS_CODE).Any();
            if (isExistsClass) throw new CustomException(-1, "CLASS CODE had already exists");
            var user = await _baseAppService.GetCurrentUserAsync();
            newClass.CLASS_ID = Guid.NewGuid().ToString() + DateTime.Now.ToString();
            newClass.AUTH_STATUS = AuthStatusConst.Draft;
            newClass.MAKER_ID = user.Id;
            newClass.CREATE_DT = DateTime.Now;
            newClass.RECORD_STATUS = 1;
            newClass.DEP_ID = user.DEP_ID;

            _dbContext.ClassEntities.Add(newClass);
            await _dbContext.SaveChangesAsync();
            return new CommonReturnDto
            {
                STATUS_CODE = 1,
                ERROR_MESSAGE = "Inserted",
                DATA = newClass
            };
        }

        public async Task<CommonReturnDto> CLASS_UPD(CLASS_ENTITY classEntity)
        {
            if (classEntity.AUTH_STATUS != AuthStatusConst.Draft || classEntity.RECORD_STATUS == 0) 
                throw new CustomException(-1, "Cannot update this class");

            _dbContext.ClassEntities.Update(classEntity);
            await _dbContext.SaveChangesAsync();
            return new CommonReturnDto
            {
                STATUS_CODE = 1,
                ERROR_MESSAGE = "Updated",
                DATA = classEntity
            };
        }
    }
}
