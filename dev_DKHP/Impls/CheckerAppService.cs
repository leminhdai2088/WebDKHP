using dev_DKHP.CoreModule.Const;
using dev_DKHP.CoreModule.Dto.Common;
using dev_DKHP.CoreModule.Dto;
using dev_DKHP.CoreModule.Helper.Authorization;
using dev_DKHP.CoreModule.Helper.Procedure;
using dev_DKHP.CoreModule.Model;
using dev_DKHP.Intfs;
using Microsoft.EntityFrameworkCore;

namespace dev_DKHP.Impls
{
    public class CheckerAppService: ICheckerAppService
    {
        private readonly IConfiguration _configuration;
        private string? ConnectionString;
        private readonly IStoredProcedureProvider _storedProcedureProvider;
        private readonly IBaseAppService _baseAppService;
        private readonly DKHPDbContext _dbContext;
        public CheckerAppService(
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

        public async Task<CommonReturnDto> CLASS_APPR(string CLASS_ID)
        {
            var classE = await _dbContext.ClassEntities.Where(e => e.CLASS_ID == CLASS_ID).FirstOrDefaultAsync();
            if (classE.AUTH_STATUS != AuthStatusConst.NotApprove || classE.RECORD_STATUS == 0)
                throw new CustomException(-1, "Cannot approve this class");
            classE.AUTH_STATUS = AuthStatusConst.Approve;
            classE.CHECKER_ID = (await _baseAppService.GetCurrentUserAsync()).Id;
            classE.APPROVE_DT = DateTime.Now;
            _dbContext.ClassEntities.Update(classE);
            await _dbContext.SaveChangesAsync();
            return new CommonReturnDto
            {
                STATUS_CODE = 1,
                ERROR_MESSAGE = "Approved",
                DATA = classE
            };
        }

        public async Task<CommonReturnDto> CLASS_REJ(string CLASS_ID)
        {
            var classE = await _dbContext.ClassEntities.Where(e => e.CLASS_ID == CLASS_ID).FirstOrDefaultAsync();
            if (classE.AUTH_STATUS != AuthStatusConst.NotApprove || classE.RECORD_STATUS == 0)
                throw new CustomException(-1, "Cannot reject this class");
            classE.AUTH_STATUS = AuthStatusConst.Reject;
            classE.CHECKER_ID = (await _baseAppService.GetCurrentUserAsync()).Id;
            classE.APPROVE_DT = DateTime.Now;
            _dbContext.ClassEntities.Update(classE);
            await _dbContext.SaveChangesAsync();
            return new CommonReturnDto
            {
                STATUS_CODE = 1,
                ERROR_MESSAGE = "Rejected",
                DATA = classE
            };
        }
    }
}
