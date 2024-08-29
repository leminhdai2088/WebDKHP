using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dev_DKHP.CoreModule.Dto
{
    [Table("ENROLLED_STUDENT")]
    public class ENROLLED_STUDENT_ENTITY
    {
        [Key]
        public string ENROLLED_ID { get; set; } = string.Empty;
        public string STUDENT_ID { get; set; } = string.Empty;
        public string SUBJECT_ID { get; set; } = string.Empty;
        public string CLASS_ID { get; set; } = string.Empty;
        public bool IS_FINISH { get; set; }
        public DateTime? CREATE_DT { get; set; }
        public string AUTH_STATUS { get; set; } = string.Empty;
        public int RECORD_STATUS { get; set; }
        public TL_USER_ENTITY? STUDENT {  get; set; }
        public SUBJECT_ENTITY? SUBJECT { get; set; }
        public CLASS_ENTITY? CLASS { get; set; }

        // FILTER
        public DEPARTMENT_ENTITY? DEPARTMENT { get; set; }
        [NotMapped]
        public DateTime? FROM_DT { get; set; }
        [NotMapped]
        public DateTime? TO_DT { get; set; }
        [NotMapped]
        public string USER_LOGIN { get; set; } = string.Empty;
    }
}
