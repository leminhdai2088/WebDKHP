using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dev_DKHP.CoreModule.Dto
{
    [Table("DEPARTMENT")]
    public class DEPARTMENT_ENTITY
    {
        [Key]
        public string DEP_ID { get; set; } = string.Empty;
        public string DEP_CODE { get; set; } = string.Empty;
        public string DEP_NAME { get; set; } = string.Empty;
        public List<SUBJECT_ENTITY>? SUBJECTS { get; set; }
    }
}
