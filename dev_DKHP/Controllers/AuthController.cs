using dev_DKHP.CoreModule.Dto;
using dev_DKHP.CoreModule.Dto.Common;
using dev_DKHP.Impls;
using dev_DKHP.Intfs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dev_DKHP.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationAppService _authenticationAppService;
        private readonly IBaseAppService _baseAppService;
        public AuthController(
            IAuthenticationAppService authenticationAppService,
            IBaseAppService baseAppService
            )
        {
            _authenticationAppService = authenticationAppService;
            _baseAppService = baseAppService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<CommonReturnDto> ForgotPasswordAsync(string email)
        {
            return await _authenticationAppService.ForgotPasswordAsync(email);
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<CommonReturnDto> RecoveryPasswordAsync([FromBody] RecoveryPasswordDto body)
        {
            return await _authenticationAppService.RecoveryPasswordAsync(body);
        }
    }
}
