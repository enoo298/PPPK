using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PPPK_Enver_Besic.Models
{
    public class Examination
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "timestamp without time zone")]
        public DateTime ExaminationDateTime { get; set; }

        [Required]
        [StringLength(10)]
        public string ExaminationType { get; set; }

        public int PatientId { get; set; }

        public virtual Patient? Patient { get; set; }

        public virtual ICollection<ExaminationImage> ExaminationImages { get; set; }

        public Examination()
        {
            ExaminationImages = new List<ExaminationImage>();
        }
    }
}
