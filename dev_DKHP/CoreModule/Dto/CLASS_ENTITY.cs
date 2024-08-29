using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dev_DKHP.CoreModule.Dto
{
    [Table("CLASS")]
    public class CLASS_ENTITY
    {
        [Key]
        public string CLASS_ID { get; set; } = string.Empty;
        public string CLASS_CODE { get; set; } = string.Empty;
        public string CLASS_NAME { get; set; } = string.Empty;
        public string SUBJECT_ID { get; set; } = string.Empty;
        public string TEACHER_NAME { get; set; } = string.Empty;
        public string DEP_ID { get; set; } = string.Empty;
        public int QUANTITY { get; set; }
        public int CURRENT_QUANTITY { get; set; }
        public DateTime? START_DT { get; set; }
        public DateTime? END_DT { get; set; }
        public string MAKER_ID { get; set; } = string.Empty;
        public DateTime? CREATE_DT { get; set; }
        public string CHECKER_ID { get; set; } = string.Empty;
        public DateTime? APPROVE_DT { get; set; }
        public string AUTH_STATUS { get; set; } = string.Empty;
        public int RECORD_STATUS { get; set; }
        [NotMapped]
        public SUBJECT_ENTITY? SUBJECT {  get; set; }
        [NotMapped]
        public DEPARTMENT_ENTITY? DEPARTMENT { get; set; }
        [NotMapped]
        public DateTime? FROM_DT { get; set; }
        [NotMapped]
        public DateTime? TO_DT { get; set; }
        [NotMapped]
        public int QUANTITY_FROM { get; set; }
        [NotMapped]
        public int QUANTITY_TO { get; set; }
        [NotMapped]
        public DateTime? FROM_START_DT { get; set; }
        [NotMapped]
        public DateTime? TO_START_DT { get; set; }
        [NotMapped]
        public DateTime? FROM_END_DT { get; set; }
        [NotMapped]
        public DateTime? TO_END_DT { get; set; }
        [NotMapped]
        public string USER_LOGIN { get; set; } = string.Empty;


    }
}
