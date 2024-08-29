using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dev_DKHP.CoreModule.Dto
{
    [Table("REQUIRED_SUBJECT")]
    public class REQUIRED_SUBJECT_ENTITY
    {
        [Key]
        public string SUBJECT_ID { get; set; } = string.Empty;
        [Key]
        public string REQUIRED_SUBJECT_ID { get; set; } = string.Empty;
        public string MAKER_ID { get; set; } = string.Empty;
        public DateTime? CREATE_DT { get; set; }
        public string CHECKER_ID { get; set; } = string.Empty;
        public DateTime? APPROVE_DT { get; set; }
        public string AUTH_STATUS { get; set; } = string.Empty;
        public int RECORD_STATUS { get; set; }
        [NotMapped]
        public SUBJECT_ENTITY? REQUIRED_SUBJECT { get; set; }
    }
}
