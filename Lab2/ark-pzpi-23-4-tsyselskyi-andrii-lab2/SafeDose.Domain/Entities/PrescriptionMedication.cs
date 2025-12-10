using SafeDose.Domain.Common;

namespace SafeDose.Domain.Entities
{
    public class PrescriptionMedication : BaseEntity<long>
    {
        public decimal Dosage { get; set; }
        public int QuantityOfDosagesOverall { get; set; }
        public int PeriodInDays { get; set; }
        public string? Description { get; set; }
        public decimal Discount { get; set; }

        public long PrescriptionId { get; set; }
        public virtual Prescription Prescription { get; set; } = null!;

        public long MedicationId { get; set; }
        public virtual Medication Medication { get; set; } = null!;

    }
}
