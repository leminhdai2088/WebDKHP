using Microsoft.IdentityModel.Tokens;

namespace dev_DKHP.CoreModule.Helper.Authorization
{
    public class TokenAuthConfiguration
    {
        public string SecurityKey { get; set; } = string.Empty;

        public string? Issuer { get; set; } = string.Empty;

        public string? Audience { get; set; } = string.Empty;

        public SigningCredentials? SigningCredentials { get; set; }

        public TimeSpan Expiration { get; set; }
    }
}
