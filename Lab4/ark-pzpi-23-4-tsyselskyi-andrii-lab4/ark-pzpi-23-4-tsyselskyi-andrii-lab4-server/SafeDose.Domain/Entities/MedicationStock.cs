using SafeDose.Domain.Common;

namespace SafeDose.Domain.Entities
{
    public class MedicationStock : BaseEntity<long>
    {
        public int Quantity { get; set; }
        public DateTime ProductionDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime ReceivedAt { get; set; }

        public long WorkplaceId { get; set; }
        public virtual Workplace Workplace { get; set; } = null!;

        public long MedicationId { get; set; }
        public virtual Medication Medication { get; set; } = null!;
    }
}
