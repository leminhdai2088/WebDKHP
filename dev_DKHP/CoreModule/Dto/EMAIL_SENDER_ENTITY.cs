using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dev_DKHP.CoreModule.Dto
{
    [Table("EMAIL_SENDER")]
    public class EMAIL_SENDER_ENTITY
    {
        [Key]
        public string? EMAIL_ID { get; set; }
        public string? RECEIVER_EMAIL {  get; set; }
        public DateTime? EXPIRE_DT { get; set; }
        public bool IS_VALID { get; set; }
        public string? TYPE_EMAIL { get; set; }
        public string? SUBJECT { get; set; }
        public string? CONTENT { get; set; }
        public DateTime? CREATE_DT { get; set; }

    }
}
