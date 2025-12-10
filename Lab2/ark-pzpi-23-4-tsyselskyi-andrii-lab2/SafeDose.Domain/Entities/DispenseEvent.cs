using SafeDose.Domain.Common;

namespace SafeDose.Domain.Entities
{
    public class DispenseEvent : BaseEntity<long>
    {
        public int QuantityDispensed { get; set; }
        public DateTime DispensedAt { get; set; }
        public decimal Price { get; set; }

        public long PrescriptionId { get; set; }
        public virtual Prescription Prescription { get; set; } = null!;

        public long DeviceId { get; set; }
        public virtual Device Device { get; set; } = null!;

        public long PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;

        public long DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;

        public long MedicationId { get; set; }
        public virtual Medication Medication { get; set; } = null!;
    }
}
