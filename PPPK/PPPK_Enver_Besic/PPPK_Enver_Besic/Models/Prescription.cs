using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PPPK_Enver_Besic.Models
{
    public class Prescription
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string MedicationName { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public DateTime DatePrescribed { get; set; }

        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
    }
}
