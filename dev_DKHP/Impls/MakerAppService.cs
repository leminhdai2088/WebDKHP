﻿using dev_DKHP.CoreModule.Const;
using dev_DKHP.CoreModule.Dto;
using dev_DKHP.CoreModule.Dto.Common;
using dev_DKHP.CoreModule.Helper.Authorization;
using dev_DKHP.CoreModule.Helper.Procedure;
using dev_DKHP.CoreModule.Model;
using dev_DKHP.Intfs;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IDictionary<string, object>> CLASS_INS(CLASS_ENTITY input)
        {
            return await _storedProcedureProvider.GetResultValueFromStore(StoredProcedureConst.CLASS_INS, input);
        }



        public async Task<CommonReturnDto> CLASS_UPD(CLASS_ENTITY input)
        {
            if (input.AUTH_STATUS != AuthStatusConst.Draft || input.RECORD_STATUS == 0) 
                throw new CustomException(-1, "Cannot update this class");

            _dbContext.ClassEntities.Update(input);
            await _dbContext.SaveChangesAsync();
            return new CommonReturnDto
            {
                STATUS_CODE = 1,
                ERROR_MESSAGE = "Updated",
                DATA = input
            };
        }

        public async Task<CommonReturnDto> CLASS_SEND_APPR(string CLASS_ID)
        {
            var classE = await _dbContext.ClassEntities.Where(e => e.CLASS_ID == CLASS_ID).FirstOrDefaultAsync();
            if (classE.AUTH_STATUS != AuthStatusConst.Draft || classE.RECORD_STATUS == 0) 
                throw new CustomException(-1, "Cannot send approve this class");
            classE.AUTH_STATUS = AuthStatusConst.NotApprove;
            _dbContext.ClassEntities.Update(classE);
            await _dbContext.SaveChangesAsync();
            return new CommonReturnDto
            {
                STATUS_CODE = 1,
                ERROR_MESSAGE = "Sent approve",
                DATA = classE
            };
        }
    }
}
