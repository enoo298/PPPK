using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PPPK_Enver_Besic.Models
{
    public class Patient
    {

        public int Id { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, StringLength(11, MinimumLength = 11)]
        public string OIB { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        [Required, StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        public virtual ICollection<MedicalRecord>? MedicalRecords { get; set; }
        public virtual ICollection<Examination>? Examinations { get; set; }
        public virtual ICollection<Prescription>? Prescriptions { get; set; }

        public Patient()
        {
            MedicalRecords = new List<MedicalRecord>();
            Examinations = new List<Examination>();
            Prescriptions = new List<Prescription>();
        }
    }
}
