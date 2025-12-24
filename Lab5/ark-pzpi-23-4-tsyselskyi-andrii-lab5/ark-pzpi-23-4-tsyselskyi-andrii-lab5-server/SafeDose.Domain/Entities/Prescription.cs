using SafeDose.Domain.Common;

namespace SafeDose.Domain.Entities
{
    public class Prescription : BaseEntity<long>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public long PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;

        public long DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;

        public virtual ICollection<DispenseEvent> DispenseEvents { get; set; } = new List<DispenseEvent>();
        public virtual ICollection<PrescriptionMedication> PrescriptionMedications { get; set; } = new List<PrescriptionMedication>();
    }
}
