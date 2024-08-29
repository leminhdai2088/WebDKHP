using System.ComponentModel.DataAnnotations;

namespace dev_DKHP.CoreModule.Dto
{
    public class RecoveryPasswordDto
    {
        [Required]
        public string PASSWORD { get; set; } = string.Empty;
        [Required]
        public string OTP { get; set; } = string.Empty;
        [Required]
        public string EMAIL {  get; set; } = string.Empty;
    }
}
