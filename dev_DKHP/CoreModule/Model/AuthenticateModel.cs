using System.ComponentModel.DataAnnotations;

namespace dev_DKHP.CoreModule.Model
{
    public class AuthenticateModel
    {
        [Required]
        [MaxLength(256)]
        public string UserNameOrEmailAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(short.MaxValue)]
        public string Password { get; set; } = string.Empty;

        public string TwoFactorVerificationCode { get; set; } = string.Empty;

        public bool RememberClient { get; set; }

        public string TwoFactorRememberClientToken { get; set; } = string.Empty;

        public bool? SingleSignIn { get; set; }

        public string ReturnUrl { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}
