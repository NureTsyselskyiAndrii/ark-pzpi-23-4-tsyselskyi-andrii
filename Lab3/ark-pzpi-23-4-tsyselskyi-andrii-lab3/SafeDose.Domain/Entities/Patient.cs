using SafeDose.Domain.Common;

namespace SafeDose.Domain.Entities
{
    public class Patient : BaseEntity<long>
    {
        public string BloodType { get; set; } = null!;

        public long UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<DispenseEvent> DispenseEvents { get; set; } = new List<DispenseEvent>();
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
