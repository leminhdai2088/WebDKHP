using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dev_DKHP.CoreModule.Dto
{
    [Table("SUBJECT")]
    public class SUBJECT_ENTITY
    {
        [Key]
        public string SUBJECT_ID { get; set; } = string.Empty;
        public string SUBJECT_CODE { get; set; } = string.Empty;
        public string SUBJECT_NAME { get; set; } = string.Empty;
        public string DEP_ID { get; set; } = string.Empty;
        public string CREDIT_NUM { get; set; } = string.Empty;
        public string MAKER_ID { get; set; } = string.Empty;
        public DateTime? CREATE_DT { get; set; }
        public string CHECKER_ID { get; set; } = string.Empty;
        public DateTime? APPROVE_DT { get; set; }
        public string AUTH_STATUS { get; set; } = string.Empty;
        public int RECORD_STATUS { get; set; }
        public List<REQUIRED_SUBJECT_ENTITY>? REQUIRED_SUBJECTS { get; set; }
        public List<CLASS_ENTITY>? CLASSES { get; set; }
        [NotMapped]
        public string USER_LOGIN { get; set; } = string.Empty;
    }
}
