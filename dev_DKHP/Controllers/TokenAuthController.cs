using Microsoft.AspNetCore.Mvc;
using dev_DKHP.CoreModule.Model;
using Microsoft.AspNetCore.Identity;
using dev_DKHP.CoreModule.Dto;
using dev_DKHP.CoreModule.Dto.Common;
using dev_DKHP.CoreModule.Helper.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using dev_DKHP.CoreModule.Const;
using Microsoft.Extensions.Caching.Memory;
using dev_DKHP.Intfs;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using dev_DKHP.Impls;
using static Dapper.SqlMapper;
using Microsoft.AspNetCore.Authorization;
namespace dev_DKHP.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TokenAuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<TL_USER_ENTITY> _userManager;
        private readonly SignInManager<TL_USER_ENTITY> _signInManager;
        private readonly TokenAuthConfiguration _tokenAuthConfiguration;
        private readonly IAuthenticationAppService _authenticationAppService;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenAuthController(
            IConfiguration configuration,
            UserManager<TL_USER_ENTITY> userManager,
            SignInManager<TL_USER_ENTITY> signInManager,
            TokenAuthConfiguration tokenAuthConfiguration,
            IAuthenticationAppService authenticationAppService,
            IMemoryCache cache,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenAuthConfiguration = tokenAuthConfiguration;
            _authenticationAppService = authenticationAppService;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPut]
        public async Task<AuthenticateResultModel> Login([FromBody] AuthenticateModel model)
        {
            return await Authenticate(model);
        }


        [HttpGet]
        public async Task<CommonReturnDto> Logout()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                // Nếu không tìm thấy userId, trả về lỗi
                return new CommonReturnDto
                {
                    STATUS_CODE = -1,
                    ERROR_MESSAGE = "Logout failed!"
                };
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                // Xóa token từ cache
                var tokenValidityKeyInClaims = _httpContextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type == AppConsts.TokenValidityKey)?.Value;

                if (!string.IsNullOrEmpty(tokenValidityKeyInClaims))
                {
                    _cache.Remove(tokenValidityKeyInClaims);
                }

                // Xóa token authentication
                await _userManager.RemoveAuthenticationTokenAsync(user, "DKHPProvider", "AccessToken");

                // Đăng xuất khỏi SignInManager
                await _signInManager.SignOutAsync();

                // Xóa claims khỏi HttpContext
                var claimsIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

                if (claimsIdentity != null)
                {
                    foreach (var claim in claimsIdentity.Claims.ToList())
                    {
                        claimsIdentity.RemoveClaim(claim);
                    }
                    _httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
                }

                // Xóa session
                _httpContextAccessor.HttpContext.Session.Clear();
                _httpContextAccessor.HttpContext.Session.Remove(ClaimTypes.NameIdentifier);


                // Xóa cookie authentication
                var authenticationCookieName = "DKHP_IdentityCookie";
                if (_httpContextAccessor.HttpContext.Request.Cookies[authenticationCookieName] != null)
                {
                    _httpContextAccessor.HttpContext.Response.Cookies.Delete(authenticationCookieName);
                }

                return new CommonReturnDto
                {
                    STATUS_CODE = 0,
                    ERROR_MESSAGE = "Logout successful!"
                };
            }

            // Nếu không tìm thấy user, trả về lỗi
            return new CommonReturnDto
            {
                STATUS_CODE = -1,
                ERROR_MESSAGE = "Logout failed!"
            };
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<CommonReturnDto> CreateUserAsync([FromBody] TL_USER_ENTITY user)
        {
            return await _authenticationAppService.CreateUserAsync(user);
        }
        private async Task<AuthenticateResultModel> Authenticate(AuthenticateModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserNameOrEmailAddress);
            if (user == null)
            {
                throw new CustomException(-1, "User not found");
            }

            var checkPassword = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!checkPassword.Succeeded)
            {
                throw new CustomException(-1, "Invalid password");
            }

            var identity = new ClaimsIdentity("Identity.Application");

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim("USER_CODE", user.USER_CODE));
            identity.AddClaim(new Claim("AspNet.Identity.SecurityStamp", user.SecurityStamp));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));


            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            string accessToken = CreateAccessToken(await CreateJwtClaims(identity, user));
            string refreshToken = CreateAccessToken(await CreateJwtClaims(identity, user));

            await _userManager.SetAuthenticationTokenAsync(user, "DKHPProvider", "AccessToken", accessToken);


            return new AuthenticateResultModel()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IsLoginNoPassword = true,
                UserLogin = user,
                UserId = user.Id
            };
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expires = null)
        {
            var token = CreateToken(claims, expires ?? _tokenAuthConfiguration.Expiration);

            if (Convert.ToBoolean(_configuration.GetValue<bool>("Securiry:HttpOnly") == true))
            {
                CookieOptions option = new CookieOptions
                {
                    Expires = DateTime.Now.AddMilliseconds(_configuration.GetValue<long>("Securiry:TimeCookieRefresh")),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Strict
                };
                Response.Cookies.Append("DKHP_IdentityCookie", token, option);
            }
            return token;
        }

        private string CreateRefreshToken(IEnumerable<Claim> claims)
        {
            return CreateToken(claims, AppConsts.RefreshTokenExpiration);
        }

        private string CreateToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            expiration = TimeSpan.FromMicroseconds(_configuration.GetValue<Int64>("Securiry:TimeCookieRefresh"));
            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenAuthConfiguration.SecurityKey));
            var now = DateTime.Now;
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenAuthConfiguration.Issuer,
                audience: _tokenAuthConfiguration.Audience,
                claims: claims,
                notBefore: now,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature),
                expires: expiration == null ? (DateTime?)null : now.Add(expiration.Value)
            );
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private async Task<IEnumerable<Claim>> CreateJwtClaims(ClaimsIdentity identity, TL_USER_ENTITY user, TimeSpan? expiration = null)
        {
            var tokenValidityKey = Guid.NewGuid().ToString();
            var claims = identity.Claims.ToList();

            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(AppConsts.TokenValidityKey, tokenValidityKey),
                new Claim(AppConsts.USER_DEPID, user.DEP_ID),
            });

            _cache.Set(tokenValidityKey, "");

            await _userManager.AddClaimsAsync(user, claims);
            return claims;
        }

    }
}
