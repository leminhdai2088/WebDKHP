using dev_DKHP.CoreModule.Dto;
using dev_DKHP.CoreModule.Dto.Common;
using dev_DKHP.Intfs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dev_DKHP.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClassSubjectController : ControllerBase
    {
        private readonly IClassSubjectAppService _classSubjectAppService;
        private readonly IMakerAppService _makerAppService;
        private readonly ICheckerAppService _checkerAppService;

        public ClassSubjectController(
            IClassSubjectAppService classSubjectAppService,
            IMakerAppService makerAppService,
            ICheckerAppService checkerAppService
            )
        {
            _classSubjectAppService = classSubjectAppService;
            _makerAppService = makerAppService;
            _checkerAppService = checkerAppService;
        }

        [HttpPost]
        public async Task<List<CLASS_ENTITY>> CLASS_SEARCH(CLASS_ENTITY filterInput)
        {
            return await _classSubjectAppService.CLASS_SEARCH(filterInput);
        }

        [HttpPost]
        public async Task<IDictionary<string, object>> CLASS_INS([FromBody] CLASS_ENTITY input)
        {
            return await _makerAppService.CLASS_INS(input);
        }

        [HttpPost]
        public async Task<CommonReturnDto> CLASS_UPD([FromBody] CLASS_ENTITY input)
        {
            return await _makerAppService.CLASS_UPD(input);
        }

        [HttpPost]
        public async Task<CommonReturnDto> CLASS_SEND_APPR([FromBody] string CLASS_ID)
        {
            return await _makerAppService.CLASS_SEND_APPR(CLASS_ID);
        }

        [HttpPost]
        public async Task<CommonReturnDto> CLASS_APPR([FromBody] string CLASS_ID)
        {
            return await _checkerAppService.CLASS_APPR(CLASS_ID);
        }

        [HttpPost]
        public async Task<CommonReturnDto> CLASS_REJ([FromBody] string CLASS_ID)
        {
            return await _checkerAppService.CLASS_REJ(CLASS_ID);
        }
    }
}
