using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PPPK_Enver_Besic.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string IllnessName { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }


        [Required(ErrorMessage = "Patient is required.")]
        public int PatientId { get; set; }

        public virtual Patient? Patient { get; set; }

    }
}
