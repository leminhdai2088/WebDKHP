using dev_DKHP.CoreModule.Dto;

namespace dev_DKHP.CoreModule.Model
{
    public class AuthenticateResultModel
    {
        public string AccessToken { get; set; } = string.Empty;

        public string EncryptedAccessToken { get; set; } = string.Empty;

        public int ExpireInSeconds { get; set; }

        public bool ShouldResetPassword { get; set; }

        public string PasswordResetCode { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public bool RequiresTwoFactorVerification { get; set; }

        public IList<string> TwoFactorAuthProviders { get; set; } = new List<string>();

        public string TwoFactorRememberClientToken { get; set; } = string.Empty;

        public string ReturnUrl { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
        public bool IsLoginNoPassword { get; set; }
        public TL_USER_ENTITY UserLogin { get; set; }
    }
}
