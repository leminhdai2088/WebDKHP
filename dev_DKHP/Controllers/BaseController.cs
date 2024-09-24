using dev_DKHP.CoreModule.Dto;
using dev_DKHP.Intfs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dev_DKHP.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly IBaseAppService _baseAppService;
        public BaseController(
            IAuthenticationAppService authenticationAppService,
            IBaseAppService baseAppService
            )
        {
            _baseAppService = baseAppService;
        }
        [HttpPost]
        public async Task<TL_USER_ENTITY> GetCurrentUserAsync()
        {
            return await _baseAppService.GetCurrentUserAsync();
        }
    }
}
