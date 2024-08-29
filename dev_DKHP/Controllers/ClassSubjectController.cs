using dev_DKHP.CoreModule.Dto;
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

        public ClassSubjectController(IClassSubjectAppService classSubjectAppService)
        {
            _classSubjectAppService = classSubjectAppService;
        }

        [HttpPost]
        public async Task<List<CLASS_ENTITY>> CLASS_SEARCH(CLASS_ENTITY filterInput)
        {
            return await _classSubjectAppService.CLASS_SEARCH(filterInput);
        }
    }
}
