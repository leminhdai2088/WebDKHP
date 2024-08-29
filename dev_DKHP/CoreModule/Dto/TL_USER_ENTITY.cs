using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace dev_DKHP.CoreModule.Dto
{
    [Table("TL_USER")]
    public class TL_USER_ENTITY: IdentityUser
    {
        public string USER_CODE { get; set; } = string.Empty;
        public string DEP_ID { get; set; } = string.Empty;
        [NotMapped]
        public string PASSWORD { get; set; } = string.Empty;
        [SwaggerIgnore]
        [JsonIgnore]
        public override string? PasswordHash { get => base.PasswordHash; set => base.PasswordHash = value; }
        public List<IdentityUserRole<string>>? ROLES { get; set; }
        public DEPARTMENT_ENTITY? DEPARTMENT { get; set; }
        public List<ENROLLED_STUDENT_ENTITY>? ENROLLED_STUDENT { get; set;}
    }
}
