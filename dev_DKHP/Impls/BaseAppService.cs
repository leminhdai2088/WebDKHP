using dev_DKHP.CoreModule.Dto;
using dev_DKHP.CoreModule.Helper.Authorization;
using dev_DKHP.Intfs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Session;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace dev_DKHP.Impls
{
    public class BaseAppService: IBaseAppService
    {
        private readonly UserManager<TL_USER_ENTITY> _userManager;
        private IHttpContextAccessor _httpContextAccessor;
        public BaseAppService(
            UserManager<TL_USER_ENTITY> userManager,
            IHttpContextAccessor httpContextAccessor
            ) 
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<TL_USER_ENTITY?> GetCurrentUserAsync()
        {
            var user = await _userManager.FindByIdAsync(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null) return null;
            return user;
        }

        public string? GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        }

        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        public string GetClaimValue(string claimKey)
        {
            var claims = _httpContextAccessor.HttpContext?.User.Identities.First().Claims;
            return claims.FirstOrDefault(x => x.Type == claimKey)?.Value;
        }

        public string GetFileName(string filePath)
        {
            string fileName = null;
            if (filePath != null)
            {
                filePath = filePath.Replace("\\", "/");
                fileName = filePath.Substring(filePath.LastIndexOf("/") + 1);
            }
            return fileName;
        }
    }
}
